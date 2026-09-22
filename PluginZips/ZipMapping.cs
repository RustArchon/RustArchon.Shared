// Copyright ©2026 Scott Blomfield

using System.Text;

namespace RustArchon.Shared.PluginZips;

/// <summary>
/// The places a file from a plugin's zip archive can be put on a game server, by <b>role</b> and not by path. The RustArchon plugin on the server turns a
/// role into the right folder for the framework it runs (Carbon keeps its settings in <c>configs</c>, Oxide in <c>config</c>). A fixed list, on purpose: an
/// archive that could name any path could overwrite the game itself.
/// </summary>
public static class ZipRoles
{
    public const string Plugins = "plugins";
    public const string Config = "config";
    public const string Data = "data";
    public const string Lang = "lang";

    /// <summary>Not installed. A skipped file is a decision, not an omission: every file in the archive has to be installed or skipped.</summary>
    public const string Skip = "skip";

    public static readonly IReadOnlyList<string> Installable = [Plugins, Config, Data, Lang];

    public static bool IsRole(string? role) => role == Skip || (role is not null && Installable.Contains(role));
}

/// <summary>
/// One instruction about a file or folder in the archive: where it goes, or that it is skipped. A <b>folder</b> rule covers everything under that folder
/// and puts it at the destination with the structure below the folder kept (<c>en/images/</c> to <c>data</c> puts <c>en/images/test/one.jpg</c> at
/// <c>data/test/one.jpg</c>). A <b>file</b> rule covers exactly one file and beats the folder rules, so a file can be skipped, or sent elsewhere, from
/// inside a folder that is otherwise mapped. Of several folder rules that cover a file, the deepest wins.
/// </summary>
public class ZipMappingRule
{
    /// <summary>True for a folder rule (<see cref="Source"/> is a folder), false for a rule about one file.</summary>
    public bool IsFolder { get; set; }

    /// <summary>The folder (with or without a trailing slash) or the file, as the archive spells it, with forward slashes.</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>One of <see cref="ZipRoles"/>: an installable role, or <see cref="ZipRoles.Skip"/>.</summary>
    public string Role { get; set; } = ZipRoles.Skip;

    /// <summary>A folder inside the role's folder to put the files in (<c>images</c> puts them in <c>data/images/...</c>). Empty for the role's own folder.</summary>
    public string SubFolder { get; set; } = string.Empty;

    /// <summary>
    /// Leave a file alone if one of that name is already on the server. Off by default: a file that is mapped is installed, and the file it replaces is
    /// backed up. Turned on for something a customer edits, such as a settings file.
    /// </summary>
    public bool KeepExisting { get; set; }
}

/// <summary>A file in the archive as the check of it recorded: its path and its declared size (which is a claim, and is checked when it is unpacked).</summary>
public sealed record ZipEntryInfo(string Path, long Size);

/// <summary>What is to happen to one file: <see cref="Install"/>, <see cref="Skip"/>, or <see cref="Unassigned"/> (no rule covers it).</summary>
public enum ZipEntryAction { Install, Skip, Unassigned }

/// <summary>One file of the archive with what the rules decide for it.</summary>
/// <param name="Destination">For an installed file: the role, a slash, and the path below it (<c>data/test/one.jpg</c>); empty otherwise.</param>
public sealed record ZipMappedEntry(string Path, long Size, ZipEntryAction Action, string Role, string Destination, bool KeepExisting);

/// <summary>Something wrong with the mapping. <see cref="Code"/> is one of <see cref="ZipMappingProblemCodes"/>; <see cref="Path"/> is the file or rule it is about.</summary>
public sealed record ZipMappingProblem(string Code, string Path, string Message);

/// <summary>The codes of <see cref="ZipMappingProblem"/>.</summary>
public static class ZipMappingProblemCodes
{
    public const string BadRule = "bad_rule";
    public const string Unassigned = "unassigned";
    public const string UnsafePath = "unsafe_path";
    public const string Executable = "executable";
    public const string Collision = "collision";
    public const string CodeOutsidePlugins = "cs_outside_plugins";
}

/// <summary>What a set of rules makes of an archive's files.</summary>
public sealed class ZipMappingResult
{
    public List<ZipMappedEntry> Entries { get; } = [];
    public List<ZipMappingProblem> Problems { get; } = [];

    /// <summary>The files that will be installed.</summary>
    public IEnumerable<ZipMappedEntry> Installed => Entries.Where(e => e.Action == ZipEntryAction.Install);

    /// <summary>The declared size of everything that will be installed, which is the most the server will unpack.</summary>
    public long InstallBytes => Installed.Sum(e => e.Size);

    /// <summary>Every file is installed or skipped, and nothing about the mapping is wrong: it can be applied.</summary>
    public bool IsValid => Problems.Count == 0;

    /// <summary>Whether every file has a decision, whatever else may be wrong - what a <i>saved</i> mapping has to satisfy for a new version of the archive.</summary>
    public bool CoversEveryFile => Entries.All(e => e.Action != ZipEntryAction.Unassigned);
}

/// <summary>
/// Works out, from an archive's files and a person's rules, what happens to each file, and finds everything that would make that unsafe or ambiguous.
/// The one place these rules are decided: the Api checks with it, the page previews with it, and the plugin on the server has its own copy of the same
/// algorithm, which the tests hold to the same answers.
/// </summary>
public static class ZipMapping
{
    /// <summary>Most rules a mapping may have. It is sent to a game server as one console command, so it has to stay short.</summary>
    public const int MaxRules = 60;

    /// <summary>The longest a path or folder name in a rule may be.</summary>
    public const int MaxPathLength = 240;

    /// <summary>
    /// The largest plugin file or archive anything here will take in: the Api will not download more, and a game server's plugin will not be told to. A
    /// safety bound on memory, not a judgement about plugins - real ones are kilobytes to a few megabytes. Both ends hold to it on their own, so a lying
    /// size (a header that claims little and a host that sends a lot, or the reverse) cannot make either allocate without limit.
    /// </summary>
    public const long MaxArchiveBytes = 128L * 1024 * 1024;

    /// <summary>The most an archive may unpack to on a game server, counted as bytes are actually produced and never from the archive's own headers.</summary>
    public const long MaxInstallBytes = 512L * 1024 * 1024;

    /// <summary>The most files an archive may hold and still be looked at: what a central directory can hold far exceeds anything a plugin is.</summary>
    public const int MaxArchiveFiles = 2000;

    /// <summary>File types that are programs, not data. A game server runs them with full privileges, and nothing here can read one to see what it does.</summary>
    private static readonly HashSet<string> ExecutableExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".dll", ".so", ".dylib", ".exe", ".bat", ".cmd", ".sh", ".ps1", ".msi", ".com", ".scr", ".vbs", ".jar", ".bin"
    };

    /// <summary>True for a file that is a program (by its extension), which is never installed from an archive.</summary>
    public static bool IsExecutable(string path) => ExecutableExtensions.Contains(System.IO.Path.GetExtension(path));

    /// <summary>The path as rules and archives compare it: forward slashes, no leading slash.</summary>
    public static string NormalizePath(string path) => path.Replace('\\', '/').TrimStart('/');

    /// <summary>Whether a folder or file name from an archive or a rule is safe to build a destination path from.</summary>
    public static bool IsSafePath(string path)
    {
        if (path.Length == 0 || path.Length > MaxPathLength)
        {
            return false;
        }

        foreach (var segment in path.Split('/'))
        {
            if (segment.Length == 0 || segment == "." || segment == ".." || segment.EndsWith('.') || segment.EndsWith(' ') || segment.StartsWith(' '))
            {
                return false;
            }

            foreach (var c in segment)
            {
                if (c < ' ' || c is '\\' or ':' or '*' or '?' or '"' or '<' or '>' or '|' or '')
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>Decides what happens to each of <paramref name="entries"/> under <paramref name="rules"/>.</summary>
    public static ZipMappingResult Resolve(IReadOnlyList<ZipEntryInfo> entries, IReadOnlyList<ZipMappingRule> rules)
    {
        var result = new ZipMappingResult();
        var usable = new List<(ZipMappingRule Rule, string Source)>();

        if (rules.Count > MaxRules)
        {
            result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.BadRule, string.Empty, $"there are more than {MaxRules} rules"));
        }

        foreach (var rule in rules.Take(MaxRules))
        {
            var source = NormalizePath(rule.Source ?? string.Empty);
            if (rule.IsFolder)
            {
                source = source.TrimEnd('/') + "/";
            }

            var sub = NormalizePath(rule.SubFolder ?? string.Empty).TrimEnd('/');
            var label = rule.Source ?? string.Empty;
            if (source.Length <= (rule.IsFolder ? 1 : 0) || !IsSafePath(source.TrimEnd('/')))
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.BadRule, label, "the path in the rule is not one that can be used"));
            }
            else if (!ZipRoles.IsRole(rule.Role))
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.BadRule, label, "the destination is not one of the places files can be put"));
            }
            else if (sub.Length > 0 && (rule.Role == ZipRoles.Skip || !IsSafePath(sub)))
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.BadRule, label, "the folder inside the destination is not one that can be used"));
            }
            else
            {
                usable.Add((rule, source));
            }
        }

        var destinations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in entries)
        {
            var path = NormalizePath(entry.Path);
            if (!IsSafePath(path))
            {
                result.Entries.Add(new ZipMappedEntry(path, entry.Size, ZipEntryAction.Unassigned, string.Empty, string.Empty, false));
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.UnsafePath, path, "the path of this file is not one that can be used"));
                continue;
            }

            var match = Match(path, usable);
            if (match is null)
            {
                result.Entries.Add(new ZipMappedEntry(path, entry.Size, ZipEntryAction.Unassigned, string.Empty, string.Empty, false));
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.Unassigned, path, "no rule says where this file goes, or that it is skipped"));
                continue;
            }

            var (rule, source) = match.Value;
            if (rule.Role == ZipRoles.Skip)
            {
                result.Entries.Add(new ZipMappedEntry(path, entry.Size, ZipEntryAction.Skip, ZipRoles.Skip, string.Empty, false));
                continue;
            }

            var below = rule.IsFolder ? path[source.Length..] : path[(path.LastIndexOf('/') + 1)..];
            var sub = NormalizePath(rule.SubFolder ?? string.Empty).TrimEnd('/');
            var destination = rule.Role + "/" + (sub.Length > 0 ? sub + "/" : string.Empty) + below;
            result.Entries.Add(new ZipMappedEntry(path, entry.Size, ZipEntryAction.Install, rule.Role, destination, rule.KeepExisting));

            if (IsExecutable(path))
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.Executable, path, "this is a program file, which is never installed from an archive"));
            }

            if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) && rule.Role != ZipRoles.Plugins)
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.CodeOutsidePlugins, path, "a plugin source file can only go to the plugins folder"));
            }

            if (!IsSafePath(destination))
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.UnsafePath, path, "the place this file would go is not one that can be used"));
            }
            else if (destinations.TryGetValue(destination, out var other))
            {
                result.Problems.Add(new ZipMappingProblem(ZipMappingProblemCodes.Collision, path, $"this file and {other} would both be put at {destination}"));
            }
            else
            {
                destinations[destination] = path;
            }
        }

        // A program file that is skipped is fine: it is not installed. One that is mapped is a problem, above. A program file with no rule at all is
        // already reported as unassigned.
        return result;
    }

    /// <summary>The rule that decides <paramref name="path"/>: a rule for that exact file, else the deepest folder rule above it, else none.</summary>
    private static (ZipMappingRule Rule, string Source)? Match(string path, List<(ZipMappingRule Rule, string Source)> rules)
    {
        (ZipMappingRule Rule, string Source)? best = null;
        foreach (var candidate in rules)
        {
            if (!candidate.Rule.IsFolder)
            {
                if (string.Equals(candidate.Source, path, StringComparison.Ordinal))
                {
                    return candidate;
                }

                continue;
            }

            if (path.StartsWith(candidate.Source, StringComparison.Ordinal) && (best is null || candidate.Source.Length > best.Value.Source.Length))
            {
                best = candidate;
            }
        }

        return best;
    }

    /// <summary>
    /// The rules as one argument for a console command: URL-safe base64 (letters, digits, <c>-</c> and <c>_</c>, so it has no spaces to break the command
    /// apart) of one line per rule, <c>F|E</c> (folder or file), source, role, sub folder, <c>1|0</c> keep existing, separated by tabs. Null if the rules
    /// cannot be sent (too many, too long, or a tab or newline in a name).
    /// </summary>
    public static string? Encode(IReadOnlyList<ZipMappingRule> rules)
    {
        if (rules.Count > MaxRules)
        {
            return null;
        }

        var text = new StringBuilder();
        foreach (var rule in rules)
        {
            var fields = new[] { rule.IsFolder ? "F" : "E", rule.Source ?? string.Empty, rule.Role ?? string.Empty, rule.SubFolder ?? string.Empty, rule.KeepExisting ? "1" : "0" };
            if (fields.Any(f => f.Contains('\t') || f.Contains('\n') || f.Contains('\r')))
            {
                return null;
            }

            text.Append(string.Join('\t', fields)).Append('\n');
        }

        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(text.ToString())).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return encoded.Length is > 0 and <= MaxEncodedLength ? encoded : null;
    }

    /// <summary>The longest <see cref="Encode"/> result a game server is sent.</summary>
    public const int MaxEncodedLength = 3000;

    /// <summary>The rules an <see cref="Encode"/>d argument holds, or null if it is not one.</summary>
    public static List<ZipMappingRule>? Decode(string? encoded)
    {
        if (string.IsNullOrEmpty(encoded) || encoded.Length > MaxEncodedLength)
        {
            return null;
        }

        try
        {
            var padded = encoded.Replace('-', '+').Replace('_', '/');
            padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
            var text = new UTF8Encoding(false, true).GetString(Convert.FromBase64String(padded));
            var rules = new List<ZipMappingRule>();
            foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var fields = line.Split('\t');
                if (fields.Length != 5 || fields[0] is not ("F" or "E") || fields[4] is not ("0" or "1"))
                {
                    return null;
                }

                rules.Add(new ZipMappingRule { IsFolder = fields[0] == "F", Source = fields[1], Role = fields[2], SubFolder = fields[3], KeepExisting = fields[4] == "1" });
            }

            return rules.Count is > 0 and <= MaxRules ? rules : null;
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            return null;
        }
    }
}

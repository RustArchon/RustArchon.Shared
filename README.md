# RustArchon.Shared

DTOs shared between [RustArchon.Panel](https://github.com/RustArchon/RustArchon.Panel) and
[RustArchon.Api](https://github.com/RustArchon/RustArchon.Api) - `RustServerDto`,
`CreateRustServerDto`, `UpdateRustServerDto`, and friends. Pure data contracts; no business logic.

Part of the [RustArchon](https://github.com/RustArchon/RustArchon) system - see that repo for the
full architecture and how to run the whole stack locally or via Docker Compose.

## Key files

- `DTOs/RustServerDto.cs` - the server registration/read model (never includes the RCON password -
  see `RustArchon.Api`'s `RconCredentialProtector`).
- `DTOs/InvitationCodeDto.cs`, `InvitationRedemptionDtos.cs` - the invitation-gated sign-up contracts
  between Panel and Api.
- `DTOs/SendEmailRequestDto.cs` - the request shape for Api's internal `/internal/email` endpoint.

## License

AGPL-3.0-or-later - see [`LICENSE`](LICENSE). This project also depends on
[JumpStart](https://github.com/cyberknet/JumpStart), a separate GPL-3.0-or-later project - see
[`NOTICE.md`](NOTICE.md) for how the two combine.

## Building standalone

**This repo cannot be built on its own.** It reaches JumpStart via a `ProjectReference` to
`../JumpStart/JumpStart/JumpStart.csproj`, a path that only resolves inside the
[umbrella repo's](https://github.com/RustArchon/RustArchon) submodule layout. Clone that instead:

```bash
git clone --recurse-submodules https://github.com/RustArchon/RustArchon.git
```

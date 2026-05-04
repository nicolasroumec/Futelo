# TODO — Sesión 4: Vault + Catálogos

## Server

- [x] DTOs — `CreateVaultRequest`, `UpdateVaultRequest`, `VaultResponse`, `VaultPlayerResponse`, `InviteRequest`, `InvitationResponse`, `TeamResponse`, `VideoGameResponse`
- [x] Vault CRUD — `IVaultRepository` + `VaultRepository` + `IVaultService` + `VaultService` + `VaultController` (GET/POST/PUT/DELETE `/api/vaults`)
- [x] Invitaciones — `IInvitationRepository` + `InvitationRepository` + `IInvitationService` + `InvitationService` + endpoints `POST /api/vaults/{id}/invite`, `POST /api/invitations/{token}/accept`
- [x] Teams — `ITeamRepository` + `TeamRepository` + `ITeamService` + `TeamService` + `TeamController` (GET `/api/teams`)
- [x] VideoGames — `IVideoGameRepository` + `VideoGameRepository` + `IVideoGameService` + `VideoGameService` + `VideoGameController` (GET `/api/videogames`)

## Client

- [ ] Vault — `VaultService` (HTTP calls) + `Dashboard.razor` + `VaultDetail.razor` + `CreateVault.razor`
- [ ] Teams — `TeamService` + `Teams.razor`
- [ ] VideoGames — `VideoGameService` + `Games.razor`

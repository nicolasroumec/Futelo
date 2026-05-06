# TODO — Sesión 5: Roles en Vault + Invitaciones

## Shared
- [x] Enum `VaultRole` (Admin, Editor, Viewer)
- [x] `VaultPlayerResponse` — campo `Role`
- [x] `InviteRequest` — campo `Role`

## Server
- [x] `VaultPlayer` — campo `Role`, default `Viewer`
- [x] `VaultInvitation` — campo `Role`
- [x] Migración `AddVaultRoles`
- [x] `VaultService.CreateAsync` — owner se agrega con `Admin`
- [x] `VaultService.UpdateAsync` — requiere rol `Admin`
- [x] `VaultService.DeleteAsync` — requiere ser owner
- [x] `InvitationService.InviteAsync` — requiere rol `Admin`; guarda `Role` en la invitación
- [x] `InvitationService.AcceptAsync` — asigna el rol de la invitación al nuevo `VaultPlayer`

## Client
- [x] `IInvitationService` + `InvitationService` — POST `/api/invitations/{token}/accept`
- [x] `AcceptInvitation.razor` — ruta `/invitations/{token}/accept`
- [x] `VaultDetail` — badges de rol en lista de miembros
- [x] `VaultDetail` — selector de rol en form de invitación (visible solo para Admin)
- [x] Mostrar el link de invitación en la app tras invitar (sin email, copiable para compartir)

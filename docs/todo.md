# TODO — Sesión 5: Temporadas

## Server

- [x] DTOs — `CreateSeasonRequest`, `ConfigureSeasonRequest`, `SeasonResponse`, `SeasonPlayerResponse`
- [x] Seasons — `ISeasonRepository` + `SeasonRepository` + `ISeasonService` + `SeasonService` + `SeasonController` (GET `/api/seasons?vaultId=`, GET `/api/seasons/{id}`, POST `/api/seasons`, PUT `/api/seasons/{id}/configure`, PUT `/api/seasons/{id}/activate`)

## Client

- [x] Season — `ISeasonService` + `SeasonService` + `CreateSeason.razor` + `SeasonDetail.razor` (configure + Start Season)
- [x] VaultDetail — muestra lista de seasons + botón "New Season"

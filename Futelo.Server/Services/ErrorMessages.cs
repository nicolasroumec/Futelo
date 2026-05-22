namespace Futelo.Server.Services;

internal static class ErrorMessages
{
    // Auth
    internal const string EmailAlreadyInUse = "Email already in use.";
    internal const string UsernameAlreadyInUse = "Username already in use.";
    internal const string InvalidCredentials = "Invalid credentials.";

    // Vault
    internal const string VaultNotFound = "Vault not found.";
    internal const string OnlyAdminsCanInvite = "Only vault admins can invite players.";
    internal const string OnlyAdminsCanUpdate = "Only vault admins can update the vault.";
    internal const string OnlyOwnerCanDelete = "Only the vault owner can delete the vault.";
    internal const string OnlyAdminsAndEditorsCanEdit = "Only vault admins and editors can record results.";

    // Invitation
    internal const string InvitationNotFound = "Invitation not found.";
    internal const string InvitationNoLongerValid = "This invitation is no longer valid.";
    internal const string InvitationExpired = "This invitation has expired.";
    internal const string AlreadyVaultMember = "You are already a member of this vault.";

    // Season
    internal const string SeasonNotFound = "Season not found.";
    internal const string OnlyOwnerCanCreateSeason = "Only the vault owner can create seasons.";
    internal const string OnlyOwnerCanConfigureSeason = "Only the vault owner can configure seasons.";
    internal const string OnlyOwnerCanFinishSeason = "Only the vault owner can finish a season.";
    internal const string OnlyOwnerCanDeleteSeason = "Only the vault owner can delete seasons.";
    internal const string OnlyOwnerCanActivateSeason = "Only the vault owner can activate a season.";
    internal const string OnlyOwnerCanUpdateSeason = "Only the vault owner can update the season.";
    internal const string OnlyOwnerCanAssignTeams = "Only the vault owner can assign teams.";
    internal const string SuperCupRequiresLeagueAndCup = "SuperCup requires both League and Cup.";
    internal const string AllPlayersMustBelongToVault = "All players must belong to the vault.";
    internal const string AtLeast2PlayersRequired = "At least 2 players are required.";
    internal const string OnlyActiveSeasonCanBeFinished = "Only Active seasons can be finished.";
    internal const string OnlyDraftSeasonCanBeActivated = "Only Draft seasons can be activated.";
    internal const string SeasonMustHavePlayers = "Season must have players before activation.";
    internal const string SeasonMustHaveCompetition = "Season must have at least one competition before activation.";
    internal const string PlayerNotFoundInSeason = "Player not found in this season.";

    // League
    internal const string LeagueNotFound = "League not found.";
    internal const string FixtureAlreadyGenerated = "Fixture has already been generated.";
    internal const string CannotRegenerateAfterResults = "Cannot regenerate fixture after results have been recorded.";
    internal const string LeagueNotActive = "League is not active.";

    // Cup
    internal const string CupNotFound = "Cup not found.";
    internal const string CupRoundNotFound = "Cup round not found.";
    internal const string BracketAlreadyGenerated = "Bracket has already been generated.";
    internal const string CupNotActive = "Cup is not active.";
    internal const string MatchNotFound = "Match not found.";
    internal const string MatchAlreadyHasResult = "Match already has a result.";
    internal const string MatchParticipantsNotDetermined = "Match participants are not yet determined.";
    internal const string ScoresCannotBeNegative = "Scores cannot be negative.";
    internal const string MatchIsDrawnNeedsPenalties = "Match is drawn — provide the winner on penalties.";
    internal const string TieOnAggregateMNeedsPenalties = "Tie is level on aggregate — provide the winner on penalties.";

    // SuperCup
    internal const string SuperCupNotFound = "SuperCup not found.";
    internal const string SuperCupAlreadyStarted = "SuperCup has already been started.";
    internal const string LeagueMustBeFinishedFirst = "League must be finished before starting the SuperCup.";
    internal const string CupMustBeFinishedFirst = "Cup must be finished before starting the SuperCup.";
    internal const string CouldNotDetermineRunnerUp = "Could not determine Cup runner-up for SuperCup.";
    internal const string SuperCupNotActive = "SuperCup is not active.";

    // Stats / Players
    internal const string PlayerNotFound = "Player not found.";
}

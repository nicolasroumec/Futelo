using Futelo.Shared;

namespace Futelo.Client.Helpers;

public static class BadgeHelper
{
    public static string StatusBadgeClass(string? status) => status switch
    {
        CompetitionStatus.Active                                  => "bg-success",
        CompetitionStatus.Finished                                => "bg-secondary",
        CompetitionStatus.Draft or CompetitionStatus.NotStarted  => "bg-warning text-dark",
        _ => "bg-secondary"
    };
}

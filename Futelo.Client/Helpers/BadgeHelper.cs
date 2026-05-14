namespace Futelo.Client.Helpers;

public static class BadgeHelper
{
    public static string StatusBadgeClass(string? status) => status switch
    {
        "Active" => "bg-success",
        "Finished" => "bg-secondary",
        "Draft" or "NotStarted" => "bg-warning text-dark",
        _ => "bg-secondary"
    };
}

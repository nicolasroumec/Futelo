namespace Futelo.Client.Services.Toast;

public record ToastMessage(string Text, ToastType Type, int DurationMs);

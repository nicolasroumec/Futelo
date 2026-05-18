namespace Futelo.Client.Services.Toast;

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;

    public void Show(string message, ToastType type = ToastType.Success, int durationMs = 3000)
        => OnShow?.Invoke(new ToastMessage(message, type, durationMs));
}

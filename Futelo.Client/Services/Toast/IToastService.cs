namespace Futelo.Client.Services.Toast;

public interface IToastService
{
    event Action<ToastMessage> OnShow;
    void Show(string message, ToastType type = ToastType.Success, int durationMs = 3000);
}

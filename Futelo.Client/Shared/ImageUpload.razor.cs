using Futelo.Client.Services.Media;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Futelo.Client.Shared;

public partial class ImageUpload : LocalizedComponentBase
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    [Inject] private MediaUrlService Media { get; set; } = null!;

    [Parameter] public int Width { get; set; } = 128;
    [Parameter] public int Height { get; set; } = 128;
    [Parameter] public int DisplaySize { get; set; } = 56;
    [Parameter] public string? ExistingImageUrl { get; set; }
    [Parameter] public EventCallback<byte[]> OnImageReady { get; set; }

    // Data URL of a freshly picked file, shown for immediate feedback. While set
    // it takes priority over ExistingImageUrl; it is cleared whenever the parent
    // changes ExistingImageUrl (e.g. after a successful upload/delete).
    private string? localPreview;
    private bool existingFailed;
    private bool existingLoaded;
    private string? errorMessage;
    private bool isProcessing;
    private string? _lastExistingUrl;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (_lastExistingUrl != ExistingImageUrl)
        {
            _lastExistingUrl = ExistingImageUrl;
            existingFailed = false;
            existingLoaded = false;
            localPreview = null;
        }
    }

    private void OnExistingLoad() => existingLoaded = true;

    // Ignore error events that arrive after a successful load: Blazor can fire a
    // spurious 'error' on the <img> during rapid re-renders, which would
    // otherwise hide an avatar that actually loaded fine.
    private void OnExistingError()
    {
        if (!existingLoaded) existingFailed = true;
    }

    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        errorMessage = null;
        var file = e.File;

        if (file.Size > 5_000_000)
        {
            errorMessage = Lang.Get("avatar.tooLarge");
            return;
        }

        isProcessing = true;
        StateHasChanged();

        try
        {
            using var stream = file.OpenReadStream(5_000_000);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var dataUrl = $"data:{file.ContentType};base64,{base64}";

            var resizedDataUrl = await JS.InvokeAsync<string>("resizeImageToWebP", dataUrl, Width, Height);

            localPreview = resizedDataUrl;
            var resizedBytes = Convert.FromBase64String(resizedDataUrl.Split(',')[1]);
            await OnImageReady.InvokeAsync(resizedBytes);
        }
        catch
        {
            errorMessage = Lang.Get("common.error");
        }
        finally
        {
            isProcessing = false;
        }
    }
}

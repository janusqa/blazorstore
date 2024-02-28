using Microsoft.JSInterop;

namespace BlazorStore.Extensions
{
    public static class IJSRuntimeExtension
    {
        public static async ValueTask ToastrSuccess(this IJSRuntime ijsr, string message)
        {
            await ijsr.InvokeVoidAsync("blazorInterop.ShowToastr", "success", message);
        }

        public static async ValueTask ToastrFailure(this IJSRuntime ijsr, string message)
        {
            await ijsr.InvokeVoidAsync("blazorInterop.ShowToastr", "error", message);
        }
    }
}

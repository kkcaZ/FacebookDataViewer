using Microsoft.JSInterop;

namespace FacebookDataViewer.Services;

// Copied from google :)

public class ScrollInfoService : IScrollInfoService
{
    public event EventHandler<ScrollEventArgs> OnScroll; 

    public ScrollInfoService(IJSRuntime jsRuntime)
    {
        RegisterServiceViaJsRuntime(jsRuntime);
    }

    private void RegisterServiceViaJsRuntime(IJSRuntime jsRuntime)
    {
        jsRuntime.InvokeVoidAsync("RegisterScrollInfoService", DotNetObjectReference.Create(this));
    }

    public int YValue { get; private set; }

    [JSInvokable("OnScroll")]
    public void JsOnScroll(int yValue, int scrollHeight)
    {
        YValue = yValue;
        OnScroll?.Invoke(this, new ScrollEventArgs() { ScrollHeight = scrollHeight, ScrollTop = yValue });
    }
}

public interface IScrollInfoService
{
    event EventHandler<ScrollEventArgs> OnScroll;
    int YValue { get; }
}

public class ScrollEventArgs : EventArgs
{
    public int ScrollTop { get; set; }
    
    public int ScrollHeight { get; set; }
}
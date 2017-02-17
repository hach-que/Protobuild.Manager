#if PLATFORM_LINUX
using System.Runtime.InteropServices;
using System;
using Gtk;
using System.Text;

namespace WebKit2
{
    public enum LoadEvent
    {
        LoadStarted,
        LoadRedirected,
        LoadCommitted,
        LoadFinished
    }

    public enum PolicyDecisionsType
    {
        NavigationAction,
        NewWindowAction,
        Response
    }

    public static class WebViewWrapper
    {
        public const string webkitpath = "libwebkit2gtk-4.0.so";

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webkit_web_view_new();

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_run_javascript(IntPtr web_view, string script, IntPtr cancellable, IntPtr callback, IntPtr user_data);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_load_uri(IntPtr web_view, string uri);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webkit_web_view_get_uri(IntPtr web_view);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webkit_navigation_policy_decision_get_request(IntPtr decision);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webkit_uri_request_get_uri(IntPtr request);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_stop_loading(IntPtr webView);

        public static unsafe string GetString(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return "";

            var ptr = (byte*)handle;
            while (*ptr != 0)
                ptr++;

            var bytes = new byte[ptr - (byte*)handle];
            Marshal.Copy(handle, bytes, 0, bytes.Length);

            return Encoding.UTF8.GetString(bytes);
        }
    }

    public class LoadArgs : GLib.SignalArgs
    {
        public LoadEvent LoadEvent
        {
            get
            {
                return (LoadEvent)Args[0];
            }
        }
    }

    public class DecideArgs : GLib.SignalArgs
    {
        public GLib.Object Decision
        {
            get
            {
                return (GLib.Object)Args[0];
            }
        }

        public PolicyDecisionsType Type
        {
            get
            {
                return (PolicyDecisionsType)Args[1];
            }
        }
    }

    public class WebView : Container
    {
        public delegate void DecidePolicyHandler(object o, DecideArgs args);
        public event DecidePolicyHandler DecidePolicy
        {
            add
            {
                this.AddSignalHandler("decide-policy", value, typeof(DecideArgs));
            }
            remove
            {
                this.RemoveSignalHandler("decide-policy", value);
            }
        }

        public delegate void LoadChangedHandler(object o, LoadArgs args);
        public event LoadChangedHandler LoadChanged
        {
            add
            {
                this.AddSignalHandler("load-changed", value, typeof(LoadArgs));
            }
            remove
            {
                this.RemoveSignalHandler("load-changed", value);
            }
        }

        public string Uri
        {
            get
            {
                return WebViewWrapper.GetString(WebViewWrapper.webkit_web_view_get_uri(Handle));
            }
        }

        public WebView() : base(WebViewWrapper.webkit_web_view_new())
        {
			
        }

        public void ExecuteScript(string script)
        {
            WebViewWrapper.webkit_web_view_run_javascript(this.Handle, script, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        public void LoadUri(string uri)
        {
            WebViewWrapper.webkit_web_view_load_uri(Handle, uri);
        }

        public void StopLoading()
        {
            WebViewWrapper.webkit_web_view_stop_loading(Handle);
        }
    }
}
#endif

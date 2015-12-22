#if PLATFORM_LINUX
using System.Runtime.InteropServices;
using System;
using Gtk;

namespace WebKit
{
    public static class WebViewWrapper
    {
        public const string webkitpath = "libwebkitgtk-3.0.so.0";

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webkit_web_view_new();

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool webkit_web_view_get_transparent(IntPtr webView);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_set_transparent(IntPtr webView, bool transparent);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_execute_script(IntPtr webView, string script);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_load_uri(IntPtr webView, string uri);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern string webkit_web_resource_get_uri(IntPtr webResource);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern string webkit_network_request_get_uri(IntPtr networkRequest);

        [DllImport(webkitpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webkit_web_view_stop_loading(IntPtr webView);
    }

    public class LoadArgs : GLib.SignalArgs
    {
        public object Frame
        {
            get
            {
                return Args[0];
            }
        }
    }

    public class LoadProgressChangedArgs : GLib.SignalArgs
    {
        public int Progress
        {
            get
            {
                return (int)Args[0];
            }
        }
    }

    public class NavigationRequestedArgs : GLib.SignalArgs
    {
        public GLib.Object Request
        {
            get
            {
                return (GLib.Object)Args[1];
            }
        }
    }

    public class WebView : Container
    {
        public bool Transparent
        {
            get
            {
                return WebViewWrapper.webkit_web_view_get_transparent(this.Handle);
            }
            set
            {
                WebViewWrapper.webkit_web_view_set_transparent(this.Handle, value);
            }
        }

        public delegate void LoadCommittedHandler(object o, LoadArgs args);
        public event LoadCommittedHandler LoadCommitted
        {
            add
            {
                this.AddSignalHandler("load-committed", value, typeof(LoadArgs));
            }
            remove
            {
                this.RemoveSignalHandler("load-committed", value);
            }
        }

        public delegate void LoadFinishedHandler(object o, LoadArgs args);
        public event LoadFinishedHandler LoadFinished
        {
            add
            {
                this.AddSignalHandler("load-finished", value, typeof(LoadArgs));
            }
            remove
            {
                this.RemoveSignalHandler("load-finished", value);
            }
        }

        public delegate void LoadProgressChangedHandler(object o, LoadProgressChangedArgs args);
        public event LoadProgressChangedHandler LoadProgressChanged
        {
            add
            {
                this.AddSignalHandler("load-progress-changed", value, typeof(LoadProgressChangedArgs));
            }
            remove
            {
                this.RemoveSignalHandler("load-progress-changed", value);
            }
        }

        public delegate void LoadStartedHandler(object o, LoadArgs args);
        public event LoadStartedHandler LoadStarted
        {
            add
            {
                this.AddSignalHandler("load-started", value, typeof(LoadArgs));
            }
            remove
            {
                this.RemoveSignalHandler("load-started", value);
            }
        }

        public delegate void NavigationRequestedHandler(object o, NavigationRequestedArgs args);
        public event NavigationRequestedHandler NavigationRequested
        {
            add
            {
                this.AddSignalHandler("navigation-requested", value, typeof(NavigationRequestedArgs));
            }
            remove
            {
                this.RemoveSignalHandler("navigation-requested", value);
            }
        }

        public WebView()
            : base(WebViewWrapper.webkit_web_view_new())
        {
			
        }

        public void ExecuteScript(string script)
        {
            WebViewWrapper.webkit_web_view_execute_script(this.Handle, script);
        }

        public void LoadUri(string uri)
        {
            WebViewWrapper.webkit_web_view_load_uri(this.Handle, uri);
        }

        public void StopLoading()
        {
            WebViewWrapper.webkit_web_view_stop_loading(this.Handle);
        }
    }
}
#endif

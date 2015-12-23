#if PLATFORM_LINUX
using System;
using System.Diagnostics;
using System.Web;
using Gtk;
using WebKit;
using System.IO;
using System.Linq;

namespace Protobuild.Manager
{
    public delegate void SignalDelegate(object obj, GLib.SignalArgs args);

    public static class Program
    {
        public static string[] embeded_dlls =
            {
                "atk-sharp.dll",
                "gdk-sharp.dll",
                "gio-sharp.dll",
                "glib-sharp.dll",
                "gtk-sharp.dll"
            };

        public static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                GLib.ExceptionManager.UnhandledException += (e) => 
                {
                    Console.Error.WriteLine(e.ExceptionObject);
                    Debugger.Break();
                };

                Run(args);
            }
            else
            {
                AppDomain.CurrentDomain.UnhandledException +=
                    (sender, e) => Console.Error.WriteLine((Exception)e.ExceptionObject);

                GLib.ExceptionManager.UnhandledException += (e) => 
                {
                    Console.Error.WriteLine((Exception)e.ExceptionObject);
                    e.ExitApplication = true;
                };

                try
                {
                    Run(args);
                }
                catch (Exception e)
                { 
                    // we don't want an infinite loop
                    if (args.ToList().Contains("--internalbreak"))
                        return;
                    
                    if (e is FileNotFoundException || e is TypeInitializationException)
                    {
                        foreach (var ed in embeded_dlls)
                        {
                            WriteResource(ed, true);
                            WriteResource(ed + ".config", true);

                            // TODO Include native .so libraries and extract the ones 
                            // needed for the current PC architecture
                        }

                        if (!ReRun(args))
                            Console.Error.WriteLine(e);
                    }
                    else
                        Console.Error.WriteLine(e);
                }
            }
        }

        private static void WriteResource(string resource, bool overide)
        {
            try
            {
                var res = typeof(Program).Assembly.GetManifestResourceStream(resource);
                var path_to_write = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resource);

                if(overide && File.Exists(path_to_write))
                    File.Delete(path_to_write);
                else if(!overide && File.Exists(path_to_write))
                    return;
                
                using (var writer = new FileStream(path_to_write, FileMode.Create, FileAccess.Write))
                    res.CopyTo(writer);
            }
            catch
            {
            }
        }

        public static void ShowRequiredLibrary(string[] librarySuggestions)
        {
            Application.Init();
            var window = new Window("Missing Library!");
            //window.SetSizeRequest(300, 150);
            window.Destroyed += delegate (object sender, EventArgs e)
            {
                Application.Quit();
            };
            var label = new Label("You are missing a required library to run this application.  Please " +
                "install the required library using the command appropriate " +
                "for your Linux distribution:\n" +
                "\n" +
                librarySuggestions.Select(a => "* " + a + "\n").Aggregate((a, b) => a + b) +
                "\n" +
                "Once you have installed this library, try running the application again.");
            label.SetPadding(20, 20);

            window.Add(label);
            window.WindowPosition = WindowPosition.Center;
            //window.AllowGrow = false;
            //window.AllowShrink = false;
            window.ShowAll();
            Application.Run();
        }

        public static void Run(string[] args)
        {
            var kernel = new LightweightKernel();
            kernel.BindCommon();
            kernel.BindAndKeepInstance<IUIManager, LinuxUIManager>();
            kernel.BindAndKeepInstance<IExecution, LinuxExecution>();
            kernel.BindAndKeepInstance<IIDEControl, MonoDevelopLinuxIDEControl>();
            kernel.BindAndKeepInstance<IIDEProjectTemplateSync, MonoDevelopProjectTemplateSync>();
            kernel.BindAndKeepInstance<IIDEAddinInstall, LinuxMonoDevelopAddinInstall>();

            kernel.Get<IErrorLog>().Log("Started game launcher on Linux platform");

            var startup = kernel.Get<IStartup>();
            startup.Start(args);
        }

        public static bool ReRun(string[] args)
        {
            var process = Process.Start("mono", "\"" + typeof(Program).Assembly.Location + "\" --internalbreak " + string.Join(" ", args));
            process.WaitForExit();

            return process.ExitCode == 0;
        }
    }
}
#endif


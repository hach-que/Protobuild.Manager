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
                catch (TypeInitializationException e)
                {
                    if (e.InnerException != null && e.InnerException.Message.Contains("libwebkitgtk-1.0.so.0"))
                    {
                        ShowRequiredLibrary(new[] {
                            "zypper in libwebkitgtk-1__0-0"
                        });
                    }
                    else if (e.InnerException != null && e.InnerException.Message.Contains("webkit-1.0"))
                    {
                        // Try and extract webkit-sharp next to the assembly.
                        try
                        {
                            var assembly = typeof(Program).Assembly.GetManifestResourceStream("webkit-sharp.dll");
                            var assemblyConfig = typeof(Program).Assembly.GetManifestResourceStream("webkit-sharp.dll.config");

                            using (var writer = new FileStream(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll"), FileMode.Create, FileAccess.Write))
                            {
                                assembly.CopyTo(writer);
                            }
                            using (var writer = new FileStream(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll.config"), FileMode.Create, FileAccess.Write))
                            {
                                assemblyConfig.CopyTo(writer);
                            }

                            // We have to relaunch the executable to pick up the new libraries next to us.
                            var process = System.Diagnostics.Process.Start("mono", "\"" + typeof(Program).Assembly.Location + "\" " + string.Join(" ", args));
                            process.WaitForExit();

                            if (process.ExitCode == 1)
                            {
                                File.Delete(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll"));
                                File.Delete(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll.config"));
                                ShowRequiredLibrary(new[] {
                                    "zypper in webkit-sharp"
                                });
                            }

                            Environment.Exit(process.ExitCode);
                        }
                        catch
                        {
                            ShowRequiredLibrary(new[] {
                                "zypper in webkit-sharp"
                            });
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine(e);
                    }
                }
                catch (System.IO.FileNotFoundException e)
                {
                    if (e.Message.Contains("webkit-sharp"))
                    {
                        // Try and extract webkit-sharp next to the assembly.
                        try
                        {
                            var assembly = typeof(Program).Assembly.GetManifestResourceStream("webkit-sharp.dll");
                            var assemblyConfig = typeof(Program).Assembly.GetManifestResourceStream("webkit-sharp.dll.config");

                            using (var writer = new FileStream(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll"), FileMode.Create, FileAccess.Write))
                            {
                                assembly.CopyTo(writer);
                            }
                            using (var writer = new FileStream(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll.config"), FileMode.Create, FileAccess.Write))
                            {
                                assemblyConfig.CopyTo(writer);
                            }

                            // We have to relaunch the executable to pick up the new libraries next to us.
                            var process = System.Diagnostics.Process.Start("mono", "\"" + typeof(Program).Assembly.Location + "\" " + string.Join(" ", args));
                            process.WaitForExit();

                            if (process.ExitCode == 1)
                            {
                                File.Delete(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll"));
                                File.Delete(Path.Combine(new FileInfo(typeof(Program).Assembly.Location).DirectoryName, "webkit-sharp.dll.config"));
                                ShowRequiredLibrary(new[] {
                                    "zypper in webkit-sharp"
                                });
                            }

                            Environment.Exit(process.ExitCode);
                        }
                        catch
                        {
                            ShowRequiredLibrary(new[] {
                                "zypper in webkit-sharp"
                            });
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine(e);
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
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
    }
}
#endif


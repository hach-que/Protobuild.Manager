﻿#if PLATFORM_MACOS
namespace Protobuild.Manager
{
    using System;
    using System.Diagnostics;
    using AppKit;

    public static class Program
    {
        public static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Run(args);
            }
            else
            {
                //AppDomain.CurrentDomain.UnhandledException +=
                //    (sender, e) => CrashReporter.Record((Exception)e.ExceptionObject);

                try
                {
                    Run(args);
                }
                catch (Exception e)
                {
                    //CrashReporter.Record(e);

                    Environment.Exit(1);
                }
            }
        }

        public static void Run(string[] args)
		{
			var kernel = new LightweightKernel();
			kernel.BindCommon();
			kernel.BindAndKeepInstance<IUIManager, MacOSUIManager>();
			kernel.BindAndKeepInstance<IExecution, MacOSExecution>();

			kernel.Get<IErrorLog>().Log("Started game launcher on Mac platform");

			var startup = kernel.Get<IStartup>();
			startup.Start();
        }
    }
}
#endif


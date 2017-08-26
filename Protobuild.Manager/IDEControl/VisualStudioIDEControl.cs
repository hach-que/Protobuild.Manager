using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace Protobuild.Manager
{
    internal class VisualStudioIDEControl : IIDEControl
    {
        private readonly RuntimeServer _runtimeServer;
        private readonly IProcessLog _processLog;
        private readonly IExecution _execution;
        private readonly IProtobuildProvider _protobuildProvider;

        public VisualStudioIDEControl(
            RuntimeServer runtimeServer,
            IProcessLog processLog,
            IExecution execution,
            IProtobuildProvider protobuildProvider)
        {
            _runtimeServer = runtimeServer;
            _processLog = processLog;
            _execution = execution;
            _protobuildProvider = protobuildProvider;
        }

        public async Task LoadSolution(string modulePath, string moduleName, string targetPlatform, string oldPlatformOnFail, bool isProtobuild)
        {
            try
            {
                _runtimeServer.Set("busy", true);
                _runtimeServer.Set("statusMode", "Processing");
                _runtimeServer.Set("status", "Searching for open Visual Studio instances...");

                var vspid = _runtimeServer.Get<int?>("visualstudiopid");

                Func<dynamic, Task> launchLogic = null;

                var existing = FindExistingVisualStudioInstance(modulePath, moduleName, vspid);
                if (existing == null)
                {
                    launchLogic = async dteRef =>
                    {
                        existing = await LaunchVisualStudio(modulePath, moduleName, targetPlatform, vspid, existing);
                    };
                }
                else
                {
                    launchLogic = async dteRef =>
                    {
                        _runtimeServer.Set("status", "Opening solution for " + targetPlatform + "...");

                        dteRef.Solution.Open(Path.Combine(modulePath, moduleName + "." + targetPlatform + ".sln"));
                        dteRef.ActiveWindow.Activate();

                        await Task.Yield();
                    };
                }

                var protobuild = Path.Combine(modulePath, "Protobuild.exe");

                dynamic dte = null;
                if (existing != null)
                {
                    try
                    {
                        dte = existing.DTE;
                    }
                    catch (COMException ex)
                    {
                        unchecked
                        {
                            if (ex.HResult == (int) 0x8001010A)
                            {
                                _runtimeServer.Set("statusMode", "Error");
                                _runtimeServer.Set("status",
                                    "Visual Studio is currently busy; can't switch platforms right now!");
                                _runtimeServer.Set("setplatform", oldPlatformOnFail);
                                _runtimeServer.Set("setplatform", null);
                                return;
                            }
                        }

                        throw;
                    }

                    if (dte.Solution.IsOpen)
                    {
                        var oldName = new FileInfo(dte.Solution.FullName).Name;
                        oldName = oldName.Substring(0, oldName.LastIndexOf('.'));
                        var oldPlatform = oldName.Substring(oldName.LastIndexOf('.') + 1);

                        _runtimeServer.Set("status", "Found Visual Studio, saving all files before solution close...");
                        dte.ExecuteCommand("File.SaveAll");
                        dte.Solution.Close(true);

                        if (isProtobuild)
                        {
                            _runtimeServer.Set("status", "Synchronising for " + oldPlatform + " platform...");
                            var syncProcess = _execution.ExecuteConsoleExecutable(protobuild, "--sync " + oldPlatform, x =>
                            {
                                x.WorkingDirectory = modulePath;
                                x.UseShellExecute = false;
                                x.CreateNoWindow = true;
                                x.RedirectStandardOutput = true;
                                x.RedirectStandardError = true;
                            },
                            _processLog.PrepareForAttachToProcess);
                            _processLog.AttachToProcess(syncProcess);
                            await syncProcess.WaitForExitAsync();
                        }
                    }
                }

                if (isProtobuild)
                {
                    _runtimeServer.Set("status", "Generating for " + targetPlatform + " platform...");
                    var process = _execution.ExecuteConsoleExecutable(protobuild, "--generate " + targetPlatform, x =>
                    {
                        x.WorkingDirectory = modulePath;
                        x.UseShellExecute = false;
                        x.CreateNoWindow = true;
                        x.RedirectStandardOutput = true;
                        x.RedirectStandardError = true;
                    },
                    _processLog.PrepareForAttachToProcess);
                    _processLog.AttachToProcess(process);
                    process.WaitForExit();
                }

                await launchLogic(dte);

                _runtimeServer.Set("statusMode", "Okay");
                _runtimeServer.Set("status", "Platforms switched successfully.");
            }
            catch (Exception exx)
            {
                _runtimeServer.Set("statusMode", "Error");
                _runtimeServer.Set("status", exx.ToString());
                _runtimeServer.Set("setplatform", oldPlatformOnFail);
                _runtimeServer.Set("setplatform", null);
            }
            finally
            {
                _runtimeServer.Set("busy", false);
            }
        }

        public async Task SaveAndSyncSolution(string modulePath, string moduleName, string targetPlatform, string oldPlatformOnFail,
            bool isProtobuild)
        {
            try
            {
                _runtimeServer.Set("busy", true);
                _runtimeServer.Set("statusMode", "Processing");
                _runtimeServer.Set("status", "Searching for open Visual Studio instances...");

                var vspid = _runtimeServer.Get<int?>("visualstudiopid");

                var existing = FindExistingVisualStudioInstance(modulePath, moduleName, vspid);
                if (existing == null)
                {
                    return;
                }

                var protobuild = Path.Combine(modulePath, "Protobuild.exe");

                dynamic dte = null;
                try
                {
                    dte = existing.DTE;
                }
                catch (COMException ex)
                {
                    unchecked
                    {
                        if (ex.HResult == (int)0x8001010A)
                        {
                            _runtimeServer.Set("statusMode", "Error");
                            _runtimeServer.Set("status",
                                "Visual Studio is currently busy; can't save all projects right now!");
                            return;
                        }
                    }

                    throw;
                }

                if (dte.Solution.IsOpen)
                {
                    var oldName = new FileInfo(dte.Solution.FullName).Name;
                    oldName = oldName.Substring(0, oldName.LastIndexOf('.'));
                    var oldPlatform = oldName.Substring(oldName.LastIndexOf('.') + 1);

                    _runtimeServer.Set("status", "Found Visual Studio, saving all files before synchronisation...");
                    dte.ExecuteCommand("File.SaveAll");

                    if (isProtobuild)
                    {
                        _runtimeServer.Set("status", "Synchronising for " + oldPlatform + " platform...");
                        var syncProcess = _execution.ExecuteConsoleExecutable(protobuild, "--sync " + oldPlatform, x =>
                        {
                            x.WorkingDirectory = modulePath;
                            x.UseShellExecute = false;
                            x.CreateNoWindow = true;
                            x.RedirectStandardOutput = true;
                            x.RedirectStandardError = true;
                        },
                        _processLog.PrepareForAttachToProcess);
                        _processLog.AttachToProcess(syncProcess);
                        await syncProcess.WaitForExitAsync();
                    }
                }

                _runtimeServer.Set("statusMode", "Okay");
                _runtimeServer.Set("status", "Synchronised projects successfully.");
            }
            catch (Exception exx)
            {
                _runtimeServer.Set("statusMode", "Error");
                _runtimeServer.Set("status", exx.ToString());
            }
            finally
            {
                _runtimeServer.Set("busy", false);
            }
        }

        public async Task CloseGenerateAndLoadSolution(string modulePath, string moduleName, string targetPlatform, string oldPlatformOnFail,
            bool isProtobuild)
        {
            try
            {
                _runtimeServer.Set("busy", true);
                _runtimeServer.Set("statusMode", "Processing");
                _runtimeServer.Set("status", "Searching for open Visual Studio instances...");

                var vspid = _runtimeServer.Get<int?>("visualstudiopid");

                Func<dynamic, Task> launchLogic = null;

                var existing = FindExistingVisualStudioInstance(modulePath, moduleName, vspid);
                if (existing == null)
                {
                    launchLogic = async dteRef =>
                    {
                        existing = await LaunchVisualStudio(modulePath, moduleName, targetPlatform, vspid, existing);
                    };
                }
                else
                {
                    launchLogic = async dteRef =>
                    {
                        _runtimeServer.Set("status", "Opening solution for " + targetPlatform + "...");

                        dteRef.Solution.Open(Path.Combine(modulePath, moduleName + "." + targetPlatform + ".sln"));
                        dteRef.ActiveWindow.Activate();

                        await Task.Yield();
                    };
                }

                var protobuild = Path.Combine(modulePath, "Protobuild.exe");

                dynamic dte = null;
                if (existing != null)
                {
                    try
                    {
                        dte = existing.DTE;
                    }
                    catch (COMException ex)
                    {
                        unchecked
                        {
                            if (ex.HResult == (int)0x8001010A)
                            {
                                _runtimeServer.Set("statusMode", "Error");
                                _runtimeServer.Set("status",
                                    "Visual Studio is currently busy; can't switch platforms right now!");
                                _runtimeServer.Set("setplatform", oldPlatformOnFail);
                                _runtimeServer.Set("setplatform", null);
                                return;
                            }
                        }

                        throw;
                    }

                    if (dte.Solution.IsOpen)
                    {
                        _runtimeServer.Set("status", "Found Visual Studio, saving all files before solution close...");
                        dte.ExecuteCommand("File.SaveAll");
                        dte.Solution.Close(true);
                    }
                }

                if (isProtobuild)
                {
                    _runtimeServer.Set("status", "Generating for " + targetPlatform + " platform...");
                    var process = _execution.ExecuteConsoleExecutable(protobuild, "--generate " + targetPlatform, x =>
                    {
                        x.WorkingDirectory = modulePath;
                        x.UseShellExecute = false;
                        x.CreateNoWindow = true;
                        x.RedirectStandardOutput = true;
                        x.RedirectStandardError = true;
                    },
                    _processLog.PrepareForAttachToProcess);
                    _processLog.AttachToProcess(process);
                    process.WaitForExit();
                }

                await launchLogic(dte);

                _runtimeServer.Set("statusMode", "Okay");
                _runtimeServer.Set("status", "Platforms switched successfully.");
            }
            catch (Exception exx)
            {
                _runtimeServer.Set("statusMode", "Error");
                _runtimeServer.Set("status", exx.ToString());
            }
            finally
            {
                _runtimeServer.Set("busy", false);
            }
        }

        private async Task<dynamic> LaunchVisualStudio(string modulePath, string moduleName, string targetPlatform, int? vspid, dynamic existing)
        {
            _runtimeServer.Set("status", "Starting Visual Studio...");
            
            // Newer installs of Visual Studio (like 2017) don't create registry entries for MSBuild, so we have to
            // use a tool called vswhere in order to find MSBuild on these systems.  This call will implicitly install
            // the vswhere package if it's not already installed.
            var protobuild = await _protobuildProvider.GetProtobuild(status =>
            {
                _runtimeServer.Set("status", status);
            });
            List<string> installations = null;
            if (protobuild != null && File.Exists(protobuild))
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = protobuild;
                    processStartInfo.Arguments = "--execute vswhere -products * -requires Microsoft.VisualStudio.Workload.CoreEditor -property installationPath";
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.RedirectStandardOutput = true;
                    processStartInfo.CreateNoWindow = true;
                    var process = Process.Start(processStartInfo);
                    var installationsString = process.StandardOutput.ReadToEnd();
                    installations = installationsString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        // Try to install vswhere (it may not be installed).
                        processStartInfo = new ProcessStartInfo();
                        processStartInfo.FileName = protobuild;
                        processStartInfo.Arguments = "--install Protobuild.vswhere";
                        processStartInfo.UseShellExecute = false;
                        processStartInfo.RedirectStandardOutput = true;
                        processStartInfo.CreateNoWindow = true;
                        process = Process.Start(processStartInfo);
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            // Can't use vswhere.
                        }
                        else
                        {
                            // Install worked, try again.
                            processStartInfo = new ProcessStartInfo();
                            processStartInfo.FileName = protobuild;
                            processStartInfo.Arguments = "--execute vswhere -products * -requires Microsoft.VisualStudio.Workload.CoreEditor -property installationPath";
                            processStartInfo.UseShellExecute = false;
                            processStartInfo.RedirectStandardOutput = true;
                            processStartInfo.CreateNoWindow = true;
                            process = Process.Start(processStartInfo);
                            installationsString = process.StandardOutput.ReadToEnd();
                            installations = installationsString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                            process.WaitForExit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Can't find any installations.
                }
            }

            var started = false;

            if (installations != null)
            {
                // Check if the IDE is present in any of those installation paths.
                foreach (var basePath in installations)
                {
                    var ideLocation = Path.Combine(basePath, "Common7\\IDE\\devenv.exe");
                    if (File.Exists(ideLocation))
                    {
                        Process.Start(
                            ideLocation,
                            "\"" + Path.Combine(modulePath, moduleName + "." + targetPlatform + ".sln") + "\"");
                        started = true;
                        break;
                    }
                }
            }
            
            if (!started)
            {
                var versions = new[] {"14.0", "12.0", "11.0", "10.0"};
                foreach (var version in versions)
                {
                    var idePath = @"C:\Program Files (x86)\Microsoft Visual Studio " + version +
                                @"\Common7\IDE\devenv.exe";
                    if (File.Exists(idePath))
                    {
                        Process.Start(
                            idePath,
                            "\"" + Path.Combine(modulePath, moduleName + "." + targetPlatform + ".sln") + "\"");
                        started = true;
                        break;
                    }
                }
            }

            if (!started)
            {
                _runtimeServer.Set("status", "Unable to find installed version of Visual Studio to start!");
            }
            else
            {
                while (existing == null)
                {
                    _runtimeServer.Set("status", "Waiting for Visual Studio to open...");

                    await Task.Delay(1000);

                    existing = FindExistingVisualStudioInstance(modulePath, moduleName, vspid);
                }
            }

            return existing;
        }

        private dynamic FindExistingVisualStudioInstance(string modulePath, string moduleName, int? forcePid)
        {
            IRunningObjectTable rot;
            IEnumMoniker enumMoniker;
            var retVal = GetRunningObjectTable(0, out rot);

            if (retVal == 0)
            {
                rot.EnumRunning(out enumMoniker);

                var fetched = IntPtr.Zero;
                var moniker = new IMoniker[1];
                while (enumMoniker.Next(1, moniker, fetched) == 0)
                {
                    IBindCtx bindCtx;
                    CreateBindCtx(0, out bindCtx);
                    string displayName;
                    moniker[0].GetDisplayName(bindCtx, null, out displayName);

                    try
                    {
                        if (forcePid != null)
                        {
                            if ((displayName.StartsWith("!VisualStudio.DTE") || displayName.Contains("Express.DTE")) && displayName.EndsWith(":" + forcePid.Value))
                            {
                                // Always use this Visual Studio instance.
                                object obj;
                                rot.GetObject(moniker[0], out obj);
                                return obj;
                            }
                        }
                        else
                        {
                            if (displayName.StartsWith("!VisualStudio.DTE") || displayName.Contains("Express.DTE"))
                            {
                                // See if this Visual Studio instance has no solution open.
                                object obj;
                                rot.GetObject(moniker[0], out obj);
                                dynamic dte = obj;
                                if (!dte.Solution.IsOpen)
                                {
                                    // Re-use an empty instance.
                                    return dte;
                                }
                            }
                            
                            if (modulePath != null &&
                                displayName.ToLowerInvariant()
                                    .StartsWith(Path.Combine(modulePath, moduleName + ".").ToLowerInvariant()) &&
                                displayName.ToLowerInvariant().EndsWith(".sln"))
                            {
                                // Found an open solution for this module.
                                object obj;
                                rot.GetObject(moniker[0], out obj);
                                return obj;
                            }
                        }
                    }
                    catch (COMException ex)
                    {
                        unchecked
                        {
                            if (ex.HResult == (int) 0x8001010A)
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            return null;
        }

        [DllImport("ole32.dll")]
        private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
    }
}
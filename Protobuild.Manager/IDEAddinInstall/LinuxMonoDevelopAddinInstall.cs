﻿using System;
using System.Threading.Tasks;
using System.IO;

namespace Protobuild.Manager
{
    public class LinuxMonoDevelopAddinInstall : IIDEAddinInstall
    {
        private readonly IExecution _execution;
        private readonly IAddinPackageDownload _addinPackageDownload;
        private readonly IBrandingEngine _brandingEngine;

        public LinuxMonoDevelopAddinInstall(
            IExecution execution,
            IAddinPackageDownload addinPackageDownload,
            IBrandingEngine brandingEngine)
        {
            _execution = execution;
            _addinPackageDownload = addinPackageDownload;
            _brandingEngine = brandingEngine;
        }

        public async Task InstallIfNeeded(bool force)
        {
            var mdtool = "/usr/bin/mdtool";

            if (!File.Exists(mdtool))
            {
                if (force)
                {
                    throw new InvalidOperationException("Unable to find mdtool to install IDE add-in.");
                }

                return;
            }

            if (!force)
            {
                var process = _execution.ExecuteConsoleExecutable(
                    mdtool,
                    "setup list",
                    x =>
                    {
                        x.UseShellExecute = false;
                        x.RedirectStandardOutput = true;
                    });
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (output.Contains("Protobuild for MonoDevelop"))
                {
                    // TODO: Probably should upgrade it anyway...
                    return;
                }
            }

            string mpackPath = null;
            try
            {
                var addinPath = await _addinPackageDownload.GetPackageRoot(
                    _brandingEngine.MonoDevelopAddinPackage);
                if (addinPath == null)
                {
                    throw new Exception("Protobuild can't be found!");
                }
                mpackPath = Path.Combine(addinPath, "Protobuild.MonoDevelop.mpack");
            }
            catch
            {
                if (force)
                {
                    throw;
                }
            }

            var installProcess = _execution.ExecuteConsoleExecutable(
                mdtool,
                "setup install \"" + mpackPath + "\"");
            await installProcess.WaitForExitAsync();

            if (force)
            {
                if (installProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException("Non-zero exit code from mdtool when installing extension.");
                }
            }
        }
    }
}


﻿using System;
using System.Reflection;
using System.IO;

namespace Protobuild.Manager
{
    public static class LightweightKernelModule
    {
        public static void BindCommon(this LightweightKernel kernel)
        {
            var assemblyDirPath = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
            var brandingPath = Path.Combine(assemblyDirPath, "Branding.xml");
            if (File.Exists(brandingPath))
            {
                kernel.BindAndKeepInstance<IBrandingEngine, ExternalBrandingEngine>();
            }
            else
            {
                kernel.BindAndKeepInstance<IBrandingEngine, EmbeddedBrandingEngine>();
            }

            kernel.BindAndKeepInstance<IConfigManager, ConfigManager>();
            kernel.BindAndKeepInstance<IErrorLog, ErrorLog>();
            kernel.BindAndKeepInstance<RuntimeServer, RuntimeServer>();
            kernel.BindAndKeepInstance<IWorkflowManager, WorkflowManager>();
            kernel.BindAndKeepInstance<IAppHandlerManager, AppHandlerManager>();
            kernel.BindAndKeepInstance<IProgressRenderer, ProgressRenderer>();
            kernel.BindAndKeepInstance<IWorkflowFactory, WorkflowFactory>();
            //kernel.BindAndKeepInstance<IPathProvider, PathProvider>();
            //kernel.BindAndKeepInstance<INewsLoader, NewsLoader>();
            kernel.BindAndKeepInstance<ILauncherSelfUpdate, LauncherSelfUpdate>();
            kernel.BindAndKeepInstance<IStartup, Startup>();
            //kernel.BindAndKeepInstance<IOfflineDetection, OfflineDetection>();
            kernel.BindAndKeepInstance<IProtobuildHostingEngine, ProtobuildHostingEngine>();

            kernel.BindAndKeepInstance<IRecentProjectsManager, RecentProjectsManager>();

            var branding = kernel.Get<IBrandingEngine>();
            if (branding.TemplateSource == "builtin")
            {
                kernel.BindAndKeepInstance<ITemplateSource, BuiltinTemplateSource>();
            }
            else if (branding.TemplateSource == "online")
            {
                throw new NotSupportedException();
            }
            else if (branding.TemplateSource.StartsWith("dir:"))
            {
                kernel.BindAndKeepInstance<ITemplateSource, OnDiskTemplateSource>();
            }
        }
    }
}
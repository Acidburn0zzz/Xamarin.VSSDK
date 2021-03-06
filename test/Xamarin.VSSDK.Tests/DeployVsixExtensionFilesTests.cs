﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Xamarin.VSSDK.Tests
{
    public class DeployVsixExtensionFilesTests : VsTest
    {
        ITestOutputHelper output;

        public DeployVsixExtensionFilesTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void ExtensionFilesAreDeployedWhenBuidingExtension()
        {
            var vsixDeploymentPath = GetVsixDeploymentPath();
            if (!string.IsNullOrEmpty(vsixDeploymentPath) && Directory.Exists(vsixDeploymentPath))
                Directory.Delete(vsixDeploymentPath, true);

            var project = new ProjectInstance("VsixTemplate.csproj", new Dictionary<string, string>
            {
                { "TargetFramework", TargetFramework },
                { "GeneratePkgDefFile", "true"},
                { "Configuration", ThisAssembly.Project.Properties.Configuration },
                { "VSSDKTargetPlatformRegRootSuffix", RootSuffix },
            }, "15.0", new ProjectCollection());

            var result = Builder.Build(project, "Restore;Build", output: output);

            Assert.Equal(BuildResultCode.Success, result.BuildResult.OverallResult);

            vsixDeploymentPath = GetVsixDeploymentPath();

            Assert.True(Directory.Exists(vsixDeploymentPath));
            Assert.True(File.Exists(Path.Combine(vsixDeploymentPath, "VsixTemplate.dll")), "VsixTemplate.dll not found");
            Assert.True(File.Exists(Path.Combine(vsixDeploymentPath, "VsixTemplate.pkgdef")), "VsixTemplate.pkgdef not found");
            Assert.True(File.Exists(Path.Combine(vsixDeploymentPath, "extension.vsixmanifest")), "extension.vsixmanifest found");
        }
    }
}
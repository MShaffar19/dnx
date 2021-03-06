// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Dnx.Tooling.Publish;
using Microsoft.Dnx.Runtime;
using Microsoft.Extensions.PlatformAbstractions;
using Xunit;

namespace Microsoft.Dnx.Tooling.Publish.Tests
{
    public class DependencyContextFacts
    {
        [Theory]
        [InlineData(Constants.RuntimeNamePrefix + "clr-win-x86.1.0.0", "dnx46,dnx451", "dnx46")]
        [InlineData(Constants.RuntimeNamePrefix + "clr-win-x64.1.0.0", "dnx451,dnxcore50", "dnx451")]
        [InlineData(Constants.RuntimeNamePrefix + "coreclr-win-x86.1.0.0", "dnx451,dnxcore50", "dnxcore50")]
        [InlineData(Constants.RuntimeNamePrefix + "coreclr-win-x64.1.0.0", "dnx451,dnxcore50", "dnxcore50")]
        [InlineData(Constants.RuntimeNamePrefix + "mono.1.0.0", "dnx451,dnxcore50", "dnx451")]  // Absence of architecture component is allowed for mono runtime
        [InlineData(Constants.RuntimeNamePrefix + "mono-x86.1.0.0", "dnx451,dnxcore50", "dnx451")]
        [InlineData(Constants.RuntimeNamePrefix + "clr.1.0.0", "dnx451,dnxcore50", null)]
        [InlineData(Constants.RuntimeNamePrefix + "coreclr-win-x86", "dnx451,dnxcore50", null)]
        [InlineData(Constants.RuntimeNamePrefix + "mono", "dnx451,dnxcore50", null)]
        [InlineData(Constants.RuntimeNamePrefix, "dnx451,dnxcore50", null)]
        public void GetCorrectFrameworkNameForRuntimes(string runtimeName, string options, string framework)
        {
            var frameworkName = DependencyContext.SelectFrameworkNameForRuntime(
                options.Split(',').Select(o => NuGet.VersionUtility.ParseFrameworkName(o)),
                runtimeName);

            if (string.IsNullOrEmpty(framework))
            {
                Assert.Null(frameworkName);
            }
            else
            {
                Assert.Equal(NuGet.VersionUtility.ParseFrameworkName(framework), frameworkName);
            }
        }

        [Theory]
        [InlineData(Constants.RuntimeNamePrefix + "mono.1.0.0", "osx.10.10-x64,osx.10.10-x86,ubuntu.14.04-x64,ubuntu.14.04-x86")]
        [InlineData(Constants.RuntimeNamePrefix + "mono-x86.1.0.0", "osx.10.10-x64,osx.10.10-x86,ubuntu.14.04-x64,ubuntu.14.04-x86")]
        [InlineData(Constants.RuntimeNamePrefix + "clr-win-x86.1.0.0", "win7-x86")]
        [InlineData(Constants.RuntimeNamePrefix + "clr-win-x64.1.0.0", "win7-x64")]
        [InlineData(Constants.RuntimeNamePrefix + "coreclr-win-x86.1.0.0", "win7-x86")]
        [InlineData(Constants.RuntimeNamePrefix + "coreclr-win-x64.1.0.0", "win7-x64")]

        // Invalid Names
        [InlineData(Constants.RuntimeNamePrefix + "mono", null)]
        [InlineData(Constants.RuntimeNamePrefix + "coreclr-win-x86", null)]
        [InlineData(Constants.RuntimeNamePrefix, "")]
        public void GetCorrectRuntimeIdsForRuntimes(string runtimeName, string expectedRids)
        {
            var actual = DependencyContext.GetRuntimeIdentifiers(runtimeName).OrderBy(r => r).ToArray();
            if (string.IsNullOrEmpty(expectedRids))
            {
                Assert.Empty(actual);
            }
            else {
                var expected = expectedRids.Split(',');
                expected = expected.OrderBy(r => r).ToArray();
                Assert.Equal(expected, actual);
            }
        }
    }
}

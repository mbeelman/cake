﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Cake.Common.Tests.Fixtures.Tools.NuGet.Init;
using Cake.Common.Tools.NuGet;
using Cake.Testing;
using Cake.Testing.Xunit;
using Xunit;

namespace Cake.Common.Tests.Unit.Tools.NuGet.Init
{
    public sealed class NuGetIniterTests
    {
        public sealed class TheInitMethod
        {
            [Fact]
            public void Should_Throw_If_Source_Is_Null()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Source = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsArgumentNullException(result, "sourcePackageSourcePath");
            }

            [Fact]
            public void Should_Throw_If_Destination_Is_Null()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Destination = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsArgumentNullException(result, "destinationPackageSourcePath");
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Settings = null;

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsArgumentNullException(result, "settings");
            }

            [Fact]
            public void Should_Throw_If_NuGet_Executable_Was_Not_Found()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.GivenDefaultToolDoNotExist();

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsCakeException(result, "NuGet: Could not locate executable.");
            }

            [Theory]
            [InlineData("/bin/nuget/nuget.exe", "/bin/nuget/nuget.exe")]
            [InlineData("./tools/nuget/nuget.exe", "/Working/tools/nuget/nuget.exe")]
            public void Should_Use_NuGet_Executable_From_Tool_Path_If_Provided(string toolPath, string expected)
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Settings.ToolPath = toolPath;
                fixture.GivenSettingsToolPathExist();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal(expected, result.Path.FullPath);
            }

            [WindowsTheory]
            [InlineData("C:/nuget/nuget.exe", "C:/nuget/nuget.exe")]
            public void Should_Use_NuGet_Executable_From_Tool_Path_If_Provided_On_Windows(string toolPath, string expected)
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Settings.ToolPath = toolPath;
                fixture.GivenSettingsToolPathExist();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal(expected, result.Path.FullPath);
            }

            [Fact]
            public void Should_Throw_If_Process_Was_Not_Started()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.GivenProcessCannotStart();

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsCakeException(result, "NuGet: Process was not started.");
            }

            [Fact]
            public void Should_Throw_If_Process_Has_A_Non_Zero_Exit_Code()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.GivenProcessExitsWithCode(1);

                // When
                var result = Record.Exception(() => fixture.Run());

                // Then
                AssertEx.IsCakeException(result, "NuGet: Process returned an error (exit code 1).");
            }

            [Fact]
            public void Should_Find_NuGet_Executable_If_Tool_Path_Not_Provided()
            {
                // Given
                var fixture = new NuGetIniterFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("/Working/tools/NuGet.exe", result.Path.FullPath);
            }

            [Fact]
            public void Should_Add_Mandatory_Arguments()
            {
                // Given
                var fixture = new NuGetIniterFixture();

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("init \"/Working/NuGet/localfeed\" \"/Working/NuGet/localfeed-destination\" -NonInteractive", result.Args);
            }

            [Fact]
            public void Should_Add_Expand_To_Arguments_If_True()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Settings.Expand = true;

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("init \"/Working/NuGet/localfeed\" \"/Working/NuGet/localfeed-destination\" -Expand -NonInteractive", result.Args);
            }

            [Fact]
            public void Should_Add_ConfigFile_To_Arguments_If_Set()
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Settings.ConfigFile = "./nuget.config";

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal("init \"/Working/NuGet/localfeed\" \"/Working/NuGet/localfeed-destination\" -ConfigFile \"/Working/nuget.config\" " +
                             "-NonInteractive", result.Args);
            }

            [Theory]
            [InlineData(NuGetVerbosity.Detailed, "init \"/Working/NuGet/localfeed\" \"/Working/NuGet/localfeed-destination\" -Verbosity detailed -NonInteractive")]
            [InlineData(NuGetVerbosity.Normal, "init \"/Working/NuGet/localfeed\" \"/Working/NuGet/localfeed-destination\" -Verbosity normal -NonInteractive")]
            [InlineData(NuGetVerbosity.Quiet, "init \"/Working/NuGet/localfeed\" \"/Working/NuGet/localfeed-destination\" -Verbosity quiet -NonInteractive")]
            public void Should_Add_Verbosity_To_Arguments_If_Set(NuGetVerbosity verbosity, string expected)
            {
                // Given
                var fixture = new NuGetIniterFixture();
                fixture.Settings.Verbosity = verbosity;

                // When
                var result = fixture.Run();

                // Then
                Assert.Equal(expected, result.Args);
            }
        }
    }
}

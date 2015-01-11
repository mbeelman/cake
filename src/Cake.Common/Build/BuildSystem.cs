﻿using Cake.Common.Build.AppVeyor;

namespace Cake.Common.Build
{    
    /// <summary>
    /// Provides functionality for interacting with
    /// different build systems.
    /// </summary>
    public sealed class BuildSystem
    {
        private readonly IAppVeyorProvider _appVeyorProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSystem"/> class.
        /// </summary>
        /// <param name="appVeyorProvider">The AppVeyor service.</param>
        public BuildSystem(IAppVeyorProvider appVeyorProvider)
        {
            _appVeyorProvider = appVeyorProvider;
        }

        /// <summary>
        /// Gets a value indicating whether the current build is running on AppVeyor.
        /// </summary>
        /// <value>
        /// <c>true</c> if the build currently is running on AppVeyor; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunningOnAppVeyor
        {
            get { return _appVeyorProvider.IsRunningOnAppVeyor; }
        }

        /// <summary>
        /// Gets a value indicating whether the current build is local build.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the current build is local build; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocalBuild
        {
            get { return !IsRunningOnAppVeyor; }
        }
    }
}

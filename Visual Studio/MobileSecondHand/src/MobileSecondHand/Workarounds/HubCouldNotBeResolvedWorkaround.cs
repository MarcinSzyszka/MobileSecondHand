// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.Extensions.DependencyModel;

namespace MobileSecondHand.Workarounds {
	public class HubCouldNotBeResolvedWorkaround : IAssemblyLocator {
		private static readonly string AssemblyRoot = typeof(Hub).GetTypeInfo().Assembly.GetName().Name;
		private readonly Assembly _entryAssembly;
		private readonly DependencyContext _dependencyContext;

		public HubCouldNotBeResolvedWorkaround(IHostingEnvironment environment) {
			_entryAssembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
			_dependencyContext = DependencyContext.Load(_entryAssembly);
		}

		public virtual IList<Assembly> GetAssemblies() {
			if (_dependencyContext == null) {
				// Use the entry assembly as the sole candidate.
				return new[] { _entryAssembly };
			}

			return _dependencyContext
				.RuntimeLibraries
				.Where(IsCandidateLibrary)
				.SelectMany(l => l.GetDefaultAssemblyNames(_dependencyContext))
				.Select(assembly => Assembly.Load(new AssemblyName(assembly.Name)))
				.ToArray();
		}

		private bool IsCandidateLibrary(RuntimeLibrary library) {
			return library.Dependencies.Any(dependency => string.Equals(AssemblyRoot, dependency.Name, StringComparison.Ordinal));
		}
	}
}
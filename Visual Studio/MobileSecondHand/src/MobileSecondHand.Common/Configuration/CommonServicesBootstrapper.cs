using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MobileSecondHand.Common.CoordinatesHelpers;
using MobileSecondHand.Common.FIleNamesHelpers;
using MobileSecondHand.Common.PathHelpers;

namespace MobileSecondHand.Common.Configuration
{
    public static class CommonServicesBootstrapper
    {
		public static void RegisterServices(IServiceCollection services) {
			services.AddSingleton<IAppFilesNamesHelper, AppFilesNamesHelper>();
			services.AddSingleton<IAppFilesPathHelper, AppFilesPathHelper>();
			services.AddSingleton<ICoordinatesCalculator, CoordinatesCalculator>();
		}
	}
}

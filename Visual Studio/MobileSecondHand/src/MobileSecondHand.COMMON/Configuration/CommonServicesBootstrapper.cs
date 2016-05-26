using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MobileSecondHand.COMMON.CoordinatesHelpers;
using MobileSecondHand.COMMON.FIleNamesHelpers;
using MobileSecondHand.COMMON.PathHelpers;

namespace MobileSecondHand.COMMON.Configuration
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

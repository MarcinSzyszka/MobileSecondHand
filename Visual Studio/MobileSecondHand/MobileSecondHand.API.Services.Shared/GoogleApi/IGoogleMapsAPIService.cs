using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.API.Services.Shared.GoogleApi
{
	public interface IGoogleMapsAPIService
	{
		Task<string> GetAddress(double lat, double lon);
		Task<string> GetCity(double lat, double lon);
	}
}

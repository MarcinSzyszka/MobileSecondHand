using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSecondHand.Services.Google_Api
{
	public interface IGoogleMapsAPIService
	{
		Task<string> GetAddress(double lat, double lon);
	}
}

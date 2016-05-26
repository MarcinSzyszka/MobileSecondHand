using System.Threading.Tasks;
using MobileSecondHand.API.Models.OutsideApisModels;

namespace MobileSecondHand.API.Services.OutsideApisManagers {
	public interface IFacebookApiManager {
		Task<FacebookUserCredentialsResponse> GetUserCredentials(string facebookToken);
	}
}
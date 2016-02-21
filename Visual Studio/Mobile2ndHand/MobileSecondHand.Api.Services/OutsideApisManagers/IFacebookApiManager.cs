using System.Threading.Tasks;
using MobileSecondHand.Api.Models.OutsideApisModels;

namespace MobileSecondHand.Api.Services.OutsideApisManagers {
	public interface IFacebookApiManager {
		Task<FacebookUserCredentialsResponse> GetUserCredentials(string facebookToken);
	}
}
using System.ComponentModel.DataAnnotations;

namespace MobileSecondHand.API.Models.Security {
	public class LoginViewModel {
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}
}

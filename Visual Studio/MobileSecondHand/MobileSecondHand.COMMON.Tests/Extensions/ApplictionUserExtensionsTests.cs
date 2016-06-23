using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.COMMON.Extensions;
using MobileSecondHand.DB.Models.Authentication;
using Xunit;

namespace MobileSecondHand.COMMON.Tests.Extensions {
	public class ApplictionUserExtensionsTests {
		[Fact]
		public void GetUserName_MethodShouldReturnNameFromEmailAdres() {
			//Arrange
			var email = "marcin.szyszka@gmail.com";
			var user = new ApplicationUser { UserName = email };
			
			var expectedResult = "marcin.szyszka";

			//Act
			var result = user.GetUserName();

			//Assert
			Assert.Equal(expectedResult, result);
		}

		[Fact]
		public void GetUserName_MethodShouldReturnFullName_WhenNameIsNotEmail()
		{
			//Arrange
			var name = "marcin.szyszka123";
			var user = new ApplicationUser { UserName = name };

			var expectedResult = "marcin.szyszka123";

			//Act
			var result = user.GetUserName();

			//Assert
			Assert.Equal(expectedResult, result);
		}
	}
}

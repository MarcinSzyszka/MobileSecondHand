using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.COMMON.Extensions;
using Xunit;

namespace MobileSecondHand.COMMON.Tests.Extensions {
	public class DateTimeExtensionsTests {
		[Fact]
		public void GetDateDottedStringFormat_MethodShouldReturnDateInCorrectFormat() {
			//Arrange
			var date = new DateTime(2016, 06, 21, 15, 25, 36);
			var expectedResult = "21.06.2016";

			//Act
			var result = date.GetDateDottedStringFormat();

			//Assert
			Assert.Equal(expectedResult, result);
		}

		[Fact]
		public void GetTimeColonStringFormat_MethodShouldReturnTimeInCorrectFormat() {
			//Arrange
			var date = new DateTime(2016, 06, 21, 15, 25, 06);
			var expectedResult = "15:25:06";

			//Act
			var result = date.GetTimeColonStringFormat();

			//Assert
			Assert.Equal(expectedResult, result);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileSecondHand.API.Services.Advertisement.Keywords;
using MobileSecondHand.DB.Models.Advertisement.Keywords;
using MobileSecondHand.DB.Services.Advertisement.Keywords;
using Moq;
using Xunit;

namespace MobileSecondHand.API.Services.Tests
{
    public class KeywordsService_Tests
    {
		[Fact]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldNotRecognizeAnyKeyword() {
			//Arrange
			var textToRecognize = "Tekst bez żadnego słowa kluczowego";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>(textToRecognize);

			//Assert
			Assert.Equal(0, recognizedKeywords.Count());
		}

		[Fact]
		public void RecognizeAndGetStringCollectionKeywords_ColorKeyword_MethodShouldNotRecognizeAnyKeyword() {
			//Arrange
			var textToRecognize = "Tekst bez żadnego słowa kluczowego";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<ColorKeyword>(textToRecognize);

			//Assert
			Assert.Equal(0, recognizedKeywords.Count());
		}


		[Theory]
		[InlineData("Biała bluzka rozm 40")]
		[InlineData("Sprzedam bluzkę 40")]
		[InlineData("Do sprzedania bluzeczka rozm 44")]
		[InlineData("Mam do sprzedania bluzeczkę rozm 44")]
		[InlineData("Na sprzedaż leci bluza rozm 44")]
		[InlineData("Mam do sprzedania fajną bluzę")]
		[InlineData("Bluzeczki na sprzedaż!")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_BLUZKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Bluzki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czerowne body")]
		[InlineData("Body do sprzedania")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_BODY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Body";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czerowne bolerko")]
		[InlineData("Bolerko do sprzedania")]
		[InlineData("Bolerka do na handel")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_BOLERKA(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Bolerka";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czerowne dresy")]
		[InlineData("Dresiki do sprzedania")]
		[InlineData("Dressowe cos na handel")]
		[InlineData("Dresowe cos na handel")]
		[InlineData("Dres do sprzednia")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_DRESY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Dresy";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czerowna garsonka")]
		[InlineData("Garsonki do sprzedania")]
		[InlineData("Garsoneczka cos na handel")]
		[InlineData("Sprzedam garsonkę")]
		[InlineData("Mam na handel garsoneczkę")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_GARSONKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Garsonki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czerowny golf")]
		[InlineData("Pozbedę się wysokiego golfa")]
		[InlineData("Cos tam z golfem do sprzedania")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_GOLFY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Golfy";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarny gorset")]
		[InlineData("Pozbedę się czarnego gorsetu")]
		[InlineData("Cos tam z czarnym gorsetem do sprzedania")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_GORSETY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Gorsety";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarna Kamizelka")]
		[InlineData("Pozbedę się czarnej kamizelki")]
		[InlineData("Cos tam z kamizelką do sprzedania")]
		[InlineData("Mam do sprzedania kamizelkę")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_KAMIZELKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Kamizelki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}


		[Theory]
		[InlineData("Czarny kombinezon")]
		[InlineData("Pozbedę się czarnego kombinezonu")]
		[InlineData("Cos tam z kombinezonem do sprzedania")]
		[InlineData("Mam do sprzedania kombinezonik")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_KOMBINEZONY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Kombinezony";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarny kostium")]
		[InlineData("Pozbedę się czarnego kostiumu")]
		[InlineData("Mam do sprzedania kostiumik")]
		[InlineData("Mam do sprzedania kostiumy")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_KOSTIUMY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Kostiumy";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Mam do sprzedania koszulkę")]
		[InlineData("Mam do sprzedania koszulki")]
		[InlineData("KOszulka na sprzedaż")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_KOSZULKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Koszulki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarna koszula")]
		[InlineData("Pozbedę się czarnej koszuli")]
		[InlineData("Mam do sprzedania koszulę")]
		[InlineData("Mam do sprzedania koszule")]
		[InlineData("KOszula na sprzedaż")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_KOSZULE(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Koszule";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarna marynarka")]
		[InlineData("Pozbedę się czarnej marynarki")]
		[InlineData("Mam do sprzedania marynarkę")]
		[InlineData("Mam do sprzedania marynarki")]
		[InlineData("Marynareczka na sprzedaż")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_MARYNARKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Marynarki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarne spodnie")]
		[InlineData("Pozbedę się czarnych spodni")]
		[InlineData("Mam do sprzedania spodenki")]
		[InlineData("Mam do sprzedania spodeneczki")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_SPODNIE(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Spodnie";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}


		[Theory]
		[InlineData("Czarna spódnica")]
		[InlineData("Pozbedę się czarnej spódniczki")]
		[InlineData("Mam do sprzedania spódniczkę")]
		[InlineData("Mam do sprzedania spódnicę")]
		[InlineData("Spódniczka na handel")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_SPÓDNICE_SPÓDNICZKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Spódnice/Spódniczki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarna sukienka")]
		[InlineData("Mam do sprzedania suknię")]
		[InlineData("Mam do sprzedania sukienkę")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_SUKIENKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Sukienki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarny sweter")]
		[InlineData("Pozbedę się czarnego swetra")]
		[InlineData("Mam do sprzedania swetr")]
		[InlineData("Mam do sprzedania sweterek")]
		[InlineData("Swetry na handel")]
		[InlineData("Sweterki na handel")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_SWETRY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Swetry";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarna tunika")]
		[InlineData("Pozbedę się czarnej tuniki")]
		[InlineData("Mam do sprzedania tuniczkę")]
		[InlineData("Mam do sprzedania tunikę")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_TUNIKI(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Tuniki";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}

		[Theory]
		[InlineData("Czarne buty")]
		[InlineData("Pozbedę się czarnych butów")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnOneKeyword_BUTY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeyword = "Buty";

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(1, recognizedKeywords.Count());
			Assert.Equal(expectedKeyword, recognizedKeywords.First());
		}



		[Theory]
		[InlineData("Biała bluzka rozm 40 i do tego leginsy")]
		[InlineData("Sprzedam bluzkę wraz z legginsami 40")]
		[InlineData("Do sprzedania bluzeczka razem z leginsami rozm 44")]
		[InlineData("Mam do sprzedania bluzeczkę i legginsy")]
		public void RecognizeAndGetStringCollectionKeywords_CategoryKeyword_MethodShouldReturnTwoKeywords_BLUZKI_and_LEGGINSY(object inlineDataTextToRecognize) {
			//Arrange
			var expectedKeywords = new List<string> { "Bluzki", "Legginsy" };

			//Act
			var recognizedKeywords = serviceUnderTest.RecognizeAndGetStringCollectionKeywords<CategoryKeyword>((string)inlineDataTextToRecognize);

			//Assert
			Assert.Equal(2, recognizedKeywords.Count());
			Assert.Equal(expectedKeywords[0], recognizedKeywords.First());
			Assert.Equal(expectedKeywords[1], recognizedKeywords.ToList()[1]);
		}

		#region CONFIGURATION
		Mock<IKeywordsDbService> keywordsDbServiceMock = new Mock<IKeywordsDbService>();
		IKeywordsService serviceUnderTest;
		public KeywordsService_Tests() {
			serviceUnderTest = new KeywordsService(keywordsDbServiceMock.Object);
		}
		#endregion
	}
}

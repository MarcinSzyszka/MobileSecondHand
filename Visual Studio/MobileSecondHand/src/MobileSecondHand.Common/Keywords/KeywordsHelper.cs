using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileSecondHand.Common.Keywords {
	public class KeywordsHelper {
		public IDictionary<string, List<string>> CategoryKeywords { get; private set; }
		public IDictionary<string, List<string>> ColorKeywords { get; private set; }

		public KeywordsHelper() {
			InitializeDictionaries();
		}

		private void InitializeDictionaries() {
			InitializeCategoryKeywordsDictionary();
			InitializeColorKeywordsDictionary();
		}

		private void InitializeColorKeywordsDictionary() {
			this.ColorKeywords = new Dictionary<string, List<string>>();
			this.ColorKeywords.Add("Biał", new List<string> {
				"Biały",
				"Biała",
				"Białe",
				"Białego"
			});
			this.ColorKeywords.Add("Czarn", new List<string> {
				"Czarny",
				"Czarna",
				"Czarne",
				"Czarnego"
			});
			this.ColorKeywords.Add("Brąz", new List<string> {
				"Brązowy",
				"Brązowa",
				"Brązowe",
				"Brązowego"
			});
			this.ColorKeywords.Add("Czerw", new List<string> {
				"Czerwony",
				"Czerwieni",
				"Czerwona",
				"Czerwone",
				"Czerwonego"
			});
			this.ColorKeywords.Add("Fiolet", new List<string> {
				"Fioletowy",
				"Fioletu",
				"Fiolecie",
				"Fioletowa",
				"Fioletowe",
				"Fioletowego"
			});
			this.ColorKeywords.Add("Niebiesk", new List<string> {
				"Niebieski",
				"Niebieska",
				"Niebieskie",
				"Niebieskiego"
			});
			this.ColorKeywords.Add("Pomarańcz", new List<string> {
				"Pomarańczowy",
				"Pomarańcz",
				"Pomarańczowa",
				"Pomarańczowe",
				"Pomarańczowego"
			});
			this.ColorKeywords.Add("Róż", new List<string> {
				"Różowy",
				"Róż",
				"Różu",
				"Różowa",
				"Różowe",
				"Różowego"
			});
			this.ColorKeywords.Add("Szar", new List<string> {
				"Szary",
				"Szara",
				"Szare",
				"Szarego"
			});
			this.ColorKeywords.Add("Ziel", new List<string> {
				"Zielony",
				"Zielona",
				"Zielone",
				"Zieleń",
				"Zieleni",
				"Zielonego"
			});
			this.ColorKeywords.Add("Żół", new List<string> {
				"Żółty",
				"Żółta",
				"Żółte",
				"Żółtego"
			});

		}

		private void InitializeCategoryKeywordsDictionary() {
			this.CategoryKeywords = new Dictionary<string, List<string>>();
			this.CategoryKeywords.Add("Bluz", new List<string> {
				"Bluzki",
				"Bluzeczka",
				"Bluzeczki",
				"Bluzeczkę",
				"Bluzka",
				"Bluzkę",
				"Bluza",
				"Bluzę",
				"Bluzy"
			});
			this.CategoryKeywords.Add("Body", new List<string> {
				"Body"
			});
			this.CategoryKeywords.Add("Boler", new List<string> {
				"Bolerka",
				"Bolerko",
				"Bolero",
				"Bolerka"
			});
			this.CategoryKeywords.Add("Dres", new List<string> {
				"Dresy",
				"Dress",
				"Dres",
				"Dresik",
				"Dresiki",
				"Dresowa",
				"Dresowy",
				"Dresowe"
			});
			this.CategoryKeywords.Add("Garson", new List<string> {
				"Garsonki",
				"Garsonkę",
				"Garsonka",
				"Garsoneczka",
				"Garsoneczkę",
				"Garsoneczki"
			});
			this.CategoryKeywords.Add("Golf", new List<string> {
				"Golfy",
				"Golf",
				"Golfik",
				"Golfem",
				"Golfa"
			});
			this.CategoryKeywords.Add("Gorset", new List<string> {
				"Gorsety",
				"Gorset",
				"Gorsetem"
			});
			this.CategoryKeywords.Add("Kamizel", new List<string> {
				"Kamizelki",
				"Kamizelka",
				"Kamizeleczka",
				"Kamizelkę",
				"Kamizelką"
			});
			this.CategoryKeywords.Add("Kombinezon", new List<string> {
				"Kombinezony",
				"Kombinezon",
				"Kombinezonik",
				"Kombinezonu",
				"Kombinezonem"
			});
			this.CategoryKeywords.Add("Kostium", new List<string> {
				"Kostiumy",
				"Kostium",
				"Kostiumu",
				"Kostiumik"
			});
			this.CategoryKeywords.Add("Koszulk", new List<string> {
				"Koszulki",
				"Koszulka",
				"Koszulkę"
				//"T-shirt",
				//"T-shirtu",
				//"Tshirt",
				//"Tshirtu"
			});
			this.CategoryKeywords.Add("Koszul", new List<string> {
				"Koszule",
				"Koszulę",
				"Koszula",
				"Koszuli"
			});
			this.CategoryKeywords.Add("Leg", new List<string> {
				"Legginsy",
				"Leginsy",
				"Leginsami",
				"Legginsami"
			});
			this.CategoryKeywords.Add("Marynar", new List<string> {
				"Marynarki",
				"Marynarka",
				"Marynareczka",
				"Marynara",
				"Marynarkę"
			});
			this.CategoryKeywords.Add("Spod", new List<string> {
				"Spodnie",
				"Spodenki",
				"Spodeneczki",
				"Spodni",
			});
			this.CategoryKeywords.Add("Spódni", new List<string> {
				"Spódnice/Spódniczki",
				"Spódnice",
				"Spódnica",
				"Spódnicę",
				"Spódniczki",
				"Spódniczka",
				"Spódniczkę",
			});
			this.CategoryKeywords.Add("Suk", new List<string> {
				"Sukienki",
				"Sukienka",
				"Sukienkę",
				"Sukieneczkę",
				"Suknię"
				//"Kiecka",
				//"Kieca",
				//"Kieckę",
				//"Kiecę",
				//"Kiecki"
			});
			this.CategoryKeywords.Add("Swet", new List<string> {
				"Swetry",
				"Swetr",
				"Sweter",
				"Sweterek",
				"Sweterkiem",
				"Swetra",
				"Sweterki"
			});
			this.CategoryKeywords.Add("Tuni", new List<string> {
				"Tuniki",
				"Tunika",
				"Tuniczka",
				"Tunikę",
				"Tuniczkę"
			});
			this.CategoryKeywords.Add("But", new List<string> {
				"Buty",
				"Butki",
				"Butów"
				//"Sandały",
				//"Szpilki",
				//"Obcasy",
				//"Baleriny",
				//"Balerinki",
				//"Kozaki",
				//"Klapki",
				//"Japonki"
			});
		}
	}
}

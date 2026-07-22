using System.Text.Json.Serialization;

namespace SovereigntyBot.Services.Endpoints.Sovsmp
{
	public class SexpDailyEntry
	{
		[JsonPropertyName("date")]
		public string Date { get; set; }
		[JsonPropertyName("uuid")]
		public string Uuid { get; set; }
		[JsonPropertyName("sexpDaily")]
		public int Sexp { get; set; }
	}
}
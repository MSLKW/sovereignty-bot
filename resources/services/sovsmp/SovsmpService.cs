using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using SovereigntyBot.Services.Endpoints.Sovsmp;
using System.Globalization;

namespace SovereigntyBot.Services
{
	public class SovsmpService
	{
		private HttpClient _client = new HttpClient();
		private	string	_apiKey;
		private JsonSerializerOptions _options = new(){ WriteIndented = true };
		private CancellationTokenSource _source = new();
		private CancellationToken _token { get{return _source.Token; } }

		public SovsmpService(string apiKey)
		{
			_apiKey = apiKey; // get a check for this
			_client = new HttpClient();
		}
		
		public async Task<List<SexpDailyEntry>?> GetSexpDataAsync()
		{
			try
			{
				List<SexpDailyEntry> data = await _client.GetFromJsonAsync<List<SexpDailyEntry>>($"http://129.150.60.28:5001/sexp-daily?key={_apiKey}", _options, _token);
				return data;
			}
			catch(HttpRequestException exception)
			{
				if(exception.StatusCode.HasValue == true)
                {
                    HttpStatusCode statusCode = exception.StatusCode.Value;
                    switch(statusCode)
                    {
                        case HttpStatusCode.NoContent:
                            await Program.Log(new Discord.LogMessage(Discord.LogSeverity.Warning, "SovsmpService.cs", "No content, Something went wrong :(", exception));
                            break;
                    }
                }
                return null;
			}
		}

		public static int GetWeeklySexp(List<SexpDailyEntry> data, string uuid)
		{
			int	totalSexp = 0;
			foreach(SexpDailyEntry entry in data)
			{
				DateTime date = DateTime.Parse(entry.Date, null, DateTimeStyles.RoundtripKind);
				TimeSpan duration = DateTime.Now.Subtract(date);
				if (entry.Uuid == uuid && duration.Days <= 7)
				{
					totalSexp += entry.Sexp;
				}
			}
			return totalSexp;
		}
	}
}
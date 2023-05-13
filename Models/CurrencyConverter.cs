#nullable disable
using Dhicoin.Areas.Identity.Data;
using Newtonsoft.Json;
using System.Web;

namespace Dhicoin.Models
{
    public class CurrencyConverter
    {
        private readonly ApplicationDbContext _context;

        public CurrencyConverter(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        private const string ApiUrl = "https://api.apilayer.com/currency_data/convert";
        private const string ApiKey = "TxPpQ3AjlRpiB3iYfHU5OYicAoFtPM7W";
        public async Task ConvertCurrency()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apikey", ApiKey);
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["to"] = "USD";
            queryString["from"] = "MVR";
            queryString["amount"] = "1";
            var url = $"{ApiUrl}?{queryString}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(content);

                CurrencyExchange currency = new CurrencyExchange();
                currency.MVRTOUSD = myDeserializedClass.result.ToString();
                _context.CurrencyExchanges.Add(currency);
                _context.SaveChanges();

                Console.WriteLine(myDeserializedClass);
            }
            else
            {
                Console.WriteLine($"Failed to convert currency. Status code: {response.StatusCode}");
            }
        }
    }
}
public class Info
{
    public int timestamp { get; set; }
    public double quote { get; set; }
}
public class Query
{
    public string from { get; set; }
    public string to { get; set; }
    public int amount { get; set; }
}
public class Root
{
    public bool success { get; set; }
    public Query query { get; set; }
    public Info info { get; set; }
    public decimal result { get; set; }
}




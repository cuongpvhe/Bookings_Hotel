using Microsoft.VisualStudio.Web.CodeGeneration;
using System.Text;

namespace Bookings_Hotel.Service
{
    public class CassoIntegration
    {
        private static readonly string apiKey = "AK_CS.da7082a08fd411efa865954870c6d3a1.nL5xCljKfKSF3qNLzPHmE7Vbeohn5BZFFTF484rFJymcJ5fst4t1C3RQj1RxXH0ye3FrYwD5";
        private static readonly string baseUrl = "https://api.casso.vn/v2/transactions"; // Endpoint của API

        public static async Task<string> CreateTransactionAsync(string accountId, decimal amount, string description)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var payload = new
                {
                    account_id = accountId,
                    amount = amount,
                    description = description
                };

                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(baseUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result; // Return transaction details
                }
                else
                {
                    throw new Exception($"Failed to create transaction: {response.ReasonPhrase}");
                }
            }
        }

        public static async Task<string> GetTransactionsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // Thêm API Key vào Header của yêu cầu
                client.DefaultRequestHeaders.Add("Authorization", $"Apikey {apiKey}");

                // Gửi yêu cầu GET đến API Casso
                HttpResponseMessage response = await client.GetAsync("https://oauth.casso.vn/v2/transactions?fromDate=" + GetYesterdayDate() + "&pageSize=100");

                if (response.IsSuccessStatusCode)
                {
                    // Đọc nội dung phản hồi
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody; // Trả về chuỗi JSON từ API
                }
                else
                {
                    throw new Exception($"API request failed with status code: {response.StatusCode}");
                }
            }
        }

        public static string GetYesterdayDate()
        {
            // Lấy ngày hiện tại và trừ đi 1 ngày để có ngày hôm qua
            DateTime yesterday = DateTime.Now.AddDays(-1);

            // Trả về ngày hôm qua theo định dạng yyyy-MM-dd
            return yesterday.ToString("yyyy-MM-dd");
        }
    }
}

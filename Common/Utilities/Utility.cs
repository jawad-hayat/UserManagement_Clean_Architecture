
using Models.Requests.ContactUs;
using System.Text.Json;

namespace Common.Utilities
{
    public class Utility
    {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }

        public static string ContactUsEmailBody(ContactRequest request)
        {
            return "Dear Sir! <br />" +
            "<p>I hope this email finds you well. I am excited to contact with you through your website's portal. My details are as under:</p>" +
            "<p><strong>Name:</strong> " + request.Name + " </p>" +
            "<p><strong>Email Address</strong>: " + request.EmailAddress + " </p>" +
            "<p><strong>Message</strong>: " + request.Message + " </p>" +
            "<p>Thank you. </p>" +
            "<p>Regards,</p>" +
            "<p>" + request.Name + "</p>";
        }

        public static async Task<List<dynamic>> GetData(string url)
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Throw an exception if HTTP request was unsuccessful
            var responseContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var data = JsonSerializer.Deserialize<List<dynamic>>(responseContent, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            return data;
        }
    }
}

using System.Text;

namespace UsbEventLoggerToSplunk
{
    public class SplunkApiClient
    {
        private readonly string _splunkUrl;
        private readonly string _splunkToken;

        public SplunkApiClient(string splunkUrl, string splunkToken)
        {
            _splunkUrl = splunkUrl;
            _splunkToken = splunkToken;
        }

        public async Task SendDataToSplunkAsync(string jsonData)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_splunkToken}");
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_splunkUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Data sent to Splunk successfully.");
            }
            else
            {
                Console.WriteLine("Failed to send data to Splunk.");
            }
        }
    }
}


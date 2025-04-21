using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UsbEventLoggerToSplunk
{
    public class UsbEventHandler
    {
        private readonly string _splunkUrl;
        private readonly string _splunkToken;

        public UsbEventHandler(string splunkUrl, string splunkToken)
        {
            _splunkUrl = splunkUrl;
            _splunkToken = splunkToken;
        }

        public async Task HandleUsbEventAsync(UsbDevice device)
        {
            Console.WriteLine($"Processing event: {device.DeviceName} ({device.DeviceId})");

            // Prepare the data to send to Splunk
            var jsonContent = JsonConvert.SerializeObject(device);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_splunkToken}");

            try
            {
                var response = await client.PostAsync(_splunkUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Device event sent to Splunk.");
                }
                else
                {
                    Console.WriteLine($"Failed to send event to Splunk. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending event to Splunk: {ex.Message}");
            }
        }
    }
}

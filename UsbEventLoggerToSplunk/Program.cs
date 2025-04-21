namespace UsbEventLoggerToSplunk
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Splunk configuration
            string splunkUrl = "https://your-splunk-url";
            string splunkToken = "your-splunk-token";

            // Initialize event handler
            var eventHandler = new UsbEventHandler(splunkUrl, splunkToken);
            var usbListener = new UsbListener();

            // Subscribe to events
            usbListener.UsbInserted += async (device) => await eventHandler.HandleUsbEventAsync(device);
            usbListener.UsbEjected += async (device) => await eventHandler.HandleUsbEventAsync(device);

            // Start listening for USB events
            usbListener.StartListening();

            Console.WriteLine("Listening for USB events... Press any key to exit.");
            Console.ReadKey();
        }
    }
}

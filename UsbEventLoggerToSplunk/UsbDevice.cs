using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbEventLoggerToSplunk
{
    public class UsbDevice
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime EventTimestamp { get; set; }
    }
}

using System;
using System.Management;

namespace UsbEventLoggerToSplunk
{
    public class UsbListener
    {
        public event Action<UsbDevice> UsbInserted;
        public event Action<UsbDevice> UsbEjected;

        public void StartListening()
        {
            WatchUsbDevices();
        }

        private void WatchUsbDevices()
        {
            // Watch for USB insertions (EventType = 2)
            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2"));
            insertWatcher.EventArrived += (sender, e) =>
            {
                var device = GetDeviceInfo(e.NewEvent);  // Use NewEvent property
                device.DeviceId = "USB Device Inserted";
                UsbInserted?.Invoke(device);
            };

            // Watch for USB removals (EventType = 3)
            ManagementEventWatcher ejectWatcher = new ManagementEventWatcher(
                new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3"));
            ejectWatcher.EventArrived += (sender, e) =>
            {
                var device = GetDeviceInfo(e.NewEvent);  // Use NewEvent property
                device.DeviceId = "USB Device Ejected";
                UsbEjected?.Invoke(device);
            };

            insertWatcher.Start();
            ejectWatcher.Start();
        }

        private UsbDevice GetDeviceInfo(ManagementBaseObject e)
        {
            string driveLetter = e.Properties["DriveName"]?.Value?.ToString(); // e.g., "D:"
            if (string.IsNullOrEmpty(driveLetter)) return null;

            var device = new UsbDevice
            {
                DeviceId = driveLetter,
                EventTimestamp = DateTime.Now,
                DeviceName = "Unknown Device"
            };

            try
            {
                // Step 1: Find Partition from LogicalDisk
                using var partitionQuery = new ManagementObjectSearcher(
                    $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{driveLetter}'}} " +
                    "WHERE AssocClass = Win32_LogicalDiskToPartition");

                foreach (ManagementObject partition in partitionQuery.Get())
                {
                    var partitionId = partition["DeviceID"]?.ToString();
                    if (string.IsNullOrEmpty(partitionId)) continue;

                    // Step 2: Find DiskDrive from Partition
                    using var driveQuery = new ManagementObjectSearcher(
                        $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} " +
                        "WHERE AssocClass = Win32_DiskDriveToDiskPartition");

                    foreach (ManagementObject drive in driveQuery.Get())
                    {
                        var model = drive["Model"]?.ToString();
                        if (!string.IsNullOrEmpty(model))
                        {
                            device.DeviceName = model; // e.g., "SanDisk Ultra USB Device"
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching device name: " + ex.Message);
            }

            return device;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Exceptions;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Extensions;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Helpers;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Models;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Models.Commands;
using Microsoft.Azure.Devices.Shared;

namespace Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Factory
{
    public static class SampleDeviceFactory
    {
        public const string OBJECT_TYPE_DEVICE_INFO = "DeviceInfo";

        public const string VERSION_1_0 = "1.0";

        private const int MAX_COMMANDS_SUPPORTED = 6;

        private const bool IS_SIMULATED_DEVICE = true;

        private static readonly Random Rand = new Random();

        private static readonly List<string> DefaultDeviceNames = new List<string>
        {
            "Tara-Aangan",
            "Mundhwa-Old-Bridge",
            "Government-Observation-Home",
            "Vadbhan-Road",
            "Turfpark-Mundhwa",
            "Le-Kebabiere",
            "Koregaon-Park-Central",
            "HH-Aga-Khan-Bridge",
            "Liberty-Phase-1And2",
            "Burning-Ghat",
            "Yerawada-Bridge",
            "Abdul-Kalam-Hall",
            "Chulangan-Hotel",
            "Hanuman-Mandir",
            "Sangamwadi-Road",
            "Madhavrao-Scindia-Garden",
            "Laxmi-Narayan-Kunj"
        };

        private static readonly List<string> FreeFirmwareDeviceNames = new List<string>
        {
            "Tara-Aangan",
            "Mundhwa-Old-Bridge",
            "Government-Observation-Home",
            "Vadbhan-Road",
            "Turfpark-Mundhwa",
            "Le-Kebabiere",
            "Koregaon-Park-Central",
            "HH-Aga-Khan-Bridge",
            "Liberty-Phase-1And2",
            "Burning-Ghat",
            "Yerawada-Bridge",
            "Abdul-Kalam-Hall",
            "Chulangan-Hotel",
            "Hanuman-Mandir",
            "Sangamwadi-Road",
            "Madhavrao-Scindia-Garden",
            "Laxmi-Narayan-Kunj"
        };

        private static readonly List<string> HighWaterLevelDeviceNames = new List<string>
        {
          "Madhavrao Scindia Garden",
          "Laxmi Narayan Kunj"
        };

        private class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

        }

        private static List<Location> _possibleDeviceLocations = new List<Location>{
            new Location(18.535056, 73.936361),  // Microsoft Red West Campus, Building A
            new Location(18.535280, 73.933679),  // Microsoft Red West Campus, Building A
            new Location(18.537077, 73.927815),  // Microsoft Red West Campus, Building A
            new Location(18.538453, 73.922965),  // Microsoft Red West Campus, Building A
            new Location(18.538613, 73.920342),  // 800 Occidental Ave S, Seattle, WA 98134
            new Location(18.539493, 73.912807),  // 11111 NE 8th St, Bellevue, WA 98004
            new Location(18.540459, 73.908101),  // 3003 160th Ave SE Bellevue, WA 98008
            new Location(18.540711, 73.904289),  // 15580 NE 31st St Redmond, WA 98008
            new Location(18.540988, 73.898541),
            new Location(18.542916, 73.888522),
            new Location(18.542463, 73.885135),
            new Location(18.542036, 73.881605),
            new Location(18.537670, 73.866308),
            new Location(18.530729, 73.860955),
            new Location(18.532370, 73.855333),
            new Location(18.540757, 73.853552),
            new Location(18.550031, 73.856186)
        };

        private static List<string> _possibleBuildingTags = new List<string>
        {
            "Building 40",
            "Building 43"
        };

        private static List<string> _possibleFloorTags = new List<string>
        {
            "1F",
            "2F",
        };
        private static int totalDevicesAssigned;

        public static DeviceModel GetSampleSimulatedDevice(string deviceId, string key)
        {
            DeviceModel device = DeviceCreatorHelper.BuildDeviceStructure(deviceId, true, null);

            AssignDeviceProperties(device);
            device.ObjectType = OBJECT_TYPE_DEVICE_INFO;
            device.Version = VERSION_1_0;
            device.IsSimulatedDevice = IS_SIMULATED_DEVICE;

            AssignTelemetry(device);
            AssignCommands(device);

            return device;
        }

        public static DeviceModel GetSampleDevice(Random randomNumber, SecurityKeys keys)
        {
            var deviceId = string.Format(
                    CultureInfo.InvariantCulture,
                    "00000-DEV-{0}C-{1}LK-{2}D-{3}",
                    MAX_COMMANDS_SUPPORTED,
                    randomNumber.Next(99999),
                    randomNumber.Next(99999),
                    randomNumber.Next(99999));

            var device = DeviceCreatorHelper.BuildDeviceStructure(deviceId, false, null);
            device.ObjectName = "IoT Device Description";

            AssignDeviceProperties(device);
            AssignTelemetry(device);
            AssignCommands(device);

            return device;
        }

        private static void AssignDeviceProperties(DeviceModel device)
        {
            int randomId = Rand.Next(0, _possibleDeviceLocations.Count - 1);
            if (device?.DeviceProperties == null)
            {
                throw new DeviceRequiredPropertyNotFoundException("Required DeviceProperties not found");
            }

            device.DeviceProperties.HubEnabledState = true;
            device.DeviceProperties.Manufacturer = "Contoso Inc.";
            device.DeviceProperties.ModelNumber = "MD-" + randomId;
            device.DeviceProperties.SerialNumber = "SER" + randomId;

            if (FreeFirmwareDeviceNames.Any(n => device.DeviceProperties.DeviceID.StartsWith(n, StringComparison.Ordinal)))
            {
                device.DeviceProperties.FirmwareVersion = "1." + randomId;
            }
            else
            {
                device.DeviceProperties.FirmwareVersion = "2.0";
            }

            device.DeviceProperties.Platform = "Plat-" + randomId;
            device.DeviceProperties.Processor = "i3-" + randomId;
            device.DeviceProperties.InstalledRAM = randomId + " MB";

            // Choose a location among the 16 above and set Lat and Long for device properties
            device.DeviceProperties.Latitude = _possibleDeviceLocations[totalDevicesAssigned < _possibleDeviceLocations.Count ? totalDevicesAssigned: 0].Latitude;
            device.DeviceProperties.Longitude = _possibleDeviceLocations[totalDevicesAssigned < _possibleDeviceLocations.Count ? totalDevicesAssigned : 0].Longitude;
            totalDevicesAssigned++;
        }

        private static void AssignTelemetry(DeviceModel device)
        {
            device.Telemetry.Add(new Telemetry("WaterLevel", "WaterLevel", "double"));
            // device.Telemetry.Add(new Telemetry("Humidity", "Humidity", "double"));
        }

        private static void AssignCommands(DeviceModel device)
        {
            // Device commands
            device.Commands.Add(new Command(
                "PingDevice",
                DeliveryType.Message,
                "The device responds to this command with an acknowledgement. This is useful for checking that the device is still active and listening."
            ));
            device.Commands.Add(new Command(
                "StartTelemetry",
                DeliveryType.Message,
                "Instructs the device to start sending telemetry."
            ));
            device.Commands.Add(new Command(
                "StopTelemetry",
                DeliveryType.Message,
                "Instructs the device to stop sending telemetry."
            ));
            device.Commands.Add(new Command(
                "ChangeSetPointWaterLevel",
                DeliveryType.Message,
                "Controls the simulated water level telemetry values the device sends. This is useful for testing back-end logic.",
                new[] { new Parameter("SetPointWaterLevel", "double") }
            ));
            device.Commands.Add(new Command(
                "DiagnosticTelemetry",
                DeliveryType.Message,
                "Controls if the device should send the external water level as telemetry.",
                new[] { new Parameter("Active", "boolean") }
            ));
            device.Commands.Add(new Command(
                "ChangeDeviceState",
                DeliveryType.Message,
                "Sets the device state metadata property that the device reports. This is useful for testing back-end logic.",
                new[] { new Parameter("DeviceState", "string") }
            ));

            // Device methods
            device.Commands.Add(new Command(
                "InitiateFirmwareUpdate",
                DeliveryType.Method,
                "Updates device Firmware. Use parameter 'FwPackageUri' to specifiy the URI of the firmware file, e.g. https://iotrmassets.blob.core.windows.net/firmwares/FW20.bin",
                new[] { new Parameter("FwPackageUri", "string") }
            ));
            device.Commands.Add(new Command(
                "Reboot",
                DeliveryType.Method,
                "Reboot the device"
            ));
            device.Commands.Add(new Command(
                "FactoryReset",
                DeliveryType.Method,
                "Reset the device (including firmware and configuration) to factory default state"
            ));
        }

        public static List<string> GetDefaultDeviceNames()
        {
            long milliTime = DateTime.Now.Millisecond;
            return DefaultDeviceNames.Select(r => string.Concat(r, "")).ToList();
        }

        public static void AssignDefaultTags(DeviceModel device)
        {
            if (device.Twin == null)
            {
                device.Twin = new Twin();
            }

            device.Twin.Tags["Building"] = Random(_possibleBuildingTags);
            device.Twin.Tags["Floor"] = Random(_possibleFloorTags);
        }

        public static void AssignDefaultDesiredProperties(DeviceModel device)
        {
            if (HighWaterLevelDeviceNames.Any(n => device.DeviceProperties.DeviceID.StartsWith(n, StringComparison.Ordinal)))
            {
                if (device.Twin == null)
                {
                    device.Twin = new Twin();
                }

                device.Twin.Properties.Desired.Set("Config.WaterLevelMeanValue", 70);
            }
        }

        private static T Random<T>(IList<T> range)
        {
            return range[Rand.Next(range.Count)];
        }
    }
}

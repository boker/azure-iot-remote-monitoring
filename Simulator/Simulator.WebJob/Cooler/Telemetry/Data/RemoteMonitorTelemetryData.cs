﻿namespace Microsoft.Azure.Devices.Applications.RemoteMonitoring.Simulator.WebJob.WaterLevel.Telemetry.Data
{
    public class RemoteMonitorTelemetryData
    {
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double? ExternalTemperature { get; set; }
    }
}

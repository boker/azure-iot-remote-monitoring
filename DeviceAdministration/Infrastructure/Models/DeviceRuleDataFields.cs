using System.Collections.Generic;

namespace Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Infrastructure.Models
{
    public static class DeviceRuleDataFields
    {
        public static string WaterLevel
        { 
            get 
            { 
                return "WaterLevel"; 
            } 
        }

        private static List<string> _availableDataFields = new List<string>
        {
            WaterLevel
        };

        public static List<string> GetListOfAvailableDataFields()
        {
            return _availableDataFields;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class DeviceCommandRequest : MessageBase
    {
        public string deviceId { get; set; }    
        public string command { get; set; }

        public DeviceCommandRequest() { }

        public DeviceCommandRequest(string deviceId, string command)
        {
            this.deviceId = deviceId;
            this.command = command;
        }
    }
}

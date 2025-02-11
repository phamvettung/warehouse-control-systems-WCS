using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class DeviceStatusResponse : MessageBase
    {
        public string deviceId { get; set; }
        public string status { get; set; }

        public DeviceStatusResponse() { }

        public DeviceStatusResponse(string deviceId, string status)
        {
            this.deviceId = deviceId;
            this.status = status;
        }
    }
}

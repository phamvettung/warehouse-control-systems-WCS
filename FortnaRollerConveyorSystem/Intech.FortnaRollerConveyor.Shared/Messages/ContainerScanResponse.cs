using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class ContainerScanResponse : MessageBase
    {
        public string deviceId { get; set; }

        public ContainerScanResponse() { }

        public ContainerScanResponse(string deviceId)
        {
            this.deviceId = deviceId;
        }
    }
}

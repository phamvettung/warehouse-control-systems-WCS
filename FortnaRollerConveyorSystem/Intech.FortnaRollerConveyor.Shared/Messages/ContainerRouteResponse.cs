using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class ContainerRouteResponse : MessageBase
    {
        public string scannerName { get; set; }
        public string deviceId { get; set; }
        public string barcode { get; set; }

        public ContainerRouteResponse()
        {

        }

        public ContainerRouteResponse(string scannerName, string deviceId, string barcode)
        {
            this.scannerName = scannerName;
            this.deviceId = deviceId;
            this.barcode = barcode;
        }
    }
}

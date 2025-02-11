using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class ContainerScanRequest : MessageBase
    {
        public string deviceId {  get; set; }
        public string barcode { get; set; }

        public ContainerScanRequest() { }

        public ContainerScanRequest(string deviceId, string barcode)
        {
            this.deviceId = deviceId;
            this.barcode = barcode;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class ContainerDivert : MessageBase
    {
        public string scannerName {  get; set; }
        public string deviceId { get; set; }
        public string barcode { get; set; }

        public ContainerDivert()
        {
            scannerName = string.Empty;
            deviceId = string.Empty;
            barcode = string.Empty;
        }

        public ContainerDivert(string scannerName, string deviceId, string barcode)
        {
            this.scannerName = scannerName;
            this.deviceId = deviceId;
            this.barcode = barcode;
        }
    }
}

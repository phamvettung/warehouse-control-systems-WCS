using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class ContainerRouteRequest : MessageBase
    {
        public string scannerName { get; set; }
        public string barcode { get; set; }

        public ContainerRouteRequest()
        {

        }

        public ContainerRouteRequest(string scannerName, string barcode)
        {
            this.scannerName = scannerName;
            this.barcode = barcode;
        }
    }
}

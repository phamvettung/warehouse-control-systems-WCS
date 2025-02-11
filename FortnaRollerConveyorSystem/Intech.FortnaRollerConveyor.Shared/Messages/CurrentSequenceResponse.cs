using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class CurrentSequenceResponse : MessageBase
    {
        public int currentSequenceNumber { get; set; }

        public CurrentSequenceResponse() { }
        public CurrentSequenceResponse(int currentSequenceNumber) { this.currentSequenceNumber = currentSequenceNumber; }
    }
}

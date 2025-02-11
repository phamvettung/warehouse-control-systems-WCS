using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Enums
{
    [Serializable]
    public enum StatusEnum
    {
        Connected,
        Disconnected,
        Started,
        Stopped,
        Ready,
        Busy,
        Error
    }
}

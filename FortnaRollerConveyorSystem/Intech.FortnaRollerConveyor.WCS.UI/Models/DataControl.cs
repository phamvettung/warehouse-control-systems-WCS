using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.WCS.UI.Models
{
    public class DataControl
    {
        public bool[] Stoppers { get; set; }

        public bool[] Pushers { get; set; }

        public bool[] Sensors { get; set; }

        public bool[] Motors { get; set; }

        public int NumOfData {  get; set; }

        public PlcEnum Mode { get; set; }    
        
        public int Running { get; set; }
        public int Pause { get; set; }
        public int Reset { get; set; }

        public int Emg00 { get; set; }
        public int Emg01 { get; set; }
        public int Emg02 { get; set; }


        public DataControl() 
        {
            NumOfData = 16;
            Stoppers = new bool[NumOfData];
            Pushers = new bool[NumOfData];
            Sensors = new bool[NumOfData];
            Motors = new bool[NumOfData];
            Mode = PlcEnum.NO_MODE;
            Running = -1;
            Pause = -1;
            Reset = -1;
            Emg00 = -1;
            Emg01 = -1;
            Emg02 = -1;
        }
    }
}

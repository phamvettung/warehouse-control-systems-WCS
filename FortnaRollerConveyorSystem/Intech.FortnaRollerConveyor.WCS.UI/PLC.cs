using Intech.FortnaRollerConveyor.Shared.Enums;
using Intech.FortnaRollerConveyor.WCS.UI.Configurations;
using Intech.FortnaRollerConveyor.WCS.UI.Logging;
using Intech.FortnaRollerConveyor.WCS.UI.Models;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Intech.FortnaRollerConveyor.WCS.UI
{
    public enum PlcEnum
    {
        NO_MODE = -1,
        ROUTING_MODE = 0,
        SORTING_MODE = 1,
        CLUSTER_01 = 2,
        CLUSTER_03A = 3,
        CLUSTER_03B = 4,
        CLUSTER_05 = 5,
        CLUSTER_07A = 6,
        CLUSTER_07B = 7,
        START = 8,
        STOP = 9,
        RESET = 10,
        PC_ON = 11,
        PC_OFF = 12
    }

    public class PLC
    {
        private ActUtlTypeLib.ActUtlTypeClass actUtlType;
        private PlcConfig config;
        public DataControl DataControl { get; set; }

        #region Properties
        public StatusEnum Status { get; private set; }
        public int IReturnCode { get; private set; } = -1;
        #endregion


        #region Delegates

        public delegate void DataControlDelegate(DataControl data);
        public static event DataControlDelegate OnDataControl;

        public delegate void Disconnection(int iReturnCode);
        public static event Disconnection OnClosed;

        #endregion


        #region Threads
        Thread readingThread;
        #endregion


        #region Contructors
        public PLC() 
        {
            actUtlType = new ActUtlTypeLib.ActUtlTypeClass();
            Status = StatusEnum.Disconnected;
            config = AppConfig.GetPlcConfig();
            DataControl = new DataControl();
        }
        #endregion


        #region Methods
        public void Open()
        {
            try
            {
                int iLogicalStationNumber;
                if (GetIntValue(config.StationNumber.ToString(), out iLogicalStationNumber) != true)//Convert stationNumberJson
                    return;
                actUtlType.ActLogicalStationNumber = iLogicalStationNumber;

                if (Status == StatusEnum.Connected)
                    return;
                IReturnCode = actUtlType.Open();
                if (IReturnCode == 0)
                {
                    Status = StatusEnum.Connected;
                    Start();
                    WriteReady(PlcEnum.PC_ON);
                }
                else
                {
                    Close();
                    Status = StatusEnum.Error;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                if (actUtlType != null)
                {
                    if (Status == StatusEnum.Disconnected)
                        return;
                    IReturnCode = actUtlType.Close();
                    if (IReturnCode == 0)
                    {
                        Status = StatusEnum.Disconnected;
                        WriteReady(PlcEnum.PC_OFF);
                    }
                    else
                    {
                        Status = StatusEnum.Error;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool GetIntValue(string sourceOfIntValue, out int iGottenIntValue)
        {
            try
            {
                iGottenIntValue = 0;
                try
                {
                    //Get the value as 32bit integer from a string
                    iGottenIntValue = Convert.ToInt32(sourceOfIntValue);
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;//Normal End
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        short[] data = new short[3]; int mode = -1, isRunning = -1, isPause = -1, isReset, emg00 = -1, emg01 = -1, emg02 = -1;
        private DataControl ReadData()
        {
            try
            {
                DataControl dataControl = new DataControl();
                IReturnCode = actUtlType.GetDevice(config.Running, out isRunning);
                IReturnCode = actUtlType.GetDevice(config.Pause, out isPause);
                IReturnCode = actUtlType.GetDevice(config.Reset, out isReset);
                dataControl.Running = isRunning;
                dataControl.Pause = isPause;
                dataControl.Reset = isReset;

                IReturnCode = actUtlType.GetDevice(config.ModeStatus, out mode);
                if (mode == (int)PlcEnum.ROUTING_MODE)
                    dataControl.Mode = PlcEnum.ROUTING_MODE;
                else if (mode == (int)PlcEnum.SORTING_MODE)
                    dataControl.Mode = PlcEnum.SORTING_MODE;

                IReturnCode = actUtlType.ReadDeviceBlock2(config.Stoppers, 1, out data[0]);
                IReturnCode = actUtlType.ReadDeviceBlock2(config.Pushers, 1, out data[1]);
                IReturnCode = actUtlType.ReadDeviceBlock2(config.Sensors, 1, out data[2]);
                dataControl.Stoppers = ConvertToArray(data[0]);
                dataControl.Pushers = ConvertToArray(data[1]);
                dataControl.Sensors = ConvertToArray(data[2]);

                IReturnCode = actUtlType.GetDevice(config.Emg00, out emg00);
                IReturnCode = actUtlType.GetDevice(config.Emg01, out emg01);
                IReturnCode = actUtlType.GetDevice(config.Emg02, out emg02);
                dataControl.Emg00 = emg00;
                dataControl.Emg01 = emg01;
                dataControl.Emg02 = emg02;

                return dataControl;
            }
            catch (Exception ex)
            {
                Status = StatusEnum.Error;
                throw ex;
            }
        }

        bool[] ConvertToArray(short @short)
        {
            try
            {
                var result = new bool[16];
                for (int i = 0; i < 16; i++)
                {
                    result[i] = (@short & (short)1) == (short)1 ? true : false;
                    @short = (short)(@short >> 1);
                }
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void WriteResultCluster(string value, PlcEnum clusterNumber)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(value);
                short[] arrDeviceValue = new short[buffer.Length];
                Buffer.BlockCopy(buffer, 0, arrDeviceValue, 0, buffer.Length);
                if (clusterNumber == PlcEnum.CLUSTER_01)
                    actUtlType.WriteDeviceBlock2(config.Cluster01, arrDeviceValue.Length, ref arrDeviceValue[0]);
                else if (clusterNumber == PlcEnum.CLUSTER_03A)
                    actUtlType.WriteDeviceBlock2(config.Cluster03A, arrDeviceValue.Length, ref arrDeviceValue[0]);
                else if (clusterNumber == PlcEnum.CLUSTER_03B)
                    actUtlType.WriteDeviceBlock2(config.Cluster03B, arrDeviceValue.Length, ref arrDeviceValue[0]);
                else if (clusterNumber == PlcEnum.CLUSTER_05)
                    actUtlType.WriteDeviceBlock2(config.Cluster05, arrDeviceValue.Length, ref arrDeviceValue[0]);
                else if (clusterNumber == PlcEnum.CLUSTER_07A)
                    actUtlType.WriteDeviceBlock2(config.Cluster07A, arrDeviceValue.Length, ref arrDeviceValue[0]);
                else if (clusterNumber == PlcEnum.CLUSTER_07B)
                    actUtlType.WriteDeviceBlock2(config.Cluster07B, arrDeviceValue.Length, ref arrDeviceValue[0]);
                else
                {
                    //Logger.WriteLog(string.Format("PC -> PLC | Arguments: value {0}, clusterNumber {1} | At Intech.FortnaRollerConveyor.WCS.UI.PLC - Line: 162 - Content: {2}", value, clusterNumber, "Arguments is invalid"), LogLevel.Error______, LogAction.WRITING);
                    throw new Exception("Arguments is invalid");
                }
 
                //Logger.WriteLog(string.Format("PC -> PLC | Arguments: value {0}, clusterNumber {1} | Content: {2}", value, clusterNumber, "Success"), LogLevel.Information, LogAction.WRITING);
                Debug.WriteLine(string.Format("{0} {1} {2} PC -> PLC | Arguments: value {3}, clusterNumber {4} | Content: {5}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.WRITING, value, clusterNumber, "Success"));
            }
            catch (Exception ex)
            {
                //Logger.WriteLog(string.Format("PC -> PLC | Arguments: value {0}, clusterNumber {1} | At Intech.FortnaRollerConveyor.WCS.UI.PLC - Line: 166 - Content: {2}", value, clusterNumber, ex.Message), LogLevel.Error______, LogAction.WRITING);
                Debug.WriteLine(string.Format("{0} {1} {2} PC -> PLC | Arguments: value {3}, clusterNumber {4} | At Intech.FortnaRollerConveyor.WCS.UI.PLC - Line: 166 - Content: {5}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Error______, LogAction.WRITING, value, clusterNumber, ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public void WriteSystem(PlcEnum options)
        {
            if(this.Status != StatusEnum.Connected)
            {
                return;
            }

            try
            {
                if (options == PlcEnum.START)
                {
                    actUtlType.SetDevice(config.StartSystem, 1);
                    Thread.Sleep(50);
                    actUtlType.SetDevice(config.StartSystem, 0);
                }
                else if (options == PlcEnum.STOP)
                {
                    actUtlType.SetDevice(config.StopSystem, 1);
                    Thread.Sleep(50);
                    actUtlType.SetDevice(config.StopSystem, 0);
                }
                else if (options == PlcEnum.RESET)
                {
                    actUtlType.SetDevice(config.ResetSystem, 1);
                    Thread.Sleep(50);
                    actUtlType.SetDevice(config.ResetSystem, 0);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modes">0: Routing mode, 1: Sorting mode</param>
        public void WriteMode(PlcEnum modes)
        {
            if (this.Status != StatusEnum.Connected)
            {
                return;
            }

            try
            {
                if (modes == PlcEnum.ROUTING_MODE)
                    actUtlType.SetDevice(config.ModeSystem, 0);
                else if (modes == PlcEnum.SORTING_MODE)
                    actUtlType.SetDevice(config.ModeSystem, 1);

                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void WriteReady(PlcEnum ready)
        {
            if (this.Status != StatusEnum.Connected)
            {
                return;
            }

            try
            {
                if(ready == PlcEnum.PC_ON)               
                    actUtlType.SetDevice(config.PcReady, 1);               
                else if(ready == PlcEnum.PC_OFF)
                    actUtlType.SetDevice(config.PcReady, 0);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


        #region Threads Methods
        private void Start()
        {
            try
            {
                readingThread = new Thread(DataReadingStream);
                readingThread.IsBackground = true;
                readingThread.Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void DataReadingStream()
        {
            try
            {
                while (Status == StatusEnum.Connected)
                {
                    this.DataControl = ReadData();
                    OnDataControl(this.DataControl);
                    if(IReturnCode != 0)
                    {
                        Status = StatusEnum.Error;
                        //Logger.WriteLog("Loss connection to PLC. " + String.Format("0x{0:x8}", IReturnCode).ToUpper(), LogLevel.Error______, LogAction.READING);
                        Close();
                    }
                    Thread.Sleep(30);
                }
                OnClosed(this.IReturnCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
    
    }
}

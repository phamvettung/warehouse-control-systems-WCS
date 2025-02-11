using Intech.FortnaRollerConveyor.Client;
using Intech.FortnaRollerConveyor.Shared.Enums;
using Intech.FortnaRollerConveyor.Shared.Messages;
using Intech.FortnaRollerConveyor.Shared.Timeout;
using Intech.FortnaRollerConveyor.WCS.UI.Configurations;
using Intech.FortnaRollerConveyor.WCS.UI.Logging;
using Intech.FortnaRollerConveyor.WCS.UI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.WCS.UI
{
    public class Grapher
    {
        #region Feilds
        /// <summary>
        /// Containers manager
        /// </summary>
        public int[,] MaxContainer; 
        public List<Models.Container>[,] Containers;

        /// <summary>
        /// Messages from the Server will not be processed immediately but will be added to the queue for later processing.
        /// </summary>
        private List<ResponseMessage> ResponseMessageQueue;
        #endregion


        #region Properties
        /// <summary>
        /// Status of grapher
        /// </summary>
        public StatusEnum Status { get; set; }

        /// <summary>
        /// Data control received from the PLC
        /// </summary>
        public DataControl DataControlReceived { get; set; }
        public DataControl DataControlActivated { get; set; }

        /// <summary>
        /// Status of the GTPs. To know that GTP station is READY or BUSY.
        /// </summary>
        public StatusEnum GTP_01_STATUS { get; set; }
        public StatusEnum GTP_02_STATUS { get; set; }

        /// <summary>
        /// List sequence number WES response.
        /// </summary>
        public List<Int32> SequenceNumberResponse { get; private set; }
        #endregion


        #region Delegates
        /// <summary>
        /// Show code result to the HomeForm.
        /// </summary>
        /// <param name="result"></param>
        public delegate void CodeResultDelegate(string result);
        public static event CodeResultDelegate CodeResult;

        /// <summary>
        /// Send log to the HomeForm
        /// </summary>
        public static event Client.Client.LogMessage OnLogReceivedMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="option">0: Retry, 1: Cancel</param>
        public delegate void ReconnectDelegate(int option);
        public static event ReconnectDelegate ReconnectTo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public delegate void ShowReady(string text, Color color);
        public static event ShowReady OnShowReady;

        public delegate void ShowMode(PlcEnum mode);
        public static event ShowMode OnShowMode;

        #endregion


        #region Threads

        private Thread dataControlThread = null;

        private Thread responseMessageThread = null;

        private Thread keepAliveThread = null;
        private bool keepAliveEnabled;
        private int keepAliveTimeout = 20000;
        private int keepAliveInterval = 1000;

        private Thread callingRequestThread = null;

        private Thread waitingForCommandThread = null;
        #endregion


        #region Contructors
        public Grapher()
        {
            InitializeValue();
            RegisterEvent();
        }

        private void RegisterEvent()
        {
            PLC.OnDataControl += PLC_OnDataControl;
            Client.Client.OnMessageReceived += Client_OnMessageReceived;
        }

        private void InitializeValue()
        {
            MaxContainer = new int[AppConfig.SIZE_OF_CONTAINER, AppConfig.SIZE_OF_CONTAINER];
            Containers = new List<Models.Container>[AppConfig.SIZE_OF_CONTAINER, AppConfig.SIZE_OF_CONTAINER];
            ResponseMessageQueue = new List<ResponseMessage>();
            SequenceNumberResponse = new List<Int32>();
            DataControlReceived = new DataControl();
            DataControlActivated = new DataControl();

            Status = StatusEnum.Stopped;
            GTP_01_STATUS = StatusEnum.Ready;
            GTP_02_STATUS = StatusEnum.Ready;

            for (int i = 0; i < AppConfig.SIZE_OF_CONTAINER; i++)
            {
                for (int j = 0; j < AppConfig.SIZE_OF_CONTAINER; j++)
                {
                    MaxContainer[i, j] = Int16.MinValue;
                    Containers[i, j] = new List<Models.Container>(AppConfig.SIZE_OF_CONTAINER);
                }
            }
            MaxContainer[0, 1] = 3;
            MaxContainer[1, 2] = 2; 
            MaxContainer[2, 3] = 3;
            MaxContainer[3, 4] = 2;
            MaxContainer[4, 5] = 2;
            MaxContainer[5, 6] = 3;
            MaxContainer[2, 7] = 4;
            MaxContainer[7, 8] = 2;
            MaxContainer[8, 9] = 3;
            MaxContainer[9, 10] = 2;
            MaxContainer[10, 11] = 2;
            MaxContainer[11, 12] = 3;
            MaxContainer[8, 13] = 4;

            for(int i = 0; i < DataControlReceived.NumOfData; i++)
            {
                DataControlActivated.Stoppers[i] = false;
                DataControlActivated.Pushers[i] = false;
                DataControlActivated.Sensors[i] = false;
                DataControlActivated.Motors[i] = false;
            }

            LoadContainer();
        }

        #endregion


        #region Processing DataControl

        public void StartDataControlProcessing()
        {
            Status = StatusEnum.Started;

            dataControlThread = new Thread(ProcessingDataControl);
            dataControlThread.IsBackground = true;
            dataControlThread.Start();

        }

        public void StartResponseMessageProcessing()
        {
            Status = StatusEnum.Started;

            responseMessageThread = new Thread(ProcessingResponseMessageQueue);
            responseMessageThread.IsBackground = true;
            responseMessageThread.Start();
        }

        /// <summary>
        /// Stop processing datacontrol
        /// </summary>
        public void Stop() 
        {
            Status = StatusEnum.Stopped;
        }

        /// <summary>
        /// Event received datacontrol from the PLC.
        /// </summary>
        /// <param name="data"></param>
        private void PLC_OnDataControl(DataControl data)
        {
            if (keepAliveEnabled) //When occur time out will not receive data from plc any more.
            {
                lock (data)
                {
                    DataControlReceived = data;
                }
            }
        }

        /// <summary>
        /// Processing the systems signal from the Datacontrol.
        /// </summary>
        /// <param name="obj"></param>
        private void ProcessingDataControl(object? obj)
        {
            while (Status == StatusEnum.Started)
            {
                for(int i = 0; i < DataControlReceived.NumOfData; i++)
                {
                    //STOPPERS
                    if (DataControlReceived.Stoppers[i] == false && DataControlActivated.Stoppers[i] == true)
                    {
                        DataControlActivated.Stoppers[i] = false;
                        switch(i)
                        {
                            case 0: //stopper 1 DOWN                              
                                MoveContainer(1, 2, 2, 7);                                                                       
                                break;
                            case 1: //stopper 2 DOWN 
                                MoveContainer(2, 3, 3, 4);
                                break;
                            case 2: //stopper 3 DOWN 
                                MoveContainer(3, 4, 4, 5);
                                CallRequest(new ContainerScanRequest(), 4, 5);
                                break;
                            case 3: //stopper 4 DOWN
                                MoveContainer(5, 6, 2, 7);
                                break;
                            case 4: //stopper 5 DOWN
                                MoveContainer(7, 8, 8, 13);
                                break;
                            case 5: //stopper 6 DOWN
                                MoveContainer(8, 9, 9, 10);
                                break;
                            case 6: //stopper 7 DOWN
                                MoveContainer(9, 10, 10, 11);
                                CallRequest(new ContainerScanRequest(), 10, 11);
                                break;
                            case 7: //stopper 8 DOWN
                                MoveContainer(11, 12, 8, 13);
                                break;
                            default: break;
                        }
                    }
                    else if(DataControlReceived.Stoppers[i] == true && DataControlActivated.Stoppers[i] == false)
                    {
                        DataControlActivated.Stoppers[i] = true;
                        switch (i)
                        {
                            case 0: //stopper 1 UP                              

                                break;
                            case 1: //stopper 2 UP 

                                break;
                            case 2: //stopper 3 UP 

                                break;
                            case 3: //stopper 4 UP

                                break;
                            case 4: //stopper 5 UP

                                break;
                            case 5: //stopper 6 UP

                                break;
                            case 6: //stopper 7 UP

                                break;
                            case 7: //stopper 8 UP

                                break;
                            default: break;
                        }
                    }

                    //PUSHERS
                    if (DataControlReceived.Pushers[i] == true && DataControlActivated.Pushers[i] == false)
                    {
                        DataControlActivated.Pushers[i] = true;
                        switch (i)
                        {
                            case 0: //pusher 1 UP
                                if (!DataControlReceived.Stoppers[1] && Containers[3, 4].Count < MaxContainer[3, 4]) // if stopper group 2 is ON that mean there is a waiting container at Pre-Op position
                                {
                                    MoveContainer(1, 2, 3, 4);
                                    CallRequest(new ContainerDivert(), 3, 4);

                                    if (GTP_01_STATUS == StatusEnum.Busy)
                                        for (int x = 0; x < Containers[3, 4].Count; x++)//injecting DeviceCommandRequest to container
                                            if (Containers[3, 4][x].DeviceId == AppConfig.GTP_01)
                                            {
                                                InjectContainer(3, 4, mem_ready_response_GTP01, x);
                                                GTP_01_STATUS = StatusEnum.Ready;
                                                break;
                                            }
                                }
                                else 
                                {
                                    MoveContainer(1, 2, 2, 3);
                                    CallRequest(new ContainerDivert(), 2, 3);

                                    //When have a new container divert to the GTP-01 and GTP-01 is ready, that mean i was traced but not found container
                                    if (GTP_01_STATUS == StatusEnum.Busy)
                                        for (int x = 0; x < Containers[2, 3].Count; x++)//injecting DeviceCommandRequest to the container
                                            if (Containers[2, 3][x].DeviceId == AppConfig.GTP_01) // check if the container's destination is GTP-01 already.
                                            {
                                                InjectContainer(2, 3, mem_ready_response_GTP01, x);
                                                GTP_01_STATUS = StatusEnum.Ready;
                                                break;
                                            }
                                }

                                break;
                            case 1: //pusher 2 UP

                                break;
                            case 2: //pusher 3 UP
                                MoveContainer(4, 5, 5, 6);
                                break;
                            case 3: //pusher 4 UP

                                break;
                            case 4: //pusher 5 UP
                                if (!DataControlReceived.Stoppers[5] && Containers[9, 10].Count < MaxContainer[9, 10])
                                {
                                    MoveContainer(7, 8, 9, 10);
                                    CallRequest(new ContainerDivert(), 9, 10);

                                    if (GTP_02_STATUS == StatusEnum.Busy)
                                        for (int x = 0; x < Containers[9, 10].Count; x++)//injecting DeviceCommandRequest to the container
                                            if (Containers[9, 10][x].DeviceId == AppConfig.GTP_02)
                                            {
                                                InjectContainer(9, 10, mem_ready_response_GTP02, x);
                                                GTP_02_STATUS = StatusEnum.Ready;
                                                break;
                                            }
                                }
                                else
                                {
                                    MoveContainer(7, 8, 8, 9);
                                    CallRequest(new ContainerDivert(), 8, 9);
                                    if (GTP_02_STATUS == StatusEnum.Busy)
                                        for (int x = 0; x < Containers[8, 9].Count; x++)//injecting DeviceCommandRequest to the container
                                            if (Containers[8, 9][x].DeviceId == AppConfig.GTP_02)
                                            {
                                                InjectContainer(8, 9, mem_ready_response_GTP02, x);
                                                GTP_02_STATUS = StatusEnum.Ready;
                                                break;
                                            }
                                }

                                break;
                            case 5: //pusher 6 UP

                                break;
                            case 6: //pusher 7 UP
                                MoveContainer(10, 11, 11, 12);
                                break;
                            case 7: //pusher 8 UP

                                break;
                            default: break;
                        }
                    }
                    else if(DataControlReceived.Pushers[i] == false && DataControlActivated.Pushers[i] == true)
                    {
                        DataControlActivated.Pushers[i] = false;
                        switch (i)
                        {
                            case 0: //pusher 1 UP                              

                                break;
                            case 1: //pusher 2 UP 

                                break;
                            case 2: //pusher 3 UP 

                                break;
                            case 3: //pusher 4 UP

                                break;
                            case 4: //pusher 5 UP

                                break;
                            case 5: //pusher 6 UP

                                break;
                            case 6: //pusher 7 UP

                                break;
                            case 7: //pusher 8 UP

                                break;
                            default: break;
                        }
                    }

                    //SENSORS
                    if (DataControlReceived.Sensors[i] == true && DataControlActivated.Sensors[i] == false)
                    {
                        DataControlActivated.Sensors[i] = true;
                        switch (i) {
                            case 0: //sensor X142 ON

                                break;
                            case 1: //sensor X145 ON
                                WriteResult(3, 4, PlcEnum.CLUSTER_03A, AppConfig.ADVANCE);
                                break;
                            case 2: //sensor X146
                                WriteResult(4, 5, PlcEnum.CLUSTER_03B, AppConfig.COMPLETE);
                                break;
                            case 3: //sensor X202 ON                        
                                                            
                                break;
                            case 4: //sensor X205 ON
                                WriteResult(9, 10, PlcEnum.CLUSTER_07A, AppConfig.ADVANCE);
                                break;
                            case 5: //sensor X206 ON
                                WriteResult(10, 11, PlcEnum.CLUSTER_07B, AppConfig.COMPLETE);
                                break;
                            case 6: //sensor scanner 01 ON
                                
                                break;
                            case 7: //sensor scanner 02 ON
                                
                                break;
                            default:
                                break;
                        }
                    }
                    else if(DataControlReceived.Sensors[i] == false && DataControlActivated.Sensors[i] == true)
                    {
                        DataControlActivated.Sensors[i] = false;
                        switch (i)
                        {
                            case 0: //sensor X142 OFF                             

                                break;
                            case 1: //sensor X145 OFF

                                break;
                            case 2: //sensor X146 OFF

                                break;
                            case 3: //sensor X202 OFF

                                break;
                            case 4: //sensor X205 OFF

                                break;
                            case 5: //sensor X206 OFF

                                break;
                            case 6: //sensor scanner 01 OFF

                                break;
                            case 7: //sensor scanner 02 OFF

                                break;
                            default: break;
                        }
                    }
                        
                }
     
                // OPERATING MODES
                if(DataControlReceived.Mode == PlcEnum.ROUTING_MODE && DataControlActivated.Mode == PlcEnum.SORTING_MODE || DataControlActivated.Mode == PlcEnum.NO_MODE)
                {
                    DataControlActivated.Mode = PlcEnum.ROUTING_MODE;
                    OnShowMode(PlcEnum.ROUTING_MODE);
                }
                else if(DataControlReceived.Mode == PlcEnum.SORTING_MODE && DataControlActivated.Mode == PlcEnum.ROUTING_MODE || DataControlActivated.Mode == PlcEnum.NO_MODE)
                {
                    DataControlActivated.Mode = PlcEnum.SORTING_MODE;
                    OnShowMode(PlcEnum.SORTING_MODE);
                }

                //RESET BIT
                if(DataControlReceived.Reset == 1 && DataControlActivated.Reset == 0)
                {
                    DataControlActivated.Reset = 1;
                    ResetContainer();
                }
                else if(DataControlReceived.Reset == 0)
                {
                    DataControlActivated.Reset = 0;
                }

                Thread.Sleep(30);
            }

        }

        /// <summary>
        /// moving container.
        /// </summary>
        /// <param name="si"></param>
        /// <param name="sj"></param>
        /// <param name="ei"></param>
        /// <param name="ej"></param>
        private void MoveContainer(int si, int sj, int ei, int ej)
        {
            if (Containers[si, sj].Count > 0 && Containers[ei, ej].Count < MaxContainer[ei, ej])
            {
                if (Containers[ei, ej].Count > 0 && (ei == 10 && ej== 11)/*OP GTP01*/ || (ei == 4 && ej == 5)/*OP GTP02*/ && DataControlReceived.Mode == PlcEnum.SORTING_MODE)
                {
                    Containers[ei, ej].Clear(); //or remove[0] because have condition count < max.
                }
                Models.Container container = Containers[si, sj][0];
                Containers[ei, ej].Add(container);
                Containers[si, sj].Remove(container);
            }
        }

        /// <summary>
        /// writing result to the PLC.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="clusterName"></param>
        /// <param name="senserId"></param>*
        private void WriteResult(int i, int j, PlcEnum clusterName, string valueToWrite = "")
        {
            if(Containers[i, j].Count > 0)
            {
                if(clusterName == PlcEnum.CLUSTER_01 || clusterName == PlcEnum.CLUSTER_05) //write if the device value is GTP01, GTP02, END
                {
                    if (Containers[i, j][0].Barcode == AppConfig.NG)
                        HomeForm.Plc.WriteResultCluster(Containers[i, j][0].Barcode, clusterName);
                    else
                        HomeForm.Plc.WriteResultCluster(Containers[i, j][0].DeviceId, clusterName);                     
                        
                }
                else if(DataControlReceived.Mode == PlcEnum.ROUTING_MODE)
                {
                    // write command ADVANCE, COMPLETE
                    if (Containers[i, j][0].Command == valueToWrite)
                        HomeForm.Plc.WriteResultCluster(Containers[i, j][0].Command, clusterName); 
                    else
                    {
                        waitingForCommandThread = new Thread(() =>
                        {
                            try
                            {
                                while (Containers[i, j].Count > 0)
                                {
                                    if (Containers[i, j].Count == 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (Containers[i, j][0].Command == valueToWrite)
                                        {
                                            HomeForm.Plc.WriteResultCluster(valueToWrite, clusterName);
                                            Containers[i, j][0].waitingCommand = false;
                                            break;
                                        }
                                        else
                                        {
                                            Containers[i, j][0].waitingCommand = true;
                                        }
                                    }
                                    Thread.Sleep(300);
                                }
                            }
                            catch (Exception ex)
                            {
                                string mes = string.Format("{0} Container: {1}, Count: {2}, Content: {4}", "waitingForCommandThread", i.ToString() + j.ToString(), ex.Message);
                                //Logger.WriteLog(ex.Message, LogLevel.Error______, LogAction.WRITING);
                                MessageBox.Show(mes, "WCS Error at WriteResult", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
               
                        });
                        waitingForCommandThread.IsBackground = true;
                        waitingForCommandThread.Start();
                    }
                }              
            }
            else
            {
                
            }
        }

        #endregion


        #region API functions

        /// <summary>
        /// Calling DivertConfirm API
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private int CallContainerDivertConfirm(int i, int j)
        {
            if (Containers[i, j].Count > 0)
            {
                ContainerDivert containerDivert = new ContainerDivert();
                containerDivert.scannerName = Containers[i, j][Containers[i, j].Count - 1].ScannerName;
                containerDivert.barcode = Containers[i, j][Containers[i, j].Count - 1].Barcode;
                containerDivert.deviceId = Containers[i, j][Containers[i, j].Count - 1].DeviceId;

                RequestMessage request = new RequestMessage();
                request.messageType = AppConfig.DIVERT_CONFIRM;
                request.data = containerDivert;

                return HomeForm.WesSocket.SendRequestMessage(request);
            }
            return 0;
        }

        /// <summary>
        /// Calling ContainerScan API
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        private int CallContainerScan(int i, int j)
        {
            if (Containers[i, j].Count > 0 && DataControlReceived.Mode == PlcEnum.ROUTING_MODE) //Just call the request if the mode is Routing
            {
                ContainerScanRequest containerScan = new ContainerScanRequest();
                containerScan.barcode = Containers[i, j][Containers[i, j].Count - 1].Barcode;
                containerScan.deviceId = Containers[i, j][Containers[i, j].Count - 1].DeviceId;

                RequestMessage request = new RequestMessage();
                request.messageType = AppConfig.CONTAINER_SCAN;
                request.data = containerScan;

                return HomeForm.WesSocket.SendRequestMessage(request);
            }
            return 0;
        }

        /// <summary>
        /// Calling ContainerRouteRequest
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="scannerName"></param>
        /// <returns>sequence number sent</returns>
        private int CallContainerRouteRequest(int i, int j, string scannerName)
        {
            if (Containers[i, j].Count > 0)
            {
                if (Containers[i, j][0].Barcode == AppConfig.NG)
                {
                    if (scannerName == AppConfig.SCANNER_01)
                    {
                        WriteResult(i, j, PlcEnum.CLUSTER_01);
                        MoveContainer(i, j, 1, 2); //0_1 -> 1_2 -> when stopper 1 down, move 1_2 -> 2_7
                    }
                    else if (scannerName == AppConfig.SCANNER_02)
                    {
                        WriteResult(i, j, PlcEnum.CLUSTER_05);
                        MoveContainer(i, j, 7, 8); //2 7 -> 7 8 -> when stopper 5 down, move 7_8 -> 8_13
                    }
                    return 0;
                }

                if (Containers[i, j][0].DeviceId == AppConfig.GTP_01 || Containers[i, j][0].DeviceId == AppConfig.GTP_02 || Containers[i, j][0].DeviceId == AppConfig.END || Containers[i, j][0].DeviceId == AppConfig.WAITING_TEXT)
                {
                    //request
                    ContainerRouteRequest containerRoute = new ContainerRouteRequest();
                    containerRoute.barcode = Containers[i, j][0].Barcode;
                    containerRoute.scannerName = scannerName;

                    RequestMessage request = new RequestMessage();
                    request.messageType = AppConfig.CONTAINER_ROUTE_REQUEST;
                    request.data = containerRoute;

                    if (Containers[i, j][0].DeviceId == AppConfig.GTP_01)
                        CodeResult(containerRoute.barcode);//show result to HomeForm

                    return HomeForm.WesSocket.SendRequestMessage(request); //return sequence number sent
                }
                else if (Containers[i, j][0].DeviceId == AppConfig.GTP_02 || Containers[i, j][0].DeviceId == AppConfig.END)
                {
                    WriteResult(i, j, PlcEnum.CLUSTER_05);
                    MoveContainer(i, j, 7, 8);
                }

            }
            return 0;
        }

        public int CallPingRequest()
        {
            RequestMessage request = new RequestMessage();
            request.messageType = AppConfig.PING_REQUEST;
            request.data = null;
            return HomeForm.WesSocket.SendRequestMessage(request);
        }

        public int CallCurrentSequenceRequest()
        {
            RequestMessage request = new RequestMessage();
            request.messageType = AppConfig.CURRENT_SEQUENCE_REQUEST;
            request.sequenceNumber = 0;
            request.data = null;
            return HomeForm.WesSocket.SendRequestMessage(request);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="scannerName"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private int TryCall(MessageBase msgType, int i, int j, string scannerName = "", int k = 0)
        {
            Type type = msgType.GetType();
            int previousSequenceNumber = -1;
            if (type == typeof(ContainerRouteRequest))
            {
                previousSequenceNumber = CallContainerRouteRequest(i, j, scannerName);
                if (previousSequenceNumber == 0) return 0;
            }
            else if (type == typeof(ContainerScanRequest))
            {
                previousSequenceNumber = CallContainerScan(i, j);
                if (previousSequenceNumber == 0) return 0;
            }
            else if (type == typeof(ContainerDivert))
            {
                previousSequenceNumber = CallContainerDivertConfirm(i, j);
                if (previousSequenceNumber == 0) return 0;
            }

            Thread.Sleep(wesConfig.RequestTimedout);
            if (this.SequenceNumberResponse.Contains(previousSequenceNumber))
            {
                this.SequenceNumberResponse.Remove(previousSequenceNumber);
                return previousSequenceNumber;
            }
            else if (k >= 2)
            {
                //Logger.WriteLog("WCS did not receive a response message from WES", LogLevel.Warning____, LogAction.SENDING);
                OnShowReady("WCS did not receive a response message from WES", Color.Orange);
                MessageBox.Show("WCS did not receive a response message from WES", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                TryCall(msgType, i, j, scannerName, k + 1);

            return 0;
        }


        /// <summary>
        /// Call the request below a thread
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="scannerName"></param>
        public void CallRequest(MessageBase msgType, int i, int j, string scannerName = "")
        {
            Type type = msgType.GetType();
            if ((type == typeof(ContainerScanRequest)) && (DataControlReceived.Mode == PlcEnum.SORTING_MODE))
            {
                return;
            }
            else
            {
                callingRequestThread = new Thread(() =>
                {
                    try
                    {
                        //Logger.WriteLog("", LogLevel.Information, LogAction.BEGINNING);
                        Debug.WriteLine(string.Format("{0} {1} {2}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.BEGINNING));
                        TryCall(msgType, i, j, scannerName);
                        Debug.WriteLine(string.Format("{0} {1} {2}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.END));
                        //Logger.WriteLog("", LogLevel.Information, LogAction.END);
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {

                    }
                });
                callingRequestThread.IsBackground = true;
                callingRequestThread.Start();
            }
        }

        #endregion


        #region Proccesing Messages Received

        /// <summary>
        /// Handle raw message (1st PROCESSING MESSAGE RECEIVED).
        /// </summary>
        /// <param name="msgReceived"></param>
        private void Client_OnMessageReceived(string msgReceived)
        {
            Logger.WriteLog(msgReceived, LogLevel.Information, LogAction.RECEIVING);
            Debug.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.RECEIVING, msgReceived));

            ResponseMessage responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(msgReceived);
            string dataConvStr = Convert.ToString(responseMessage.data); // json data
            if (responseMessage.messageType == AppConfig.CONTAINER_ROUTE_RESPONSE)
            {
                //Convert to object
                ContainerRouteResponse containerRouteResponse = JsonConvert.DeserializeObject<ContainerRouteResponse>(dataConvStr);
                responseMessage.data = containerRouteResponse; //reassign data
                ResponseMessageQueue.Add(responseMessage); //add to the response message queue
                OnLogReceivedMessage(responseMessage);
                //Add to the list sequence number
                this.SequenceNumberResponse.Add(responseMessage.sequenceNumber);

                //no need to response to WES.
            }
            else if (responseMessage.messageType == AppConfig.DEVICE_COMMAND)
            {
                DeviceCommandRequest deviceCommandRequest = JsonConvert.DeserializeObject<DeviceCommandRequest>(dataConvStr);
                responseMessage.data = deviceCommandRequest;
                ResponseMessageQueue.Add(responseMessage);
                OnLogReceivedMessage(responseMessage);

                //response to WES.
                DeviceStatusResponse deviceStatusResponse = new DeviceStatusResponse();
                deviceStatusResponse.status = deviceCommandRequest.command;
                deviceStatusResponse.deviceId = deviceCommandRequest.deviceId;
                ResponseMessage resp = new ResponseMessage();
                resp.messageType = AppConfig.DEVICE_STATUS;
                resp.sequenceNumber = responseMessage.sequenceNumber;
                resp.codeReturn = 0;
                resp.message = AppConfig.SUCCESS;
                resp.data = deviceStatusResponse;
                HomeForm.WesSocket.SendResponseMessage(resp);
            }
            else if (responseMessage.messageType == AppConfig.CONTAINER_SCAN_RESPONSE)
            {
                ContainerScanResponse containerScanResponse = JsonConvert.DeserializeObject<ContainerScanResponse>(dataConvStr);
                responseMessage.data = containerScanResponse;
                ResponseMessageQueue.Add(responseMessage);
                OnLogReceivedMessage(responseMessage);
                this.SequenceNumberResponse.Add(responseMessage.sequenceNumber);
            }
            else if (responseMessage.messageType == AppConfig.PONG_RESPONSE)
            {
                OnLogReceivedMessage(responseMessage);
                this.SequenceNumberResponse.Add(responseMessage.sequenceNumber);
            }
            else if (responseMessage.messageType == AppConfig.DIVERT_CONFIRM_RESPONSE)
            {
                OnLogReceivedMessage(responseMessage);
                this.SequenceNumberResponse.Add(responseMessage.sequenceNumber);
            }
            else if (responseMessage.messageType == AppConfig.CURRENT_SEQUENCE_RESPONSE)
            {
                CurrentSequenceResponse currentSequenceResponse = JsonConvert.DeserializeObject<CurrentSequenceResponse>(dataConvStr);
                responseMessage.data = currentSequenceResponse;
                ResponseMessageQueue.Add(responseMessage);
                OnLogReceivedMessage(responseMessage);
                this.SequenceNumberResponse.Add(responseMessage.sequenceNumber);
            }

        }

        /// <summary>
        /// Handle message received is a object from the ResponseMessageQueue (2nd PROCESSING MESSAGE RECEIVED)
        /// </summary>
        private void ProcessingResponseMessageQueue()
        {
            ResponseMessage responseMsg = new ResponseMessage();
            Type type = null;
            ContainerRouteResponse containerRouteResponse = null;
            DeviceCommandRequest deviceCommandRequest = null;
            ContainerScanResponse containerScanResponse = null;
            ContainerDivert containerDivertResponse = null;
            CurrentSequenceResponse currentSequenceResponse = null;

            while (Status == StatusEnum.Started)
            {
                if (ResponseMessageQueue.Count > 0)
                {
                    try
                    {
                        responseMsg = ResponseMessageQueue[0];
                        HomeForm.WesSocket.CurrentSequenceNumber = responseMsg.sequenceNumber;

                        type = responseMsg.data.GetType();
                        //======================STARTING HANDLE ContainerRouteResponse
                        if (type == typeof(ContainerRouteResponse))
                        {
                            containerRouteResponse = (ContainerRouteResponse)responseMsg.data;
                            if (containerRouteResponse.scannerName == AppConfig.SCANNER_01)
                            {
                                if (containerRouteResponse.deviceId == AppConfig.GTP_01)
                                {
                                    InjectContainer(0, 1, responseMsg);
                                    HomeForm.note_gtp_01 = true; //Effect
                                }
                                else if (containerRouteResponse.deviceId == AppConfig.GTP_02)
                                {
                                    InjectContainer(0, 1, responseMsg);
                                    HomeForm.note_gtp_02 = true;
                                }
                                else if (containerRouteResponse.deviceId == AppConfig.END)
                                {
                                    InjectContainer(0, 1, responseMsg);
                                    HomeForm.note_end = true;
                                }
                                MoveContainer(0, 1, 1, 2);//0...1 -> 1...2
                                WriteResult(1, 2, PlcEnum.CLUSTER_01, containerRouteResponse.deviceId);
                            }
                            else if (containerRouteResponse.scannerName == AppConfig.SCANNER_02)
                            {
                                if (containerRouteResponse.deviceId == AppConfig.GTP_01)
                                {
                                    InjectContainer(2, 7, responseMsg);
                                    HomeForm.note_gtp_01 = true; //Effect
                                }
                                else if (containerRouteResponse.deviceId == AppConfig.GTP_02)
                                {
                                    InjectContainer(2, 7, responseMsg);
                                    HomeForm.note_gtp_02 = true;
                                }
                                else if (containerRouteResponse.deviceId == AppConfig.END)
                                {
                                    InjectContainer(2, 7, responseMsg);
                                    HomeForm.note_end = true;
                                }
                                MoveContainer(2, 7, 7, 8); //2...7 -> 7...8
                                WriteResult(7, 8, PlcEnum.CLUSTER_05, containerRouteResponse.deviceId);
                            }
                        }
                        //======================STARTING HANDLE DeviceCommandRequest
                        else if (type == typeof(DeviceCommandRequest))
                        {
                            deviceCommandRequest = (DeviceCommandRequest)responseMsg.data;
                            //======================STARTING HANDLE DeviceCommandRequest GTP 01
                            if (deviceCommandRequest.deviceId == AppConfig.GTP_01)
                            {
                                if (deviceCommandRequest.command == AppConfig.ADVANCE)
                                {
                                    if (Containers[3, 4].Count == 0) //that mean the Pre-OP had not container. so need to trace container at the other locations on the conveyor
                                    {
                                        responseMsg.data = deviceCommandRequest;
                                        TraceContainer(3, 4, deviceCommandRequest.deviceId, responseMsg);
                                    }
                                    else 
                                    {
                                        InjectContainer(3, 4, responseMsg);
                                    }
                                    HomeForm.note_advance = true;
                                }
                                else if (deviceCommandRequest.command == AppConfig.COMPLETE)
                                {
                                    if (Containers[4, 5].Count > 0)//that mean the OP had container
                                    {
                                        InjectContainer(4, 5, responseMsg);
                                    }
                                    else
                                    {
                                        //NOP
                                    }
                                    HomeForm.note_complete = true;
                                }
                                else if (deviceCommandRequest.command == AppConfig.COMPLETEADVANCE)
                                {
                                    if ((Containers[3, 4].Count == 0 && Containers[4, 5].Count == 0) || (Containers[3, 4].Count > 0 && Containers[4, 5].Count == 0))
                                    {
                                        //need to trace the container at the other locations on the conveyor
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        TraceContainer(3, 4, deviceCommandRequest.deviceId, responseMsg);
                                    }
                                    else if (Containers[3, 4].Count == 0 && Containers[4, 5].Count > 0)
                                    {
                                        // container is in OP
                                        deviceCommandRequest.command = AppConfig.COMPLETE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(4, 5, responseMsg);

                                        //need to trace the container at the other locations on the conveyor
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        TraceContainer(3, 4, deviceCommandRequest.deviceId, responseMsg);
                                    }
                                    else if (Containers[3, 4].Count > 0 && Containers[4, 5].Count == 0)
                                    {
                                        // container is in Pre - OP
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(4, 5, responseMsg);
                                    }
                                    else if (Containers[3, 4].Count > 0 && Containers[4, 5].Count > 0)
                                    {
                                        // container is in Pre-OP
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(3, 4, responseMsg);

                                        // container is in OP
                                        deviceCommandRequest.command = AppConfig.COMPLETE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(4, 5, responseMsg);
                                    }

                                    HomeForm.note_complete = true;
                                    HomeForm.note_advance = true;
                                }
                            }
                            //======================STARTING HANDLE DeviceCommandRequest GTP 02
                            else if (deviceCommandRequest.deviceId == AppConfig.GTP_02)
                            {
                                if (deviceCommandRequest.command == AppConfig.ADVANCE)
                                {
                                    if (Containers[9, 10].Count == 0)
                                    {
                                        responseMsg.data = deviceCommandRequest;
                                        TraceContainer(9, 10, deviceCommandRequest.deviceId, responseMsg);
                                    }
                                    else
                                    {
                                        InjectContainer(9, 10, responseMsg);
                                    }
                                    HomeForm.note_advance = true;
                                }
                                else if (deviceCommandRequest.command == AppConfig.COMPLETE)
                                {
                                    if (Containers[10, 11].Count > 0)
                                    {
                                        InjectContainer(10, 11, responseMsg);
                                    }
                                    else 
                                    {
                                        //NOP
                                    }
                                    HomeForm.note_complete = true;
                                }
                                else if (deviceCommandRequest.command == AppConfig.COMPLETEADVANCE)
                                {
                                    if ((Containers[9, 10].Count == 0 && Containers[10, 11].Count == 0) || (Containers[9, 10].Count > 0 && Containers[10, 11].Count == 0))
                                    {
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        TraceContainer(9, 10, deviceCommandRequest.deviceId, responseMsg);
                                    }
                                    else if (Containers[9, 10].Count == 0 && Containers[10, 11].Count > 0)
                                    {
                                        deviceCommandRequest.command = AppConfig.COMPLETE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(10, 11, responseMsg);

                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        TraceContainer(9, 10, deviceCommandRequest.deviceId, responseMsg);
                                    }
                                    else if (Containers[9, 10].Count > 0 && Containers[10, 11].Count == 0)
                                    {
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(10, 11, responseMsg);
                                    }
                                    else if (Containers[9, 10].Count > 0 && Containers[10, 11].Count > 0)
                                    {
                                        deviceCommandRequest.command = AppConfig.ADVANCE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(9, 10, responseMsg);
                                        deviceCommandRequest.command = AppConfig.COMPLETE;
                                        responseMsg.data = deviceCommandRequest;
                                        InjectContainer(10, 11, responseMsg);
                                    }
                                    HomeForm.note_complete = true;
                                    HomeForm.note_advance = true;
                                }
                            }
                        }
                        //======================STARTING HANDLE ContainerScanResponse
                        else if (type == typeof(ContainerScanResponse))
                        {
                            containerScanResponse = (ContainerScanResponse)responseMsg.data;
                        }
                        //======================STARTING HANDLE ContainerDivert
                        else if (type == typeof(ContainerDivert))
                        {
                            containerDivertResponse = (ContainerDivert)responseMsg.data;
                        }
                        //======================STARTING HANDLE CurrentSequenceResponse
                        else if (type == typeof(CurrentSequenceResponse))
                        {
                            currentSequenceResponse = (CurrentSequenceResponse)responseMsg.data;
                            HomeForm.WesSocket.CurrentSequenceNumber = currentSequenceResponse.currentSequenceNumber;
                            if (wesConfig.EnableKeepAlive == 1)
                                SetKeepAliveOption(true, wesConfig.KeepAliveTimeout, wesConfig.KeepAliveInterval);
                            else
                                SetKeepAliveOption(false, wesConfig.KeepAliveTimeout, wesConfig.KeepAliveInterval);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("ProcessingResponseMessageQueue" + e.Message);
                    }
                    finally
                    {
                        ResponseMessageQueue.Remove(responseMsg);
                    }
                }

                Thread.Sleep(30);
            }

        }

        /// <summary>
        /// To save the ResponseMessage.
        /// </summary>
        ResponseMessage mem_ready_response_GTP01, mem_ready_response_GTP02;
        /// <summary>
        /// Trường hợp WES gửi DeviceCommand trước khi container ỏ vị trí Pre-OP và trước cả khi container được đặt lên băng tải
        /// Cần phải tìm container ở những vị trí khác trên băng tải, nếu thấy container sẽ injecting response cho container đó
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="device"></param>
        /// <param name="msgReceived">Là message từ server trả về, có thể là Request hoặc Response. Lớp ResponseMessage có thể được dùng như 1 lớp RequestMessage</param>
        /// <returns>-1 if dose not find any containers </returns>
        private int TraceContainer(int i, int j, string device, ResponseMessage msgReceived)
        {
            for (int x = i; x >= 0; x--)
                for(int y = j; y >= 0; y--)
                {
                    if (Containers[x,y].Count > 0)
                        if(Containers[x, y][0].DeviceId == device)
                        {
                            //Trường hợp tìm thấy container trên băng tải
                            InjectContainer(x, y, msgReceived);
                            return 0;
                        }
                }

            //Trường hợp không tìm thấy container
            if (device == AppConfig.GTP_01)
            {
                GTP_01_STATUS = StatusEnum.Busy;
                mem_ready_response_GTP01 = msgReceived;
            }
            else if(device == AppConfig.GTP_02)
            {
                GTP_02_STATUS = StatusEnum.Busy;
                mem_ready_response_GTP02 = msgReceived;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="response"></param>
        /// <param name="index">index of container to assign</param>
        private void InjectContainer(int i, int j, ResponseMessage response, int index = 0)
        {
            if (Containers[i, j].Count == 0)
            {
                return;
            }

            Type type = response.data.GetType();
            if (type == typeof(ContainerRouteResponse))
            {
                ContainerRouteResponse data = (ContainerRouteResponse)response.data;
                Containers[i, j][index].ResponseMessages.Add(response);
                Containers[i, j][index].MessageType = response.messageType;
                Containers[i, j][index].SequenceNumber = response.sequenceNumber;
                Containers[i, j][index].CodeReturn = response.codeReturn;
                Containers[i, j][index].Message = response.message;
                Containers[i, j][index].DeviceId = data.deviceId;
                Containers[i, j][index].Barcode = data.barcode;
                Containers[i, j][index].ScannerName = data.scannerName;
                Containers[i, j][index].Flag = ContainerFlag.DeviceId;
            }
            else if (type == typeof(DeviceCommandRequest))
            {
                DeviceCommandRequest data = (DeviceCommandRequest)response.data;
                Containers[i, j][index].ResponseMessages.Add(response);
                Containers[i, j][index].MessageType = response.messageType;
                Containers[i, j][index].CodeReturn = response.codeReturn;
                Containers[i, j][index].Message = response.message;
                Containers[i, j][index].Command = data.command;
                Containers[i, j][index].DeviceId = data.deviceId;
                Containers[i, j][index].Flag = ContainerFlag.Command;
            }
            else if(type == typeof(ContainerScanResponse))
            {
                ContainerScanResponse data = (ContainerScanResponse)response.data;
                Containers[i, j][index].ResponseMessages.Add(response);
                Containers[i, j][index].MessageType = response.messageType;
                Containers[i, j][index].SequenceNumber = response.sequenceNumber;
                Containers[i, j][index].CodeReturn = response.codeReturn;
                Containers[i, j][index].Message = response.message;
                Containers[i, j][index].DeviceId = data.deviceId;
            }
        }

        #endregion


        #region Keep alive 
        WesConfig wesConfig = AppConfig.GetWESConfig();
        public void SetKeepAliveOption(bool enabled, int timeout, int interval)
        {
            if (timeout < 0)
                throw new ArgumentException("must be zero or more", "timeout");

            if (interval < 1)
                throw new ArgumentException("must be 1 or more", "interval");

            bool keepAliveEnable = keepAliveEnabled;
            keepAliveEnabled = enabled;
            keepAliveTimeout = timeout;
            keepAliveInterval = interval;
            lock (this)
            {
                if (HomeForm.WesSocket.Status == StatusEnum.Connected && !keepAliveEnable && keepAliveEnabled)
                {
                    keepAliveThread = new Thread(() =>
                    {
                        KeepAliveFunc();
                    });
                    keepAliveThread.IsBackground = true;
                    keepAliveThread.Name = "KeepAlive Thread";
                    keepAliveThread.Start();
                }
            }
        }

        private void KeepAliveFunc()
        {
            Func<bool> theFunction = ExecutionPingRequest;
            TimeSpan timeout = TimeSpan.FromMilliseconds(keepAliveTimeout);
            Func<bool> onTimeout = () =>
            {
                keepAliveEnabled = false;
                return false;
            };

            bool success = false;
            while (HomeForm.WesSocket.Status == StatusEnum.Connected && keepAliveEnabled)
            {
                try
                {
                    success = TimedExecution<bool>.Execute(theFunction, timeout, onTimeout);
                    Debug.WriteLine($"{nameof(KeepAliveFunc)}: success={success}");
                    if(success) Thread.Sleep(keepAliveInterval);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("KeepAliveThread", "Caught " + ex.Message + ". Thread now terminates.");
                    break;
                }
            }

            if (HomeForm.WesSocket.Status == StatusEnum.Connected)
            {
                OnShowReady("Lost connection to WES.", Color.Red);
                ReconnectTo(0);
            }
        }

        private bool ExecutionPingRequest()
        {
            int sequenceNumberResponse = -1;
            sequenceNumberResponse = CallPingRequest();
            while (!SequenceNumberResponse.Contains(sequenceNumberResponse) && keepAliveEnabled)
            {

            }
            SequenceNumberResponse.Remove(sequenceNumberResponse);
            return true;
        }

        #endregion


        #region Save, load, reset container into the settings
        public void SaveContainer()
        {
            try
            {
                Properties.Settings.Default.Containers01.Clear();
                for (int k = 0; k < Containers[0, 1].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[0, 1][k]);
                    Properties.Settings.Default.Containers01.Add(containerJson);
                }

                Properties.Settings.Default.Containers12.Clear();
                for (int k = 0; k < Containers[1, 2].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[1, 2][k]);
                    Properties.Settings.Default.Containers12.Add(containerJson);
                }

                Properties.Settings.Default.Containers23.Clear();
                for (int k = 0; k < Containers[2, 3].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[2, 3][k]);
                    Properties.Settings.Default.Containers23.Add(containerJson);
                }

                Properties.Settings.Default.Containers34.Clear();
                for (int k = 0; k < Containers[3, 4].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[3, 4][k]);
                    Properties.Settings.Default.Containers34.Add(containerJson);
                }

                Properties.Settings.Default.Containers45.Clear();
                for (int k = 0; k < Containers[4, 5].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[4, 5][k]);
                    Properties.Settings.Default.Containers45.Add(containerJson);
                }

                Properties.Settings.Default.Containers27.Clear();
                for (int k = 0; k < Containers[2, 7].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[2, 7][k]);
                    Properties.Settings.Default.Containers27.Add(containerJson);
                }

                Properties.Settings.Default.Containers78.Clear();
                for (int k = 0; k < Containers[7, 8].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[7, 8][k]);
                    Properties.Settings.Default.Containers78.Add(containerJson);
                }

                Properties.Settings.Default.Containers89.Clear();
                for (int k = 0; k < Containers[8, 9].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[8, 9][k]);
                    Properties.Settings.Default.Containers89.Add(containerJson);
                }

                Properties.Settings.Default.Containers910.Clear();
                for (int k = 0; k < Containers[9, 10].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[9, 10][k]);
                    Properties.Settings.Default.Containers910.Add(containerJson);
                }


                Properties.Settings.Default.Containers1011.Clear();
                for (int k = 0; k < Containers[10, 11].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[10, 11][k]);
                    Properties.Settings.Default.Containers1011.Add(containerJson);
                }

                Properties.Settings.Default.Containers1112.Clear();
                for (int k = 0; k < Containers[11, 12].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[11, 12][k]);
                    Properties.Settings.Default.Containers1112.Add(containerJson);
                }

                Properties.Settings.Default.Containers813.Clear();
                for (int k = 0; k < Containers[8, 13].Count; k++)
                {
                    string containerJson = JsonConvert.SerializeObject(Containers[8, 13][k]);
                    Properties.Settings.Default.Containers813.Add(containerJson);
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save container: " + ex.Message);
            }
        }

        private void LoadContainer()
        {
            try
            {
                for (int k = 0; k < Properties.Settings.Default.Containers01.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers01[k]);
                    Containers[0, 1].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers12.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers12[k]);
                    Containers[1, 2].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers23.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers23[k]);
                    Containers[2, 3].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers34.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers34[k]);
                    Containers[3, 4].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers45.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers45[k]);
                    Containers[4, 5].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers27.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers27[k]);
                    Containers[2, 7].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers78.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers78[k]);
                    Containers[7, 8].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers89.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers89[k]);
                    Containers[8, 9].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers910.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers910[k]);
                    Containers[9, 10].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers1011.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers1011[k]);
                    Containers[10, 11].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers1112.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers1112[k]);
                    Containers[11, 12].Add(container);
                }
                for (int k = 0; k < Properties.Settings.Default.Containers813.Count; k++)
                {
                    Models.Container container = JsonConvert.DeserializeObject<Models.Container>(Properties.Settings.Default.Containers813[k]);
                    Containers[8, 13].Add(container);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load container: " + ex.Message);
            }
        }

        public void ResetContainer()
        {
            for(int i = 0; i < AppConfig.SIZE_OF_CONTAINER; i++)
                for(int j = 0; j < AppConfig.SIZE_OF_CONTAINER; j++)
                    if (Containers[i, j].Count > 0)
                    {
                        Containers[i, j].Clear();
                    }
        }


        #endregion
    
    }
}
    
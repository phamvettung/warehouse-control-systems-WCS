using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Intech.FortnaRollerConveyor.Shared.Enums;
using Intech.FortnaRollerConveyor.Shared.Messages;
using Intech.FortnaRollerConveyor.WCS.UI.Logging;
using Newtonsoft.Json;

namespace Intech.FortnaRollerConveyor.Client
{
    public class Client
    {
        private Thread receivingThread;
        private Thread sendingThread;

        private NetworkStream networkStream;

        #region Properties
        public TcpClient TcpClient { get; set; }
        public String Address { get; private set; }
        public int Port { get; private set; }
        /// <summary>
        /// 0: WES, 1: Scanner01, 2: Scanner02
        /// </summary>
        public int DeviceType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public StatusEnum Status { get; private set; }
        /// <summary>
        /// List message to send
        /// </summary>
        public List<String> MessageQueue { get; private set; }

        public int CurrentSequenceNumber { get; set; }
        #endregion


        #region Delegate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="scannerNumber"></param>
        public delegate void CodeResultDelegate(string result, int scannerNumber);
        public static event CodeResultDelegate CodeResult;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgReceived"></param>
        public delegate void MessageReceivedDelegate(string msgReceived);
        public static event MessageReceivedDelegate OnMessageReceived;

        /// <summary>
        /// Write log
        /// </summary>
        /// <param name="msg"></param>
        public delegate void LogMessage(MessageBase msg);
        public static event LogMessage OnLogSentMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public delegate void Disconnection(Client client);
        public static event Disconnection OnDisconnect;
        #endregion


        #region Contructors
        public Client()
        {
            MessageQueue = new List<String>();
            Status = StatusEnum.Disconnected;
            this.CurrentSequenceNumber = -1;
        }
        #endregion



        #region Methods
        public void Connect(String address, int port, int deviceType)
        {
            try
            {
                if (Status == StatusEnum.Connected) return;

                Address = address;
                Port = port;
                DeviceType = deviceType;
                TcpClient = new TcpClient();
                TcpClient.Connect(Address, Port);
                Status = StatusEnum.Connected;
                TcpClient.ReceiveBufferSize = 1024;
                TcpClient.SendBufferSize = 1024;

                networkStream = TcpClient.GetStream();

                receivingThread = new Thread(ReceivingMethod);
                receivingThread.IsBackground = true;
                receivingThread.Start();

                sendingThread = new Thread(SendingMethod);
                sendingThread.IsBackground = true;
                sendingThread.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Disconnect()
        {
            if (Status == StatusEnum.Disconnected) return;
            try
            {
                MessageQueue.Clear();
                Status = StatusEnum.Disconnected;
                TcpClient.Client.Disconnect(false);
                networkStream.Close();
                TcpClient.Close();     
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


        #region Threads methods
        private void SendingMethod(object? obj)
        {
            byte[] buffer;
            string message;
            while (Status != StatusEnum.Disconnected)
            {
                if (MessageQueue.Count > 0)
                {
                    message = MessageQueue[0];
                    try
                    {
                        buffer = Encoding.ASCII.GetBytes(message);
                        networkStream.BeginWrite(buffer, 0, buffer.Length, CallBack, null);
                    }
                    catch
                    {
                        Disconnect();
                    }
                    finally
                    {
                        MessageQueue.Remove(message);
                    }
                }
                Thread.Sleep(30);
            }
        }

        private void CallBack(IAsyncResult ar)
        {
            
        }

        public void SendResponseMessage(ResponseMessage response)
        {
            OnLogSentMessage(response);
            string message = JsonConvert.SerializeObject(response);
            char stx = '\u0002';
            char etx = '\u0003';
            Logger.WriteLog(message, LogLevel.Information, LogAction.SENDING);
            Debug.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.SENDING, message));
            MessageQueue.Add(string.Format("{0}{1}{2}", stx, message, etx));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">Return sequence number requested</param>
        /// <returns>sequence number</returns>
        public int SendRequestMessage(RequestMessage request)
        {
            this.CurrentSequenceNumber++;
            request.sequenceNumber = this.CurrentSequenceNumber;

            OnLogSentMessage(request);
            string message = JsonConvert.SerializeObject(request);
            char stx = '\u0002';
            char etx = '\u0003';
            Logger.WriteLog(message, LogLevel.Information, LogAction.SENDING);
            Debug.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.SENDING, message));
            MessageQueue.Add(string.Format("{0}{1}{2}", stx, message, etx));

            int previousSequenceNumber = CurrentSequenceNumber;
            if (CurrentSequenceNumber >= Math.Pow(2, 15) - 1) CurrentSequenceNumber = 1;

            return previousSequenceNumber;
        }

        private void ReceivingMethod(object? obj)
        {
            string msgQueue = string.Empty;
            byte[] buffer = new byte[TcpClient.ReceiveBufferSize];
            string msgReceiv = string.Empty;
            int ret = 0;
            while (Status != StatusEnum.Disconnected) 
            {
                try
                {
                    if (TcpClient.Available > 0)
                    {
                        ret = networkStream.Read(buffer, 0, TcpClient.Available);
                        msgReceiv = Encoding.ASCII.GetString(buffer).Trim('\0');
                        msgQueue = string.Concat(msgQueue, msgReceiv);
                        Array.Clear(buffer, 0, ret);
                        HandleMessageQueue(ref msgQueue);
                    }

                    if (TcpClient.Client.Poll(1, SelectMode.SelectRead) && !networkStream.DataAvailable)
                    {
                        Disconnect();
                        OnDisconnect(this);
                    }

                    Thread.Sleep(30);
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("ReceivingMethod " + ex.Message, LogLevel.Error______, LogAction.RECEIVING);
                    throw ex;
                }
            }
        }

        private void HandleMessageQueue(ref string msgQueue)
        {
            int lastEtx = msgQueue.LastIndexOf('\u0003');
            if (lastEtx > 0)
            {
                string resMessage = msgQueue.Substring(0, lastEtx + 1);
                msgQueue = msgQueue.Remove(0, lastEtx + 1);
                string[] resMessageArr = resMessage.Split(new char[] { '\u0002', '\u0003' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    for (int i = 0; i < resMessageArr.Length; i++)
                    {
                        switch (this.DeviceType) {
                            case 0: //WES
                                //Logger.WriteLog(resMessageArr[i], LogLevel.Information, LogAction.RECEIVING);
                                //Debug.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("HH:mm:ss"), LogLevel.Information, LogAction.RECEIVING, resMessageArr[i]));
                                OnMessageReceived(resMessageArr[i]);                         
                                break;
                            case 1: // Scanner 01
                                CodeResult(resMessageArr[i], 1);
                                break;
                            case 2: // Scanner 02
                                CodeResult(resMessageArr[i], 2);
                                break;
                            default:
                                break;
                        }

                    }
                }
                catch (Newtonsoft.Json.JsonReaderException e)
                {
                    Debug.WriteLine("Can not deserialize the request message." + e.Message);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Not use
        /// </summary>
        /// <param name="msgQueue"></param>
        private void HandleCodeResultQueue(ref string msgQueue)
        {
            int lastEtx = msgQueue.LastIndexOf("\\n");
            if (lastEtx > 0)
            {
                string resMessage = msgQueue.Substring(0, lastEtx + 2);
                msgQueue = msgQueue.Remove(0, lastEtx + 2);
                string[] resMessageArr = resMessage.Split("\\r\\n", StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    for (int i = 0; i < resMessageArr.Length; i++)
                    {
                        CodeResult(resMessageArr[i], 1);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
        #endregion

    }
}

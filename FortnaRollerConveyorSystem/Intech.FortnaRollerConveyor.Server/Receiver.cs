using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Intech.FortnaRollerConveyor.Shared.Enums;
using Intech.FortnaRollerConveyor.Shared.Messages;
using Newtonsoft.Json;
using static Intech.FortnaRollerConveyor.Server.Receiver;
namespace Intech.FortnaRollerConveyor.Server
{
    public class Receiver
    {
        private Thread receivingThread;
        private Thread sendingThread;
        private Thread checkingConnectsThread;
 
        private NetworkStream networkStream;
        private int sequenceNumber;

        #region Properties
        /// <summary>
        /// The receiver unique id.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The reference to the parent server.
        /// </summary>
        public Server Server { get; set; }
        /// <summary>
        /// The real client working in the background. 
        /// </summary>
        public TcpClient TcpClient { get; set; }
        /// <summary>
        /// The current status of the reciever instance.
        /// </summary>
        public StatusEnum Status { get; set; }
        /// <summary>
        /// The message queue that contains all the messages to deliver to the remote client.
        /// </summary>
        public List<String> MessageQueue { get; private set; }
        #endregion

        #region Delegates

        public delegate void ClientConnectedDelegate(Receiver receiver);
        public static event ClientDiconnectedDelegate OnClientConnected;
        public delegate void ClientDiconnectedDelegate(Receiver receiver);
        public static event ClientDiconnectedDelegate OnClientDisconnected;
        public delegate void ClientSentMessage(Receiver receiver, string msgReceived);
        public static event ClientSentMessage OnClientSentMessage;
        public delegate void MessageReceivedDelegate(MessageBase data, ResponseMessage header);
        public static event MessageReceivedDelegate MessageReceived;

        #endregion

        #region Contructors
        /// <summary>
        /// Initializes a new receiver instance
        /// </summary>
        /// <param name="client"></param>
        /// <param name="server"></param>
        public Receiver(TcpClient client, Server server)
        {
            TcpClient = client;
            Server = server;
            TcpClient.ReceiveBufferSize = 1024;
            TcpClient.SendBufferSize = 1024;

            networkStream = TcpClient.GetStream();
            sequenceNumber = 1;

            Id = Guid.NewGuid();
            MessageQueue = new List<String>();
            Status = StatusEnum.Connected;
            OnClientConnected(this);
        }

        #endregion

        #region Threads Methods
        public void Start()
        {
            receivingThread = new Thread(ReceivingMethod);
            receivingThread.IsBackground = true;
            receivingThread.Start();

            sendingThread = new Thread(SendingMethod);
            sendingThread.IsBackground = true;
            sendingThread.Start();

        }

        public void Disconnect()
        {
            if (Status == StatusEnum.Disconnected) return;
            
            Status = StatusEnum.Disconnected;
            OnClientDisconnected(this);
            TcpClient.Client.Disconnect(false);
            TcpClient.Close();
        }

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
                        networkStream.BeginWrite(buffer, 0, buffer.Length, WriteCallBack, null);
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

        private void WriteCallBack(IAsyncResult ar)
        {
           
        }

        private void SendResponseMessage(ResponseMessage response)
        {
            string message = string.Empty;
            if(response != null)
                message = JsonConvert.SerializeObject(response);
            char stx = '\u0002';
            char etx = '\u0003';
            MessageQueue.Add(string.Format("{0}{1}{2}", stx, message, etx));
        }

        private void SendRequestMessage(RequestMessage request)
        {
            string message = string.Empty;
            if (request != null)
                message = JsonConvert.SerializeObject(request);
            char stx = '\u0002';
            char etx = '\u0003';
            MessageQueue.Add(string.Format("{0}{1}{2}", stx, message, etx));
        }

        private void ReceivingMethod(object? obj)
        {
            string msgQueue = string.Empty;
            byte[] buffer = new byte[TcpClient.ReceiveBufferSize];
            string msgReceiv = string.Empty;
            while (Status != StatusEnum.Disconnected)
            {
                if(TcpClient.Available > 0)
                {
                    try
                    {
                        networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallBack, null);
                        msgReceiv = Encoding.ASCII.GetString(buffer).Trim('\0');
                        msgQueue = string.Concat(msgQueue, msgReceiv);
                        Array.Clear(buffer, 0, buffer.Length);
                        HandleMessageQueue(ref msgQueue);
                    }
                    catch (Exception ex)
                    {
                        Disconnect();
                        Debug.WriteLine(ex.Message);
                    }
                }

                if (TcpClient.Client.Poll(1, SelectMode.SelectRead) && !networkStream.DataAvailable)
                {
                    Disconnect();
                    Debug.WriteLine("This Socket is close.");
                }

                Thread.Sleep(1000);
            }
        }

        private void ReadCallBack(IAsyncResult ar)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgQueue"></param>
        /// <returns></returns>
        private void HandleMessageQueue(ref string msgQueue)
        {
            int lastEtx = msgQueue.LastIndexOf('\u0003');
            if (lastEtx > 0)
            {
                string reqMessage = msgQueue.Substring(0, lastEtx + 1);
                msgQueue = msgQueue.Remove(0, lastEtx + 1);
                string[] reqMessageArr = reqMessage.Split(new char[] { '\u0002', '\u0003' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    for (int i = 0; i < reqMessageArr.Length; i++)
                    {
                        //send to the UI
                        OnClientSentMessage(this, reqMessageArr[i]);
                        ResponseMessage msgReceived = JsonConvert.DeserializeObject<ResponseMessage>(reqMessageArr[i]);
                        OnMessageReceived(msgReceived);
                    }
                }
                catch(Newtonsoft.Json.JsonReaderException e)
                {
                    Debug.WriteLine("Can not deserialize the request message." + e.Message);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

            }
        }

        #endregion


        private void OnMessageReceived(ResponseMessage msgReceived)
        {
            string dataConvStr = Convert.ToString(msgReceived.data);
            switch (msgReceived.messageType)
            {
                case "ContainerRouteRequest":
                    ContainerRouteRequest containerRouteRequest = JsonConvert.DeserializeObject<ContainerRouteRequest>(dataConvStr);
                    OnResponseMessage(containerRouteRequest, msgReceived);
                    break;           
                case "ContainerScan":
                    ContainerScanRequest containerScanRequest = JsonConvert.DeserializeObject<ContainerScanRequest>(dataConvStr);
                    OnResponseMessage(containerScanRequest, msgReceived);
                    break;
                case "Ping":
                    OnResponseMessage(new PingRequest(), msgReceived);
                    break;
                case "DivertConfirm":
                    OnResponseMessage(new ContainerDivert(), msgReceived);
                    break;
                case "CurrentSequenceRequest":
                    OnResponseMessage(new CurrentSequenceRequest(), msgReceived);
                    break;
                default:
                    break;
            }
        }

        public void OnRequestMessage(MessageBase msg)
        {
            RequestMessage request = new RequestMessage();

            Type type = msg.GetType();
            if (type == typeof(PingRequest))
            {
                request.messageType = "Ping";
                request.sequenceNumber = sequenceNumber;
                request.data = null;
            }
            else if (type == typeof(ContainerRouteRequest))
            {
                ContainerRouteRequest containerRouteRequest = (ContainerRouteRequest)msg;
                request.messageType = "ContainerRouteRequest";
                request.sequenceNumber = sequenceNumber;
                request.data = containerRouteRequest;
            }
            else if (type == typeof(DeviceCommandRequest))
            {
                DeviceCommandRequest deviceCommandRequest = (DeviceCommandRequest)msg;
                request.messageType = "DeviceCommand";
                request.sequenceNumber = sequenceNumber;
                request.data = deviceCommandRequest;
            }
            else if (type == typeof(ContainerScanRequest))
            {
                ContainerScanRequest containerScanRequest = (ContainerScanRequest)msg;
                request.messageType = "ContainerScan";
                request.sequenceNumber = sequenceNumber;
                request.data = containerScanRequest;
            }

            SendRequestMessage(request);
            sequenceNumber++;
            if (sequenceNumber >= Math.Pow(2, 15) - 1) sequenceNumber = 1;
        }

        string mem_gtp = string.Empty;
        public void OnResponseMessage(MessageBase data, ResponseMessage header)
        {
            ResponseMessage response = new ResponseMessage();
            response.sequenceNumber = header.sequenceNumber;

            Type type = data.GetType();
            if (type == typeof(ContainerRouteRequest))
            {
                //Request
                ContainerRouteRequest containerRouteRequest = (ContainerRouteRequest)data;
                //Handle
                string gtpName = string.Empty;
                if (containerRouteRequest.barcode == "115007000BK" || containerRouteRequest.barcode == "115007000FR")
                {
                    gtpName = "GTP-01";
                    //if (mem_gtp != string.Empty)
                    //{
                    //    gtpName = "GTP-02";
                    //}
                    //else
                    //{
                    //    gtpName = "GTP-01";
                    //    mem_gtp = gtpName;
                    //}
                }             
                else if (containerRouteRequest.barcode == "115007001BK" || containerRouteRequest.barcode == "115007001FR")
                {
                    gtpName = "GTP-01";
                }               
                else if (containerRouteRequest.barcode == "115007002BK" || containerRouteRequest.barcode == "115007002FR")
                {
                    gtpName = "GTP-01";
                }              
                else if (containerRouteRequest.barcode == "115007003BK" || containerRouteRequest.barcode == "115007003FR")
                {
                    gtpName = "GTP-01";
                }                  
                else if (containerRouteRequest.barcode == "115007004BK" || containerRouteRequest.barcode == "115007004FR")
                {
                    gtpName = "GTP-01";
                }                 
                else if (containerRouteRequest.barcode == "115007005BK" || containerRouteRequest.barcode == "115007005FR")
                {
                    gtpName = "GTP-01";
                }               
                else if (containerRouteRequest.barcode == "115007006BK" || containerRouteRequest.barcode == "115007006FR")
                {
                    gtpName = "GTP-01";
                }            
                else if (containerRouteRequest.barcode == "115007007BK" || containerRouteRequest.barcode == "115007007FR")
                {
                    gtpName = "GTP-01";
                }
                    
                else if (containerRouteRequest.barcode == "115007008BK" || containerRouteRequest.barcode == "115007008FR")
                {
                    gtpName = "GTP-02";
                }
                    
                else if (containerRouteRequest.barcode == "115007009BK" || containerRouteRequest.barcode == "115007009FR")
                {
                    gtpName = "GTP-02";
                }
                else if (containerRouteRequest.barcode == "115007010BK" || containerRouteRequest.barcode == "115007010FR")
                {
                    gtpName = "GTP-02";
                }
                else if (containerRouteRequest.barcode == "115007011BK" || containerRouteRequest.barcode == "115007011FR")
                {
                    gtpName = "GTP-02";
                }
                else if (containerRouteRequest.barcode == "115007012BK" || containerRouteRequest.barcode == "115007012FR")
                {
                    gtpName = "GTP-02";

                }
                else if (containerRouteRequest.barcode == "115007013BK" || containerRouteRequest.barcode == "115007013FR")
                {
                    gtpName = "END";
                }
                else
                {
                    gtpName = "END";
                }
                //Data to response
                ContainerRouteResponse containerRouteResponse = new ContainerRouteResponse();
                containerRouteResponse.barcode = containerRouteRequest.barcode;
                containerRouteResponse.deviceId = gtpName;
                containerRouteResponse.scannerName = containerRouteRequest.scannerName;
                //Response message
                response.messageType = "ContainerRouteResponse";
                response.codeReturn = 0;
                response.message = "Success";
                response.data = containerRouteResponse;
            }
            else if (type == typeof(DeviceCommandRequest))
            {
                //Request
                DeviceCommandRequest deviceCommandRequest = (DeviceCommandRequest)data;
                //Data to response
                DeviceStatusResponse deviceStatusResponse = new DeviceStatusResponse();
                deviceStatusResponse.status = deviceCommandRequest.command;
                deviceStatusResponse.deviceId = deviceCommandRequest.deviceId;
                //Response message
                response.messageType = "DeviceStatus";
                response.codeReturn = 0;
                response.message = "Success";
                response.data = deviceStatusResponse;
            }
            else if (type == typeof(ContainerScanRequest))
            {
                //Request
                ContainerScanRequest containerScanRequest = (ContainerScanRequest)data;
                //Data to response
                ContainerScanResponse containerScanResponse = new ContainerScanResponse();
                containerScanResponse.deviceId = containerScanRequest.deviceId;
                //Response message
                response.messageType = "ContainerScanResponse";
                response.codeReturn = 0;
                response.message = "Success";
                response.data = containerScanResponse;
            }
            else if (type == typeof(PingRequest))
            {
                //Response message
                response.messageType = "Pong";
                response.codeReturn = 0;
                response.message = "Success";
                response.data = null;
            }
            else if(type == typeof(ContainerDivert))
            {
                //Request (data to response)
                ContainerDivert containerDivert = (ContainerDivert)data;
                //Data to response

                //Response  message
                response.messageType = "DivertConfirmResponse";
                response.codeReturn = 0;
                response.message = "Success";
                response.data = containerDivert;
            }
            else if (type == typeof(CurrentSequenceRequest))
            {
                //Request
                CurrentSequenceRequest currentSequenceRequest = (CurrentSequenceRequest)data;
                //Data to response
                CurrentSequenceResponse currentSequenceResponse = new CurrentSequenceResponse();
                currentSequenceResponse.currentSequenceNumber = 1000;

                //Response  message
                response.messageType = "CurrentSequenceResponse";
                response.codeReturn = 0;
                response.message = "Success";
                response.data = currentSequenceResponse;
            }

            SendResponseMessage(response);
        }

    }
}

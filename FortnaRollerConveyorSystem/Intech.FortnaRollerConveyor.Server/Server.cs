using Intech.FortnaRollerConveyor.Shared.Enums;
using Intech.FortnaRollerConveyor.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Server
{
    public class Server
    {

        private Thread keepAliveThread;


        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public TcpListener Listener { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Return true if the server instance is running
        /// </summary>
        public bool IsStarted { get; private set; }
        /// <summary>
        /// List of currently connected receivers
        /// </summary>
        public List<Receiver> Receivers { get; private set; }
        #endregion

        #region Contructors
        public Server(int port) 
        {
            Receivers = new List<Receiver>();
            Port = port;

            //Register events
            Receiver.OnClientConnected += Receiver_OnClientConnected;
            Receiver.OnClientDisconnected += Receiver_OnClientDisconnected;
        }

        #endregion

        #region Public methods
        public void Start()
        {
            if (!IsStarted)
            {
                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();
                IsStarted = true;
                Debug.WriteLine("Server started!");
            }

            // start asynchronous accepting new connections
            WaitForConnection();
        }

        public void Stop()
        {
            if (!IsStarted)
            {
                Listener.Stop();
                IsStarted = false;
                Debug.WriteLine("Server stoped!");
            }
        }
        #endregion

        #region Incomming connection methods
        private void WaitForConnection()
        {
            Listener.BeginAcceptTcpClient(new AsyncCallback(ConnectionHandler), null);
        }

        private void ConnectionHandler(IAsyncResult ar)
        {
            lock (Receivers)
            {
                Receiver newReceiver = new Receiver(Listener.EndAcceptTcpClient(ar), this);
                newReceiver.Start();
            }
            WaitForConnection();
        }
        #endregion

        #region Event handles
        private void Receiver_OnClientDisconnected(Receiver receiver)
        {
            Receivers.Remove(receiver);
        }

        private void Receiver_OnClientConnected(Receiver receiver)
        {
            Receivers.Add(receiver);
        }

        #endregion

    }
}

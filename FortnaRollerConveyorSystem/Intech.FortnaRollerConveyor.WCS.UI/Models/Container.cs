using Intech.FortnaRollerConveyor.Shared.Messages;
using Intech.FortnaRollerConveyor.WCS.UI.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.WCS.UI.Models
{
    public enum ContainerFlag
    {
        None = 0,
        Command = 1, //display color of container by Command
        DeviceId = 2, //display color of container by DeviceId
    }
    public class Container
    {
        /// <summary>
        /// Save the current values to the variables
        /// </summary>
        public string MessageType { get; set; }
        public int SequenceNumber { get; set; }
        public int CodeReturn { get; set; }
        public string Message { get; set; }
        public string Barcode { get; set; }
        public string DeviceId { get; set; }
        public string Command { get; set; }
        public string ScannerName { get; set; }
        /// <summary>
        /// waiting command from WES
        /// </summary>
        public bool waitingCommand { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public ContainerFlag Flag { get; set; }
        /// <summary>
        /// Save to the history of the messages
        /// </summary>
        public List<ResponseMessage> ResponseMessages { get; set; }

        public Container() 
        {
            this.ResponseMessages = new List<ResponseMessage>();
            this.MessageType = string.Empty;
            this.SequenceNumber = -1;
            this.CodeReturn = -1;
            this.Message = string.Empty;
            this.Barcode = string.Empty;
            this.DeviceId = AppConfig.WAITING_TEXT;
            this.Command = string.Empty;
            this.Flag = 0;
        }

        public Container(List<ResponseMessage> responseMsgs, string messageType, int sequenceNumber, int codeReturn, string message,  string barcode, string deviceId, string command)
        {
            this.ResponseMessages = responseMsgs;
            this.MessageType = messageType;
            this.CodeReturn = codeReturn;
            this.SequenceNumber= sequenceNumber;    
            this.Message = message;
            this.Barcode = barcode;
            this.DeviceId = deviceId;
            this.Command = command;
        }
    }
}

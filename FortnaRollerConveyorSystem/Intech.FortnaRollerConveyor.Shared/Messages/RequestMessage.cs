using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class RequestMessage : MessageBase
    {
        public string messageType { get; set; }
        public int sequenceNumber {  get; set; }
        public Object data { get; set; }

        public RequestMessage() 
        {
            this.messageType = string.Empty;
            this.sequenceNumber = 0;
            this.data = new Object();
        }

        public RequestMessage(string messageType, int sequenceNumber, Object data)
        {
            this.messageType = messageType;
            this.sequenceNumber = sequenceNumber;
            this.data = data;
        }

        public override string ToString()
        {
            string ret = string.Empty;
            if (data == null)
            {
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, data: null", this.messageType, this.sequenceNumber);
                return ret;
            }
               
            Type type = data.GetType();
            if (type == typeof(ContainerRouteRequest))
            {
                ContainerRouteRequest request = (ContainerRouteRequest)data;
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, scannerName: {2}, barcode: {3}",
                                            this.messageType, this.sequenceNumber, request.scannerName, request.barcode);
            }
            else if (type == typeof(DeviceCommandRequest))
            {
                DeviceCommandRequest request = (DeviceCommandRequest)data;
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, device: {2}, command: {3}",
                            this.messageType, this.sequenceNumber, request.deviceId, request.command);
            }
            else if (type == typeof(ContainerScanRequest))
            {
                ContainerScanRequest request = (ContainerScanRequest)data;
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, device: {2}, barcode: {3}",
                            this.messageType, this.sequenceNumber, request.deviceId, request.barcode);
            }
            return ret;
        }
    }
}

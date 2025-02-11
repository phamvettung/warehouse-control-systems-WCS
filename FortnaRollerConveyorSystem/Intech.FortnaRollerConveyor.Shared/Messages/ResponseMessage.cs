using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.Shared.Messages
{
    public class ResponseMessage : MessageBase
    {
        public string messageType {  get; set; }
        public int sequenceNumber {  get; set; }
        public int codeReturn { get; set; }
        public string message { get; set; }
        public Object data { get; set; }

        public ResponseMessage() 
        {
            this.messageType = string.Empty;
            this.sequenceNumber = 0;
            this.codeReturn = 0;
            this.message = string.Empty;
            this.data = null;
        }
        
        public ResponseMessage(string messageType, int sequenceNumber, int codeReturn, string message, object data)
        {
            this.messageType = messageType;
            this.sequenceNumber = sequenceNumber;
            this.codeReturn = codeReturn;
            this.message = message;
            this.data = data;
        }

        public override string ToString() { 
            Type type = data.GetType();
            string ret = string.Empty;  
            if (type == null) return ret;
            else if (type == typeof(ContainerRouteResponse))
            {
                ContainerRouteResponse response = (ContainerRouteResponse) data;
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, codeReturn: {2}, message: {3}, scannerName: {4}, deviceId: {5}, barcode: {6}",
                                            this.messageType, this.sequenceNumber, this.codeReturn, this.message, response.scannerName, response.deviceId, response.barcode );
            }
            else if (type == typeof(DeviceStatusResponse))
            {
                DeviceStatusResponse response = (DeviceStatusResponse)data;
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, codeReturn: {2}, message: {3}, device: {4}, status: {5}",
                            this.messageType, this.sequenceNumber, this.codeReturn, this.message, response.deviceId, response.status);
            }
            else if (type == typeof(ContainerScanResponse)) 
            {
                ContainerScanResponse response = (ContainerScanResponse) data;
                ret = string.Format("messageType: {0}, sequenceNumber: {1}, codeReturn: {2}, message: {3}, device: {4}",
                            this.messageType, this.sequenceNumber, this.codeReturn, this.message, response.deviceId);
            }
            return ret;
        }
    }
}

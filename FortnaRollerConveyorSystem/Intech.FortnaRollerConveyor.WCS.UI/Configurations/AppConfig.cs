using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intech.FortnaRollerConveyor.WCS.UI.Configurations
{
    public class PlcConfig
    {
        public int StationNumber { get; set; }
        public string PcReady {  get; set; }
        public string Running { get; set; }
        public string Pause { get; set; }
        public string Reset { get; set; }
        public string ModeStatus { get; set; }
        public string StartSystem { get; set; }
        public string StopSystem { get; set; }
        public string ResetSystem { get; set; }
        public string ModeSystem { get; set; }
        public string Stoppers { get; set; }
        public string Pushers { get; set; }
        public string Sensors { get; set; }
        public string Cluster01 { get; set; }
        public string Cluster03A { get; set; }
        public string Cluster03B { get; set; }
        public string Cluster05 { get; set; }
        public string Cluster07A { get; set; }
        public string Cluster07B { get; set; }
        public string Emg00 { get; set; }
        public string Emg01 { get; set; }
        public string Emg02 { get; set; }

        public static string CLUSTER_01 = "cluster_01";
        public static string CLUSTER_03A = "cluster_03A";
        public static string CLUSTER_03B = "cluster_03B";
        public static string CLUSTER_05A = "cluster_05A";
        public static string CLUSTER_05B = "cluster_05B";
        public static string CLUSTER_07 = "cluster_07";
    }

    public class ScannerConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }

    public class WesConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int EnableKeepAlive { get; set; }
        public int KeepAliveInterval { get; set; }
        public int KeepAliveTimeout { get; set; }
        public int RequestTimedout { get; set; }
        public int AutoReconnectTimes {  get; set; }
        public int AutoReconnectInterval { get; set; }

    }

    static class AppConfig
    {
        public static string GTP_01 = "GTP-01";
        public static string GTP_02 = "GTP-02";
        public static string END = "END";
        public static string SCANNER_01 = "SCANNER-01";
        public static string SCANNER_02 = "SCANNER-02";
        public static string ADVANCE = "ADVANCE";
        public static string COMPLETE = "COMPLETE";
        public static string COMPLETEADVANCE = "COMPLETEADVANCE";
        public static string READY = "READY";
        public static string BUSY = "BUSY";
        public static string NG = "NG";
        public static string WAITING_TEXT = ". . .";
        public static string SUCCESS = "Succes";

        public static string CONTAINER_ROUTE_REQUEST = "ContainerRouteRequest";
        public static string CONTAINER_ROUTE_RESPONSE = "ContainerRouteResponse";

        public static string DEVICE_COMMAND = "DeviceCommand";
        public static string DEVICE_STATUS = "DeviceStatus";

        public static string CONTAINER_SCAN = "ContainerScan";
        public static string CONTAINER_SCAN_RESPONSE = "ContainerScanResponse";

        public static string PING_REQUEST = "Ping";
        public static string PONG_RESPONSE = "Pong";
        public static string FIN_REQUEST = "Fin";

        public static string DIVERT_CONFIRM = "DivertConfirm";
        public static string DIVERT_CONFIRM_RESPONSE = "DivertConfirmResponse";

        public static string CURRENT_SEQUENCE_REQUEST = "CurrentSequenceRequest";
        public static string CURRENT_SEQUENCE_RESPONSE = "CurrentSequenceResponse";

        public static readonly int SIZE_OF_CONTAINER = 14;

        public static WesConfig GetWESConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            WesConfig config = new WesConfig();
            config.IpAddress = configuration["WES:ipAddress"];
            config.Port = int.Parse(configuration["WES:port"]);
            config.KeepAliveInterval = int.Parse(configuration["WES:keepAliveInterval"]);
            config.KeepAliveTimeout = int.Parse(configuration["WES:keepAliveTimeout"]);
            config.EnableKeepAlive = int.Parse(configuration["WES:enableKeepAlive"]);
            config.RequestTimedout = int.Parse(configuration["WES:requestTimedout"]);
            config.AutoReconnectTimes = int.Parse(configuration["WES:autoReconnectTimes"]);
            config.AutoReconnectInterval = int.Parse(configuration["WES:autoReconnectInterval"]);
            return config;
        }

        public static ScannerConfig GetScanner01Config()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            ScannerConfig config = new ScannerConfig();
            config.IpAddress = configuration["Scanner01:ipAddress"];
            config.Port = int.Parse(configuration["Scanner01:port"]);
            return config;
        }

        public static ScannerConfig GetScanner02Config()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            ScannerConfig config = new ScannerConfig();
            config.IpAddress = configuration["Scanner02:ipAddress"];
            config.Port = int.Parse(configuration["Scanner02:port"]);
            return config;
        }

        public static PlcConfig GetPlcConfig()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            PlcConfig config = new PlcConfig();
            config.StationNumber = int.Parse(configuration["PLC:stationNumber"]);
            config.PcReady = configuration["PLC:pcReady"];
            config.Running = configuration["PLC:running"];
            config.Pause = configuration["PLC:pause"];
            config.Reset = configuration["PLC:reset"];
            config.ModeStatus = configuration["PLC:modeStatus"];
            config.StartSystem = configuration["PLC:startSystem"];
            config.StopSystem = configuration["PLC:stopSystem"];
            config.ResetSystem = configuration["PLC:resetSystem"];
            config.ModeSystem = configuration["PLC:modeSystem"];
            config.Stoppers = configuration["PLC:stoppers"];
            config.Pushers = configuration["PLC:pushers"];
            config.Sensors = configuration["PLC:sensors"];
            config.Cluster01 = configuration["PLC:cluster01"];
            config.Cluster03A = configuration["PLC:cluster03A"];
            config.Cluster03B = configuration["PLC:cluster03B"];
            config.Cluster05 = configuration["PLC:cluster05"];
            config.Cluster07A = configuration["PLC:cluster07A"];
            config.Cluster07B = configuration["PLC:cluster07B"];          
            config.Emg00 = configuration["PLC:emg0"];          
            config.Emg01 = configuration["PLC:emg1"];          
            config.Emg02 = configuration["PLC:emg2"];          
            return config;
        }
    }
}

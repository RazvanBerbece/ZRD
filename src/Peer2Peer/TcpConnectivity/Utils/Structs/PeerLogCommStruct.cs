using System;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using System.Text.Json;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.TcpDirectionEnumNS;
using StaticsNS;

namespace Peer2PeerNS.TcpConnectivity.PeerCommLogStructNS
{
    public struct PeerCommLogStruct
    {
        public string FromIp { get; set; }
        public string ToIp { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
        public string TrafficDirection { get; set; }

        public PeerCommLogStruct(string src, string dest, DateTime logTimestamp, string data, TcpDirectionEnum direction)
        {
            FromIp = src;
            ToIp = dest;
            Timestamp = logTimestamp;
            Data = data;
            TrafficDirection = direction.ToString();
        }

        public string ToJsonString()
        {
            string jsonStringBlock = JsonSerializer.Serialize(
                this,
                options: new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // this specifies that specific symbols like '/' don't get encoded in unicode
                }
            );
            return jsonStringBlock;
        }
        
                /// <summary>
        /// Logs a comm session between the server peer and the client peer to a default filepath in local/
        /// </summary>
        /// <param name="peer">Peer which the node communicates to</param>
        /// <param name="data">Data in/out for comm</param>
        /// <param name="timestamp">Timestamp when comm happened</param>
        public static void LogPeerCommunication(TcpClient peer, string data, DateTime timestamp, TcpDirectionEnum direction)
        {
            // Create logs directory under local/ if not existing
            System.IO.Directory.CreateDirectory("local/logs"); 
            
            string logFilepath = "local/logs/TCPServer.logs";
            PeerCommLogStruct logObject;
            switch ((ushort)direction)
            {
                case 0:
                    // TCP IN
                    logObject = new PeerCommLogStruct(
                        Statics.GetPeerPublicIp(peer),
                        Statics.GetExternalPublicIpAddress().ToString(),
                        timestamp,
                        data,
                        direction
                    );
                    System.IO.File.AppendAllText(logFilepath, logObject.ToJsonString());
                    System.IO.File.AppendAllText(
                        logFilepath, 
                        "\n---------------------------------------------------------------------------------------------------------------------------------------------------------------\n"
                    );
                    break;
                case 1:
                    // TCP OUT
                    logObject = new PeerCommLogStruct(
                        Statics.GetExternalPublicIpAddress().ToString(),
                        Statics.GetPeerPublicIp(peer),
                        timestamp,
                        data,
                        direction
                    );
                    System.IO.File.AppendAllText(logFilepath, logObject.ToJsonString());
                    System.IO.File.AppendAllText(
                        logFilepath, 
                        "\n---------------------------------------------------------------------------------------------------------------------------------------------------------------\n"
                    );
                    break;
            }
        }
    }
}
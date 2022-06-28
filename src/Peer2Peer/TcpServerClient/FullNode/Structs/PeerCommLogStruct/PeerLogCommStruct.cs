using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using Peer2PeerNS.TcpServerClientNS.FullNodeNS.EnumsNS.TcpDirectionEnumNS;

namespace Peer2PeerNS.TcpServerClientNS.FullNodeNS.StructsNS.PeerCommLogStructNS
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
    }
}
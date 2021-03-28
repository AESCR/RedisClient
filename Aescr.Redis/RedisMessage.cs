using System;

namespace Aescr.Redis
{
    public class RedisMessage
    {
        public string SendCommand { get; set; }
        public string RawCommand { get; set; }
        public string RawMessage { get; set; }
        public object ReceiveMessage { get; set; }
    }
}
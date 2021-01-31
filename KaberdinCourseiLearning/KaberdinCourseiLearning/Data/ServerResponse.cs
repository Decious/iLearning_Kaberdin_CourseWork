using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data
{
    public class ServerResponse
    {
        public ServerResponse(bool successful, string message,string url=null)
        {
            Successful = successful;
            Message = new string[1] { message };
            Url = url;
        }
        public ServerResponse(bool successful, string[] message, string url = null)
        {
            Successful = successful;
            Message = message;
            Url = url;
        }
        public bool Successful { get; set; }
        public string[] Message { get; set; }
        public string Url { get; set; }
    }
}

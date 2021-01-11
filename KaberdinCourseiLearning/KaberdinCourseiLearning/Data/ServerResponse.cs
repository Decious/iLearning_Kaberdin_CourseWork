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
            Message = message;
            Url = url;
        }
        public static ServerResponse MakeForbidden() => new ServerResponse(false, "You are not authorized to commit these changes.");
        public static ServerResponse MakeSuccess() => new ServerResponse(true, "Action completed successfully.");
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }
}

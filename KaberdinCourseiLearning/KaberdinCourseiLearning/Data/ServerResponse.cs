using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data
{
    public class ServerResponse
    {
        public ServerResponse(ServerResponseStatus statusCode, string message)
        {
            status = ConvertStatusCodeToStatus(statusCode);
            this.message = message;
        }
        public static ServerResponse MakeForbidden() => new ServerResponse(ServerResponseStatus.ERROR, "You are not authorized to commit these changes.");
        public static ServerResponse MakeSuccess() => new ServerResponse(ServerResponseStatus.SUCCESS, "Action completed successfully.");
        public string status { get; set; }
        public string message { get; set; }
        private string ConvertStatusCodeToStatus(ServerResponseStatus statusCode)
        {
            if (statusCode == ServerResponseStatus.ERROR) return "error";
            return "success";
        }
    }
}

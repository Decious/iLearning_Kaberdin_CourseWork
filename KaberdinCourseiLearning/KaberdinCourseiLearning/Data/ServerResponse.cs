using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data
{
    public class ServerResponse
    {
        public ServerResponse(bool successful, string message)
        {
            this.successful = successful;
            this.message = message;
        }
        public static ServerResponse MakeForbidden() => new ServerResponse(false, "You are not authorized to commit these changes.");
        public static ServerResponse MakeSuccess() => new ServerResponse(true, "Action completed successfully.");
        public bool successful { get; set; }
        public string message { get; set; }
    }
}

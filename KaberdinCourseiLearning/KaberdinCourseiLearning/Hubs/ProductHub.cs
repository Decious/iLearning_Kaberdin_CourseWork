using KaberdinCourseiLearning.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Hubs
{
    public class ProductHub : Hub
    {
        ApplicationDbContext dbContext;
        public ProductHub(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}

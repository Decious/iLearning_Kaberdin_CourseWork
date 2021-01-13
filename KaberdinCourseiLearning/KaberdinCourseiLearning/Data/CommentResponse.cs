using KaberdinCourseiLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaberdinCourseiLearning.Data
{
    public class CommentResponse
    {
        public string Message { get; set; }
        public string[] Roles { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public string CreationDate { get; set; }
        public CommentResponse(CustomUser user,Comment comment,string[] roles)
        {
            Message = comment.Message;
            Roles = roles;
            UserName = user.UserName;
            AvatarUrl = user.AvatarUrl;
            CreationDate = comment.CreationTime.ToShortDateString() +" "+ comment.CreationTime.ToShortTimeString();
        }
    }
}

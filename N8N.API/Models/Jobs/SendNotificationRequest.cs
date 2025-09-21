using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N8N.API.Models.Jobs
{
    public class SendNotificationRequest
    {
        public string FirstName {  get; set; }
        
        public string LastName { get; set; }

        public string Email { get; set; }


    }
}

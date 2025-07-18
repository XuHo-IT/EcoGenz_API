using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request.User
{
    public class GoogleLoginRequest
    {
        public string tokenId { get; set; }
        public string role { get; set; }
    }
}

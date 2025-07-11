﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request.User
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string[] Roles { get; set; } = new string[] { "User" };
    }
}

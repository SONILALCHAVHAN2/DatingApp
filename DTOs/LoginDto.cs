using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace DatingApp.DTOs
{
    public class LoginDto
    {
        public string  Username { get; set; }
        public string Password { get; set; }
    }
}
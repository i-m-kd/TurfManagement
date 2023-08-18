using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TurfManagement.Models
{
    public class RegisterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Place { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

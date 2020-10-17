using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Personal.Webapi.Entity
{
    public partial class Users
    {
        [Key]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
    }
}

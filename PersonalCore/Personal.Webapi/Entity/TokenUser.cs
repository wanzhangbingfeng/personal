using System;
using System.Collections.Generic;

namespace Personal.Webapi.Entity
{
    public partial class TokenUser
    {
        public Guid TokenUserId { get; set; }
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpriedTime { get; set; }
    }
}

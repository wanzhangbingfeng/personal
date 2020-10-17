using System;
using System.Collections.Generic;

namespace Personal.Webapi.Entity
{
    public partial class PayType
    {
        public Guid PayTypeId { get; set; }
        public string Name { get; set; }
        public bool State { get; set; }
    }
}

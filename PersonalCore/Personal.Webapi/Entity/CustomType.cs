using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal.Webapi.Entity
{
    public partial class CustomType
    {
        public Guid CustomTypeId { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public bool State { get; set; }
    }
}

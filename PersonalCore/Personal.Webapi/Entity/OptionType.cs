using System;
using System.Collections.Generic;

namespace Personal.Webapi.Entity
{
    public partial class OptionType
    {
        public Guid OptionTypeId { get; set; }
        public string Name { get; set; }
        public int? Value { get; set; }
        public string ParentId { get; set; }
    }
}

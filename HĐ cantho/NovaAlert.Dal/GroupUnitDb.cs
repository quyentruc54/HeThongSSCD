//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NovaAlert.Dal
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupUnitDb
    {
        public int GroupId { get; set; }
        public int PhoneNumberId { get; set; }
        public int ListOrder { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual GroupDb Group { get; set; }
        public virtual PhoneNumber PhoneNumber { get; set; }
    }
}

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
    
    public partial class DbLog
    {
        public long Id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public byte PanelId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Info { get; set; }
    }
}

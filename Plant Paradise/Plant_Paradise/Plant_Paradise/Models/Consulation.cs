//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Plant_Paradise.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Consulation
    {
        public int Consulation_Id { get; set; }
        public string User_Id { get; set; }
        public string Order_id { get; set; }
        public string Plant_status { get; set; }
        public string Plant_image { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Order Order { get; set; }
    }
}

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
    
    public partial class cart
    {
        public int Cart_Id { get; set; }
        public Nullable<int> Product_id { get; set; }
        public string userId { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Product Product { get; set; }
    }
}

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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.carts = new HashSet<cart>();
            this.Comments = new HashSet<Comment>();
            this.Order_Details = new HashSet<Order_Details>();
        }

        [DisplayName("Product Name")]
        public int Product_id { get; set; }
        [DisplayName("Product Name")]
        public string Product_Name { get; set; }
        [DisplayName("Product Image")]
        public string Product_Image { get; set; }
        [DisplayName("Product Size")]
        public string Product_Size { get; set; }
        public string Product_Color { get; set; }
        [DisplayName("Product Price")]
        public Nullable<double> Product_Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        [DisplayName("Product Discription")]
        public string Product_Description { get; set; }
        
        public Nullable<int> Subcategory_id { get; set; }
        [DisplayName("Watering")]

        public string Water_needed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart> carts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Details> Order_Details { get; set; }
        public virtual Sub_Categories Sub_Categories { get; set; }
    }
}

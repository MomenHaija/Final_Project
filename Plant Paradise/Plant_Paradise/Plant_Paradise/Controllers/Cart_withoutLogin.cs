using Plant_Paradise.Models;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Web;

namespace Plant_Paradise.Controllers
{
    public class Cart_withoutLogin
    {
        
        public Nullable<int> Product_id { get; set; }
        public string userId { get; set; }
        public Nullable<int> Quantity { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Product Product { get; set; }
    }
}
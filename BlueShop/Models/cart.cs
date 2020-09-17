using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueShop.Models
{
    public class cart
    {
        public System.Guid ProductID { get; set; }
        public Nullable<System.Guid> ShopID { get; set; }
        public string Name { get; set; }
        public string ShopName { get; set; }
        public string Image { get; set; }
        public float Price { get; set; }
        public int qty { get; set; }
        public float bill { get; set; }
    }
}
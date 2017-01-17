using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YTTimeRentWeb.Models
{
    public class ShopModel
    {
        public long Id { get; set; }//门店Id;
        public string Name { get; set; }
        public decimal? Longitude { get; set; }//经度
        public decimal? Latitude { get; set; }//纬度
        public string Address { get; set; }
        public int Carcount { get; set; }
    }
}
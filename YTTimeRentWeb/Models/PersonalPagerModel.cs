using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YTTimeRentWeb.Models
{
    public class PersonalPagerModel
    {
        public List<PersonalCenterModel> Personallist { get; set; }
        public string Pagerlink { get; set; }
    }
}
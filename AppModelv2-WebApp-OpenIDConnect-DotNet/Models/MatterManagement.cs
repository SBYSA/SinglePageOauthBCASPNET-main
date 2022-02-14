using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Models
{
    public class MatterManagement
    {
        public List<Matter> AvailableMatters { get; set; }
        public List<Matter> MyMatters { get; set; }
    }
}
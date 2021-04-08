using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoresWebApi.Models
{
    public class Token
    {
        public string JWT { get; set; }
        public string RefreshToken { get; set; }
    }
}

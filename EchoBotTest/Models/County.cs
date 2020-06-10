using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalBotTest.Models
{
    public class County
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public List<SubCounty> SubCounties { get; set; }
    }
}

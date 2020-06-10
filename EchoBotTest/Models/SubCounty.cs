using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalBotTest.Models
{
    public class SubCounty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string [] Wards { get; set; }
    }
}

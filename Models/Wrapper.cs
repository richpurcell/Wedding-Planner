using System;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wrapper
    {
        public List<Wedding> AllWeddings { get; set; }

        public List<Association> AllEvents {get;set;}
    }
}
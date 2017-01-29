using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lernplat.Models
{
    /// <summary>
    /// Contains a model of the necessary values from the given csv file 
    /// </summary>
    public class LernplanModel
    {
        public string Name { get; set; }
        public DateTime Beginn { get; set; }
        public DateTime Ende { get; set; }
        public int Gewichtung { get; set; }
    }
}
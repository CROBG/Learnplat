using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lernplat.Models
{
    /// <summary>
    /// Contains the necessary output values
    /// </summary>
    public class ZeitsverbrachtModel
    {
        public DateTime Tag { get; set; }
        public TimeSpan Verbrauch { get; set; }
        public int Lerneinheiten { get; set; }
        public List<string> LernFacher { get; set; }
        public string KlausurFach { get; set; }
    }
}
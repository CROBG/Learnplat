using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lernplat.Models
{
    public class MainModel
    {
        /// <summary>
        /// Creates a list of the given models
        /// </summary>
        public List<LernplanModel> lernplan = new List<LernplanModel>();
        public List<ZeitsverbrachtModel> zeitverbracht = new List<ZeitsverbrachtModel>();

        /// <summary>
        /// Creates a grouping lists of the Name string from the LernplanModel
        /// </summary>
        public List<LernplanModel> grupiertFach = new List<LernplanModel>();
        public List<int> Gewichtung { get; set; }
    }
}
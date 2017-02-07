using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lernplat.Models;

namespace Lernplat.Interfaces
{
    public interface IHomeController
    {
        /// <summary>
        /// Controller for the Index page
        /// Contains the Subject table and the Timeusage table
        /// </summary>
        /// <returns>Index.cshtml/returns>
        ActionResult Index();

        /// <summary>
        /// Refreshing the page with the added values
        /// </summary>
        /// <param name="gew">Returning model</param>
        /// <returns></returns>
        [HttpPost]
        ActionResult Index(GewichtungsModel gew);

        /// <summary>
        /// Opens a new Tab that consists of the requsted PDF document 
        /// </summary>
        /// <param name="FileName">The name of the PDF file to what the link corespondes</param>
        /// <returns></returns>
        ActionResult PDF(String FileName);
    }
}
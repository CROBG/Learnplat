using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using Lernplat.Models;

namespace Lernplat.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Contains a filepath to the csv file
        /// </summary>
        public static string Pfad = "~/LernplanFiles/Stundenplan.csv";

        public static Listen listenObj = new Listen();

        public static String PathToPDF = "";

        /// <summary>
        /// Controller for the Index page
        /// Contains the Subject table and the Timeusage table
        /// </summary>
        /// <returns>Index.cshtml/returns>
        public ActionResult Index()
        {
            listenObj.lernplan = HomeHeader.DataLoader(Server.MapPath(Pfad));

            listenObj.zeitverbracht = HomeHeader.VebrauchsCalculations(listenObj.lernplan);

            /*Groups the Name values from the LernplanModel list and creates a Subject list
             */
            listenObj.grupiertFach = listenObj.lernplan.Where(o => o.Name.Contains("T2"))
                                                       .GroupBy(o => o.Name)
                                                       .Select(grp => grp.FirstOrDefault())
                                                       .ToList();
            return View(new GewichtungsModel());
        }

        [HttpPost]
        public ActionResult Index(GewichtungsModel gew)
        {
            for (int i = 0; i < listenObj.grupiertFach.Count; i++)
                listenObj.grupiertFach[i].Gewichtung = gew.Gewichtung[i];

            return View(gew);
        }

        private string FDir_AppData = "~/PDFs/";

        /// <summary>
        /// Opens a new Tab that consists of the requsted PDF document 
        /// </summary>
        /// <param name="FileName">PDF name</param>
        /// <returns></returns>
        public ActionResult PDF(String FileName)
        {
            var slapath = Server.MapPath(FDir_AppData + FileName + ".pdf");

            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "inline;filename=" + slapath);
            Response.ContentType = "application/pdf";
            Response.WriteFile(slapath);
            Response.Flush();
            Response.Clear();
            return View();
        }
    }
}
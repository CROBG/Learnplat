using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Lernplat.Models;

namespace Lernplat.Controllers 
{
    public class HomeHeader : Interfaces.IHomeHeader
    {
        public List<ZeitsverbrachtModel> VebrauchsCalculations(List<LernplanModel> lernplan)
        {
            List<ZeitsverbrachtModel> zeitverbracht = new List<ZeitsverbrachtModel>();

            /*Iteration Variable
             */
            int i;

            for (i = 0; i < lernplan.Count() - 1; i++)
            {
                DateTime dt = new DateTime();
                TimeSpan ts = new TimeSpan();

                /*Minimal posible TimeSpan value
                 */
                TimeSpan nullVerbrauch = new TimeSpan(0, 0, 0);

                /*Threshold for 3 instances of the Lerneinheit value
                 */
                TimeSpan mindestVerbrauch = new TimeSpan(7, 29, 0);

                /*Integer containing the number of "Lerneinheiten pro Tag"
                 */
                int lerneinheit;

                /*Skips the current Iteration for the given filtered Subject
                 * Filtered subject won´t be counted into he working hours
                 */
                if (lernplan[i].Name == "Repetitorium Mathematik")
                    continue;

                /*Breaks the Iteration if the List contains one member with the name "Klausur" in it
                 */
                if (lernplan[i].Name.Contains("Klausur"))
                    break;

                /*Sets the Timespan ts to 0 for the given filtered Subject
                 */
                if (lernplan[i].Name == "Studium flexibel" || lernplan[i].Name == "Praxisphase" || lernplan[i].Name == "Weihnachten" || lernplan[i].Name == "Neujahr")
                {
                    dt = lernplan[i].Beginn.Date;
                    ts = new TimeSpan(0, 0, 0);
                }
                /*If there are two instances of Name with the same Date they are counted together into the Timespan ts
                 * Also counts the time inbetween the two instances as an Pause
                 */
                else if (lernplan[i + 1].Beginn - lernplan[i].Beginn < new TimeSpan(10, 1, 0))
                {
                    dt = lernplan[i].Beginn.Date;
                    ts = lernplan[i + 1].Ende - lernplan[i].Beginn;
                    i++;
                }
                /*Sets the timespan ts approporiate to the lenght of the Name instance
                 */
                else
                {
                    dt = lernplan[i].Beginn.Date;
                    ts = lernplan[i].Ende - lernplan[i].Beginn;
                }

                /*Sets the lerneinheit number to 1 if the Timespan ts for the Date is longer than 7:29:00
                 */
                if (ts > mindestVerbrauch) lerneinheit = 1;
                /*Sets the lerneinheit number to 6 if the Timespan ts for the Date is 00:00:00
                 */
                else if (ts == nullVerbrauch) lerneinheit = 6;
                /*Sets the lerneinheit number to 3 for the rest of the cases
                 */
                else lerneinheit = 3;

                /*Adds the DateTime dt, TimeSpan ts and Lerneinheiten to the Zeitvebracht model list
                 */
                zeitverbracht.Add(new ZeitsverbrachtModel()
                {
                    Tag = dt,
                    Verbrauch = ts,
                    Lerneinheiten = lerneinheit,
                    LernFacher = new List<string>() { " ", " " }
                    
                });

                HomeController.LehreinheitenCount += lerneinheit;
            }

            /*Continues where the Broken Iteration left of
             * Iterates the other member in the list and adds the needed "Klausur" Subjects on the coresponding days
             */
            for(int j = i; j < lernplan.Count(); j++)
            {
                /*Checks the name for the keywords "Klausur"
                 * Adds the DateTime and the Timespan for the Subject in question
                 */
                if (lernplan[j].Name.Contains("Klausur"))
                    zeitverbracht.Add(new ZeitsverbrachtModel()
                    {
                        Tag = lernplan[j].Beginn.Date,
                        Verbrauch = lernplan[i].Ende - lernplan[i].Beginn,
                        Lerneinheiten = 0,
                        LernFacher = new List<string>() { "Klausur ", lernplan[j].Name.Replace("Klausur ", "") }

                    });
            }

            return zeitverbracht;
        }

        public List<LernplanModel> DataLoader(string Path)
        {
            List<LernplanModel> lernplan = new List<LernplanModel>();
            //Makes a reference to the csv file
            StreamReader reader = new StreamReader(Path);

            /*Reads the whole csv file to the end
             */
            while (!reader.EndOfStream)
            {
                //Reads the current line in the file
                var line = reader.ReadLine();

                /*Converts the current line into an string array
                 * column Name = values[0]
                 * column Beginn = values[1]
                 * column Ende = values[2]
                 */
                var values = line.Split(';');

                /*Checks the value of the current row and continues the loop if its on the first line of the table
                 */
                if (values[0] == "Name" || values[0] == "Repetitorium Mathematik")
                    continue;

                /*Adds the data from the current row to the LernplanModel list
                 */
                lernplan.Add(new LernplanModel
                {
                    Name = values[0].ToString(),
                    Beginn = DateTime.Parse(values[1].ToString()),
                    Ende = DateTime.Parse(values[2].ToString())
                });
            }

            /*Disposes the used StreamReader for the csv file
             */
            reader.Dispose();

            return lernplan;
        }

        public Listen LerneinheitenVerteiler(Listen listenObj)
        {
            /*Orders the list of Subjets depending on the Gewichtungs Factor
             */
            listenObj.grupiertFach = listenObj.grupiertFach.OrderByDescending(o => o.LerneinheitenZahl).ToList();

            /*Iterates through the list of Dates and adds the Subjects to the Date
             */
            for (int i = 0; i < listenObj.zeitverbracht.Count(); i++)
            {
                if (listenObj.zeitverbracht[i].LernFacher[0] != " ") continue;
                //If the day has 6 Lerneinheiten it adds two subjets with the highest Gewichtungs factor
                if (listenObj.zeitverbracht[i].Lerneinheiten == 6)
                {
                    listenObj.zeitverbracht[i].LernFacher[0] = listenObj.grupiertFach[0].Name.Split('(').First();
                    listenObj.zeitverbracht[i].LernFacher[1] = listenObj.grupiertFach[1].Name.Split('(').First();
                    listenObj.grupiertFach[0].LerneinheitenZahl -= 3;
                    listenObj.grupiertFach[1].LerneinheitenZahl -= 3;
                }
                //If the day has less than 6, it adds just one Subject
                else
                {
                    listenObj.zeitverbracht[i].LernFacher[0] = listenObj.grupiertFach[0].Name.Split('(').First();
                    listenObj.grupiertFach[0].LerneinheitenZahl -= listenObj.zeitverbracht[i].Lerneinheiten;
                }

                /*Orders the list of Subjets depending on the Gewichtungs Factor with every Iteration
                 */
                listenObj.grupiertFach = listenObj.grupiertFach.OrderByDescending(o => o.LerneinheitenZahl).ToList();
            }

            return listenObj;
        }
    }
}
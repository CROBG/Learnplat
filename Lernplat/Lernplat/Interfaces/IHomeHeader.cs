using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lernplat.Models;

namespace Lernplat.Interfaces
{
    public interface IHomeHeader
    {
        /// <summary>
        /// Creates a ZeitverbrachtModel list for the given csv filepath
        /// </summary>
        /// <param name="Path">String containing the csv filepath</param>
        /// <returns></returns>
        List<LernplanModel> DataLoader(string Path);

        /// <summary>
        /// Creates a calculated ZeitverbrachtModel list for the given LernplanModel list
        /// </summary>
        /// <param name="lernplan">LernplanModel list</param>
        /// <returns></returns>
        List<ZeitsverbrachtModel> VebrauchsCalculations(List<LernplanModel> lernplan);

        /// <summary>
        /// Spreads the available Subjets on the Calender Dates dependent on the Gewichtugs factor on that Subjet
        /// </summary>
        /// <param name="listenObj">Object containing all Lists</param>
        /// <returns></returns>
        Listen LerneinheitenVerteiler(Listen listenObj);
    }
}
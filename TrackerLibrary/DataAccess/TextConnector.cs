using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public class TextConnector : IDataConnection
    {
        // TODO - Wire up the CreatePrize for text files. 
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Load the text file - (not based on a certain model)
            // Convert the text to List<PrizeModel>
            // Find the max ID using LINQ
            // Add the new record with the new ID (max + 1)
            // Convert the prizes to List<string>
            // Save the List<string> to the text file
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TrackerLibrary.Models;
using System.CodeDom;
using System.Data.Odbc;

// "LoadFile" Load the text file - (not based on a certain model)
// "ConvertToPrizeModels" Convert the text to List<PrizeModel>
// Find the max ID using LINQ
// Add the new record with the new ID (max + 1)
// Convert the prizes to List<string>
// Save the List<string> to the text file

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Returns the full path from the file name we passed in
        /// Extension Method: "string example".FullFilePath();
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FullFilePath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName } ";
        }

        /// <summary>
        /// Takes in the fullfilepath and loads that string
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] columns = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(columns[0]);
                p.PlaceNumber = int.Parse(columns[1]);
                p.PlaceName = columns[2];
                p.PrizeAmount = decimal.Parse(columns[3]);
                p.PrizePercentage = double.Parse(columns[4]);
                output.Add(p);
            }

            return output;

        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.EmailAddress = cols[3];
                p.CellPhoneNumber = cols[4];
                output.Add(p);
            }

            return output;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            // id, team name, list of ids seperated by the pipe symbol
            // Row example below:
            // 3, Colin's Team, 1|3|5

            List<TeamModel> output = new List<TeamModel>();
            // We now have a list of all the people stored in the text file
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                // List of ids for people / team members
                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());   
                }
            }

            return output;

        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName}, { p.PrizeAmount},{ p.PrizePercentage }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.EmailAddress },{ p.CellPhoneNumber }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in models)
            {
                lines.Add($"{t.Id},{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        /// <summary>
        /// Taking the list of personModel and returning a string instead
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";
            //2|5| < Example

            if (people.Count == 0)
            {
                return "";
            }
            foreach (PersonModel p in people)
            {
                output += $"{ p.Id }|";
            }
            // Removing the last pipe symbol
            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}

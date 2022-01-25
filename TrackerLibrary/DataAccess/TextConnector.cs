using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";

        public PersonModel CreatePerson(PersonModel model)
        {
            // Load the text fileand convert the text to List<PrizeModel>
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find the last ID
            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            // Add new record with the new ID (last Id + 1)
            people.Add(model);

            // Convert the people to List<string> and save to the text file
            people.SaveToPersonFile(PeopleFile);

            return model;
        }

        /// <summary>
        /// Saves a prize to a file.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Load the text fileand convert the text to List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Find the last ID
            int currentId = 1;
            if (prizes.Count > 0)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            
            model.Id = currentId;

            // Add new record with the new ID (last Id + 1)
            prizes.Add(model);

            // Convert the prizes to List<string> and save to the text file
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }
    }
}
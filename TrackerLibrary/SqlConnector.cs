namespace TrackerLibrary
{
    public class SqlConnector : IDataConnection
    {
        // TODO: Make the CreatePrize method actually save to the database
        /// <summary>
        /// Saves a prize to the database.
        /// </summary>
        /// <param name="prize">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel prize)
        {
            prize.Id = 1;

            return prize;
        }
    }
}
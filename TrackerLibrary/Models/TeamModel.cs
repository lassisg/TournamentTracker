namespace TrackerLibrary.Models
{ 
    /// <summary>
    /// Represents a team in the tournament.
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// The unique identifier for the team.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the team.
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// The members of the team.
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
    }
}
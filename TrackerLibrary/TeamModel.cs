/// <summary>
/// Represents a team in the tournament.
/// </summary>
public class TeamModel
{
    /// <summary>
    /// The members of the team.
    /// </summary>
    public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

    /// <summary>
    /// The name of the team.
    /// </summary>
    public string TeamName { get; set; }

}
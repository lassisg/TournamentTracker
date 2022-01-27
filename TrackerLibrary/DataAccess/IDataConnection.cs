using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel prize);
        PersonModel CreatePerson(PersonModel person);
        TeamModel CreateTeam(TeamModel team);
        List<PersonModel> GetPerson_All();
    }
}
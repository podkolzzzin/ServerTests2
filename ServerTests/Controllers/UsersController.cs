using Server.ItSelf;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTests.Controllers
{
    public record User(string Name, string Surname, string Login);

    public class UsersController : IController
    {
        public User[] Users()
        {
            Thread.Sleep(5);
            return new[]
            {
                new User("Andrii", "Podkolzin", "WORLDKing"),
                new User("Andrii", "Podkolzin", "Zorro"),
                new User("Andrii", "Podkolzin", "podkolzzzin")
            };
        }

        public async Task<User[]> UsersAsync()
        {
            await Task.Delay(5);
            return new[]
            {
                new User("Andrii", "Podkolzin", "WORLDKing"),
                new User("Andrii", "Podkolzin", "Zorro"),
                new User("Andrii", "Podkolzin", "podkolzzzin")
            };
        }
    }
}

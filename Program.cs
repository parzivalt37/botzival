using System.Threading.Tasks;
using botzival.Services;


namespace Botzival
{
    class Program
    {
        static async Task Main(string[] args) => await new DiscordService().InitializeAsync();
    }
}
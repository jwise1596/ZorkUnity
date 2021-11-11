using System;
using System.IO;
using Newtonsoft.Json;
using Zork.Common;

namespace Zork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string defaultGameFilename = "Zork.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultGameFilename);

            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(gameFilename));

            ConsoleOutputService output = new ConsoleOutputService();
            ConsoleInputService input = new ConsoleInputService();

            Game.StartFromFile(gameFilename, output, input);

            while (game.IsRunning)
            {
                output.WriteLine(game.Player.Location);
                output.Write("\n> ");
                input.GetInput();
            }

            output.WriteLine("Thank you for playing!");
            
        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}
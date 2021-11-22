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

            ConsoleInputService input = new ConsoleInputService();
            ConsoleOutputService output = new ConsoleOutputService();

            output.WriteLine(string.IsNullOrWhiteSpace(game.WelcomeMessage) ? "Welcome to Zork!" : game.WelcomeMessage);
            game.Start(input, output);

            Room previousRoom = null;
            while (game.IsRunning)
            {
                game.Output.WriteLine($"In One Turn, Your Score Will Be: {game.Player.Score}");
                output.WriteLine(game.Player.Location);
                if (previousRoom!= game.Player.Location)
                {
                    Game.Look(game);
                    previousRoom = game.Player.Location;
                }
                output.Write("\n> ");
                input.ProcessInput();
            }

            output.WriteLine("Thank you for playing!");
        }

        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}
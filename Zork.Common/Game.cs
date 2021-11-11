using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
using Zork.Common;

namespace Zork
{
    public class Game : INotifyPropertyChanged
    {
        private string commandString;

        public event PropertyChangedEventHandler PropertyChanged;

        public World World { get; private set; }

        public string StartingLocation { get; set; }
        
        public string WelcomeMessage { get; set; }
        
        public string ExitMessage { get; set; }

        [JsonIgnore]
        public IOutputService Output { get; set; }
        [JsonIgnore]
        public IInputService Input { get; set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; private set; }

        private bool IsRestarting { get; set; }

        [JsonIgnore]
        public Dictionary<string, Command> Commands { get; private set; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;

            Commands = new Dictionary<string, Command>()
            {
                { "QUIT", new Command("QUIT", new string[] { "QUIT", "Q", "BYE" }, Quit) },
                { "LOOK", new Command("LOOK", new string[] { "LOOK", "L" }, Look) },
                { "NORTH", new Command("NORTH", new string[] { "NORTH", "N" }, game => Move(game, Directions.North)) },
                { "SOUTH", new Command("SOUTH", new string[] { "SOUTH", "S" }, game => Move(game, Directions.South)) },
                { "EAST", new Command("EAST", new string[] { "EAST", "E"}, game => Move(game, Directions.East)) },
                { "WEST", new Command("WEST", new string[] { "WEST", "W" }, game => Move(game, Directions.West)) },
            };
        }

        public static void StartFromFile(string gameFilename, IOutputService output, IInputService input)
        {
            if (!File.Exists(gameFilename))
            {
                throw new FileNotFoundException("Expected File.", gameFilename);
            }

            Load(File.ReadAllText(gameFilename), output, input);
        }

        public static Game Load(string gamejsonString, IOutputService output, IInputService input)
        {
           Game game = JsonConvert.DeserializeObject<Game>(gamejsonString);
           game.Player = game.World.SpawnPlayer();
           game.Output = output;
           game.Input = input;
           game.IsRunning = true;
           game.Input.InputReceived += game.InputReceivedHandler;

           return game;
         }

        private void InputReceivedHandler(object sender, string inputString)
        {
            {
                Output.WriteLine(string.IsNullOrWhiteSpace(WelcomeMessage) ? "Welcome to Zork!" : WelcomeMessage);

                //Output = output;
                Assert.IsNotNull(Output);

                //Input = input;
                Assert.IsNotNull(Input);

                Output.WriteLine(WelcomeMessage);

                Command foundCommand = null;
                Room previousRoom = Player.Location;

                foreach (Command command in Commands.Values)
                    {
                        if (command.Verbs.Contains(commandString))
                        {
                            foundCommand = command;
                            break;
                        }

                        if (previousRoom != Player.Location)
                        {
                            Look(this);
                        }
                    }

                if (foundCommand != null)
                    {
                        foundCommand.Action(this);
                    }
                    else
                    {
                        Output.WriteLine("Unknown command.");
                    }
            }
        }

        public void Restart()
        {
           IsRunning = false;
           IsRestarting = true;
           Output.Clear();
        }

        private void Look(Game game)
        {
            Output.WriteLine(game.Player.Location.Description);
        }
        private void Move(Game game, Directions direction)
        {

            if (game.Player.Move(direction) == false)
            {
                Output.WriteLine("The way is shut!");
            }
        }

        private static void Quit(Game game) => game.IsRunning = false;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Player = new Player(World, StartingLocation);

    }
}
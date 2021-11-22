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
        public event PropertyChangedEventHandler PropertyChanged;

        public World World { get; private set; }

        public string StartingLocation { get; set; }

        public string WelcomeMessage { get; set; }
        
        public string ExitMessage { get; set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; private set; }

        public IInputService Input { get; set; }

        public IOutputService Output { get; set; }

        [JsonIgnore]
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
                { "REWARD", new Command("REWARD", new string[] { "REWARD", "R" }, Reward) },
                { "NORTH", new Command("NORTH", new string[] { "NORTH", "N" }, game => Move(game, Directions.North)) },
                { "SOUTH", new Command("SOUTH", new string[] { "SOUTH", "S" }, game => Move(game, Directions.South)) },
                { "EAST", new Command("EAST", new string[] { "EAST", "E"}, game => Move(game, Directions.East)) },
                { "WEST", new Command("WEST", new string[] { "WEST", "W" }, game => Move(game, Directions.West)) },
            };
        }

        public void Start(IInputService input, IOutputService output)
        {
            Assert.IsNotNull(input);
            Input = input;
            Input.InputReceived += InputReceivedHandler;

            Assert.IsNotNull(output);
            Output = output;

            IsRunning = true;
        }

        private void InputReceivedHandler(object sender, string commandString)
        {
            Command foundCommand = null;
            foreach (Command command in Commands.Values)
            {
                if (command.Verbs.Contains(commandString))
                {
                    foundCommand = command;
                    break;
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

        public void Restart()
        {
           IsRunning = false;
           IsRestarting = true;
           Output.Clear();
        }

        private static void Move(Game game, Directions direction)
        {
            if (game.Player.Move(direction) == false)
            {
                game.Output.WriteLine("The way is shut!");
            }
            else
            {
                game.Output.WriteLine(game.Player.Location.Description);
            }
        }

        public static void Look(Game game) => game.Output.WriteLine(game.Player.Location.Description);        
        private static void Quit(Game game) => game.IsRunning = false;
        public static void Reward(Game game) => game.Player.Score = game.Player.Score + 5;
        public static void Score(Game game) => game.Output.WriteLine($"In One Turn, Your Score Will Be: {game.Player.Score}");

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Player = new Player(World, StartingLocation);
    }
}
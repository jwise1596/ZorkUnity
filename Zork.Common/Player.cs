using Newtonsoft.Json;

namespace Zork
{
    public class Player
    {
        public World World { get; }

        public string StartingLocation { get; set; }

        [JsonIgnore]
        public Room Location { get; private set; }

        public Player(World world, string startingLocation)
        {
            Assert.IsTrue(world != null);
            Assert.IsTrue(world.RoomsByName.ContainsKey("West of House"));

            World = world;
            Location = world.RoomsByName["West of House"];
        }

        public bool Move(Directions direction)
        {
            bool isValidMove = Location.Neighbors.TryGetValue(direction, out Room destination);
            if (isValidMove)
            {
                Location = destination;
            }

            return isValidMove;
        }
    }
}
using System;
using System.IO;
using cc3k.Attributes;
using cc3k.Menus;
using cc3k.Entities;
using cc3k.Entities.Monsters;
using cc3k.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cc3k 
{
    public class Program
    {
        public static readonly Random RandomGenerator = new Random(2);

        public static void Main(string[] args)
        {
            var board = new GameBoard("./res/floor.txt");

            var play = true;
            while (play)
            {

                Player player = null;
                if (File.Exists("./res/save.txt")) //read save file
                {
                    string[] saveData = File.ReadAllLines("./res/save.txt");
                    foreach (string save in saveData)
                    {
                        JObject? deserialized = (JObject?)JsonConvert.DeserializeObject(save);
                        MapObjectType objType = (MapObjectType)Enum.Parse(typeof(MapObjectType),(string)deserialized["ObjectType"]);
                        if (objType == MapObjectType.Player)
                        {
                            player = Player.Deserialize(save, board);
                            board.SpawnObject(player);
                        }
                        else if (objType == MapObjectType.Monster)
                        {
                            Monster monster = Monster.Deserialize(save, board);
                            board.SpawnObject(monster);
                        }
                        else if (objType == MapObjectType.Item)
                        {
                            GameItem item = GameItem.Deserialize(save, board);
                            board.SpawnObject(item);
                        }
                        else if (objType == MapObjectType.Inventory)
                            player.DeserializeInventory(save);
                        else
                            throw new ArgumentException("Invalid MapObjectType", nameof(objType));
                    }
                }


                else //no save file found
                {
                    RaceSelectionMenu menu = new RaceSelectionMenu();
                    menu.Display();

                    player = new Player(board, menu.SelectedRace);
                    board.SpawnAll(player, false);
                }

                board.Play(player);

                Console.Clear();
                Console.WriteLine("game has ended, play again? y/n");
                string temp = Console.ReadLine();
                if (!temp.ToLower().StartsWith("y"))
                {
                    play = false;
                }
            }
            Console.WriteLine("see ya");
        }
    }
}
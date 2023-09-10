using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using cc3k.Attributes;
using cc3k.Items;
using cc3k.Menus;
using cc3k.Entities.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cc3k.Entities
{
    public class Player : IEntity, IMapObject
    {
        public PlayerRace Race { get; private set; }
        private int _gold;
        public int Gold
        {
            get { return _gold; }
            set
            {
                int changingBy = value - _gold;
                if (changingBy > 0)
                {
                    if (Race == PlayerRace.Dwarf) changingBy = changingBy * 2;
                    else if (Race == PlayerRace.Orc) changingBy = changingBy / 2;
                    Actions.Add($"{changingBy.ToString()} gold picked up");
                }
                _gold = Math.Max(0, _gold + changingBy);

            }
        }
        public int Level { get; private set; }
        public List<string> Actions { get; private set; }

        private int _health;
        public int Health
        {
            //get; private set;
            get { return _health; }
            set
            {
                _health = Math.Max(0, Math.Min(value, MaxHealth)); // 0 <= value <= maxhealth
            }
        }
        public int MaxHealth { get; private set; }

        public bool IsElf { get { return Race == PlayerRace.Elf; } }
        public bool IsOrc { get { return Race == PlayerRace.Orc; } }
        public bool IsDwarf { get { return Race == PlayerRace.Dwarf; } }

        public int X { get; private set; }
        public int Y { get; private set; }
        public GameBoard Board { get; private set; }
        public char MapSymbol { get { return '@'; } }
        public MapObjectType ObjectType { get { return MapObjectType.Player; } }
        public ConsoleColor ObjectColor { get { return ConsoleColor.White; } }

        public int BaseAttack { get; private set; }
        public int FloorAttack { get; set; }
        public int Attack { get { return BaseAttack + FloorAttack; } }

        public int BaseDefense { get; private set; }
        public int FloorDefense { get; set; }
        public int Defense { get { return BaseDefense + FloorDefense; } }

        public bool IsDead { get { return Health <= 0; } }

        public int FloorNumber { get; set; }
        public List<GameItem> Inventory { get; private set; }
        public bool IsSerializable { get { return true; } }

        public Player(GameBoard board, PlayerRace race)//constructor
        {
            Actions = new List<string>();
            Inventory = new List<GameItem>();

            Board = board;
            Race = race;

            StatsAttribute stats = race.GetAttribute<StatsAttribute, PlayerRace>();

            MaxHealth = stats.Health;
            Health = stats.Health;
            BaseAttack = stats.Attack;
            BaseDefense = stats.Defense;
            Gold = 20; //shop tester

        }

        public void Spawn()
        {
            if (X == 0 && Y == 0)
            {
                FloorNumber = 1;
                int rX, rY;
                Board.GenerateUnoccupied(GameBoard.GetRandomRoom(), out rY, out rX);

                X = 14;
                Y = 15;
            }

            Actions.Add("Player has spawned");

        }

        public void Print()
        {
            Console.WriteLine($"Race: {Race}  Gold: {Gold}  HP: {Health}  ATK: {Attack}  DEF: {Defense}".PadRight(69, ' ') + $"Floor {FloorNumber}");
            //consoladation makes moves less jarring

            string action = string.Join(", ", Actions);
            Console.WriteLine($"Action: {action}");
        }
        public bool UseBoardPotion(string direction)
        {
            int ox, oy;
            GameBoard.GetDirectionOffset(direction, out ox, out oy);

            int potionX = this.X + ox;
            int potionY = this.Y + oy;

            IMapObject search = Board.GetObjectAt(potionY, potionX);

            if (search == null)
                throw new MenuException("potion not found, move closer?");
            if (search.ObjectType != MapObjectType.Item)
                throw new MenuException("you cannot use that, try looking for a potion");
            GameItem item = (GameItem)search;
            if (!item.IsPotion)
                throw new MenuException("try using a potion...");

            item.Use(this);

            Board.DespawnObject(item);
            Actions.Add($"{item.Type.ToString()} potion used");
            return true;
        }

        public void RemovePotion(int index)
        {
            Inventory.RemoveAt(index);
        }
        public bool Move(string direction)
        {
            int tempX, tempY;
            GameBoard.GetDirectionOffset(direction, out tempX, out tempY);
            tempX = X + tempX;
            tempY = Y + tempY;

            char[] legal = new char[] { '.', '+', '#' };
            MoveError error;
            if (!Board.IsMovable(tempY, tempX, legal, out error)) //player can step on gold, staircase + potion
            {
                if (error == MoveError.MonsterOccupied)
                {
                    throw new MenuException("cannot move on top of a monster, try attacking it");
                }
                else if (error == MoveError.Wall)
                {
                    throw new MenuException("cannot move onto a wall, try another direction");
                }
                else if (error == MoveError.InvalidPath)
                {
                    throw new MenuException("stay on the path, nothing is there");
                }
                return false;
            }

            var obj = Board.GetObjectAt(tempY, tempX); // is something at where we're moving to?
            if (obj != null && obj.ObjectType == MapObjectType.Item)
            {
                GameItem item = (GameItem)obj;
                item.Pickup(this);
            }

            Actions.Add($"player moved {direction}");
            X = tempX;
            Y = tempY;

            int potionCount = Board
                .GetObjectsNearby(this)
                .Where(obj => (obj.ObjectType == MapObjectType.Item) && ((GameItem)obj).IsPotion)
                .Count();
            if (potionCount == 1) Actions.Add($"unknown potion nearby");
            else if (potionCount > 1) Actions.Add($"{potionCount} unknown potions nearby");
            return true;
        }
        public void ReceiveDamage(int damage)
        {
            Health = Health - damage;
        }
        public bool AttackMonster(string direction)
        {
            int mX, mY;
            GameBoard.GetDirectionOffset(direction, out mX, out mY);

            IMapObject search = Board.GetObjectAt(mY + Y, mX + X);
            if (search == null)
                throw new MenuException("enemy not found, move closer?");

            if (search.ObjectType != MapObjectType.Monster)
                throw new MenuException("you can only attack monsters");

            Monster monster = (Monster)search;
            int damage = (int)Math.Ceiling(100.0 / (100.0 + monster.Defense) * this.BaseAttack);


            Actions.Add($"PC deals {damage} to {monster.MapSymbol}");
            monster.ReceiveDamage(damage, this);
            return true;
        }
        public JObject Serialize()
        {
            return JObject.FromObject(new
            {
                ObjectType,
                Race, // RAce = Race,
                Health, // Health = Helath,
                FloorAttack,
                FloorDefense,
                X,
                Y,
                FloorNumber
            });
        }
        /*public string SerializeInventory()
        {
            string temp = $"{ObjectType}";
            if (Inventory.Count > 0)
            {
                foreach (GameItem item in Inventory)
                {
                    if (item.IsPotion)
                    {
                        Potion potion = (Potion)item;
                        temp += $"~{potion.Type}~{potion.IsIdentified}";
                    }
                    else
                        temp += $"~{item.Type}~False";
                    //other items would not be identified, but maybe used/applied BOOL? 
                }
            }
            return temp;

            //"player" if empty
            //"player~dec..." if not
        }*/
        public void Deserialize(JObject deserialized)
        {

            Health = (int)deserialized[nameof(Health)];
            FloorAttack = (int)deserialized[nameof(FloorAttack)];
            FloorDefense = (int)deserialized[nameof(FloorDefense)];
            X = (int)deserialized[nameof(X)];
            Y = (int)deserialized[nameof(Y)];
            FloorNumber = (int)deserialized[nameof(FloorNumber)];
        }
        public void DeserializeInventory(string serial)
        {
            //only called if serial starts w "Inventory"? only possible if not empty

            //string[] temp = serial.Split('~');
            ///string[] temp2 = new string[temp.Length];
            //Array.Copy(temp, 1, temp2, 0, temp2.Length);
            //GameItem item =  GameItem.Deserialize(String.Join('~',temp2), Board);

            JObject? deserialized = (JObject?)JsonConvert.DeserializeObject(serial);
            GameItemType type = (GameItemType)Enum.Parse(typeof(GameItemType), (string)deserialized["Type"]);
            if (GameItem.PotionTypes.Contains(type))
            {
                Potion item = new Potion(Board, type);
                Inventory.Add(item);
            }
            else
            {
                //whatever new items, need to be created and added here
                //Inventory.Add(item);
            }

        }
        public static Player Deserialize(string serial, GameBoard board)
        {
            JObject? deserialized = (JObject?)JsonConvert.DeserializeObject(serial);
            PlayerRace race = (PlayerRace)Enum.Parse(typeof(PlayerRace), (string)deserialized[nameof(Race)]);
            Player player = new Player(board, race); 
            
            /*string[] temp = serial.Split('~');
            Player player;
            PlayerRace race = (PlayerRace)Enum.Parse(typeof(PlayerRace), race[1]);
            player = new Player(board, race);*/

            player.Deserialize(deserialized);
            return player;
        }
    }
}               
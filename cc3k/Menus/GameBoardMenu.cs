using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using cc3k.Attributes;
using cc3k.Entities;
using cc3k.Entities.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cc3k.Menus
{
    public class GameBoardMenu :GameMenu
    {
        public GameBoard Board { get; private set; }
        public Player Player { get; private set; }


        private Dictionary<string, MethodInfo> _commands;


        public GameBoardMenu(GameBoard board)
            :base()
        {
            _commands = new Dictionary<string, MethodInfo>();

            Board = board;
            Player = board.GetPlayer();
            Initialize();
        }
        private void Initialize()
        {
            //i dont understand why this exists?

            //stores the prefixes for commands in _commands 
            //this prevents the need to search through typeof EVERY time
            Type type = typeof(GameBoardMenu);
            MethodInfo[] method = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo m in method)
            {
                CommandAttribute? attribute = m.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    for (int i = 0; i < attribute.Prefix.Length; i++)
                    {
                        _commands[attribute.Prefix[i]] = m;
                    }
                }
            }
        }
        protected override void DisplayMenu()
        {
            Board.Print();
            Player.Print();
        }

        [Command("inv" ,"inventory")][Description("Show inventory")]
        private bool InventoryCommand(string[] actionsArray)
        {
            if (Player.Inventory.Count == 0)
                throw new MenuException("nothing in inventory");
            Board.ActiveMenu = new PlayerInventoryMenu(Player);
            return false;
        }
        [Command("q")][Description("Quit game")]
        private bool QuitCommand(string[] actionsArray)
        {
            Active = false;
            return false;
        }
        [DirectionCommand][Description("Move around")]
        private bool MoveCommand(string[] actionsArray)
        {

            string direction = actionsArray[0];
            Player.Move(direction);
            return true;
        }
        [Command("a")][Description("Attack monster")]
        private bool AttackCommand(string[] actionsArray)
        {
            if (actionsArray.Length <= 1)
                throw new MenuException("no direction provided");
            string direction = actionsArray[1];
            if (!GameBoard.Directions.Contains(direction))
                throw new MenuException("Invalid direction\ntry no, ea, so, we, ne, nw, se, or sw");
            if (Player.AttackMonster(direction))
                return true;
            return false;
        }
        [Command("t")][Description("Talk to merchant")]
        private bool TalkCommand(string[] actionsArray)
        {
            if (actionsArray.Length <= 1)
                throw new MenuException("no direction provided");
            string direction = actionsArray[1];
            if (!GameBoard.Directions.Contains(actionsArray[1]))
                throw new MenuException("Invalid direction\ntry no, ea, so, we, ne, nw, se, or sw");

            int dX, dY;
            GameBoard.GetDirectionOffset(direction, out dX, out dY);
            IMapObject search = Board.GetObjectAt(dY + Player.Y, dX + Player.X);
            if (search == null)
                throw new MenuException("nothing is there");

            Merchant? merchant = search as Merchant; //returns null if cannot cast
            if (merchant == null)
                throw new MenuException("try talking to a merchant");

            if (!merchant.IsShopOpen)
                throw new MenuException("that merchant shop is closed");

            Board.ActiveMenu = new MerchantMenu(merchant, Board);
            return false;
        }

        [Command("u")][Description("Use potion seen on board")]
        private bool UseCommand(string[] actionsArray)
        {
            if (actionsArray.Length <= 1)
                throw new MenuException("no direction provided");
            string direction = actionsArray[1];
            if (!GameBoard.Directions.Contains(direction))
                throw new MenuException("Invalid direction\ntry no, ea, so, we, ne, nw, se, or sw");

            if (Player.UseBoardPotion(direction))
                return true; //time will pass
            return false;
        }
        [Command("help")][Description("open the help menu that you're reading now")]
        private bool HelpCommand(string[] actionsArray)
        {
            Board.ActiveMenu = new HelpMenu(Board.GetPlayer()); ;
            return false;
        }
        [Command("save")][Description("save current game and quit")]
        private bool SaveCommand(string[] actionsArray)
        {
            StreamWriter writer;
            writer = File.CreateText("./res/save.txt");

            foreach( var obj in Board.Objects)
            {
                if (obj.IsSerializable)
                    writer.WriteLine(JsonConvert.SerializeObject(obj.Serialize()));
            }

            foreach (var item in Player.Inventory)
            {
                JObject serialized = item.Serialize();
                serialized["ObjectType"] = (int)MapObjectType.Inventory;
                writer.WriteLine(JsonConvert.SerializeObject(serialized));
            }
            

            writer.Close(); //closes from further input

            Player.Actions.Add("game has saved");
            return false;
            //NEED TO END GAME?
        }
        protected override void HandleInput()
        {
            Console.Write("Input: ");
            string? action = Console.ReadLine();
            string[]? actionsArray = action.Split(' ');
            bool? performedAction = null;

            //clear player's action/"history"
            Player.Actions.Clear();

            // prefix -> MethodInfo
            // string (word) -> int (number of times it occured)

            string prefix = actionsArray[0];
            if (!_commands.ContainsKey(prefix))
            {
                throw new MenuException("Invalid command, try typing \"help\"");
            }
            else
            {
                MethodInfo m = _commands[prefix];
                try
                {
                    performedAction = (bool?)m.Invoke(this, new object[] { actionsArray });
                }
                catch (TargetInvocationException error)
                {
                    if (error.InnerException != null)
                        throw error.InnerException;
                }
                
            }
            
            //time passes in game
            if (performedAction == true)
                Board.Tick();
        }
    }
}

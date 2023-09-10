using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Items;
using cc3k.Entities;
using cc3k.Entities.Monsters;

namespace cc3k.Menus
{
    public class MerchantMenu : GameMenu
    {
        public Merchant Merchant { get; private set; }
        public Player Player { get; private set; }
        public GameBoard Board { get; private set; }
        public MerchantMenu(Merchant merchant, GameBoard board)
            : base()
        {
            Merchant = merchant;
            Board = board;
            Player = board.GetPlayer();
        }
        protected override void DisplayMenu()
        {
            Console.WriteLine("Welcome to the Merchant's shop");
            Console.WriteLine();
            Console.WriteLine($"PC has {Player.Gold} gold");
            Console.WriteLine();
            Console.WriteLine("what item would you like to buy? \n");
            Console.WriteLine("1. Increase Health - restores 10 HP \n \t cost: 10 gold");
            Console.WriteLine("2. Increase Attack - increase ATK by 5 \n \t cost: 10 gold");
            Console.WriteLine("3. Increase Defense - increase DEF by 5 \n \t cost: 5 gold \n");

            Console.WriteLine($"if PC would like to use Merchant's potion identification service, input \"p\"");
            Console.WriteLine("using only the potion service will not count as buying from the merchant");
            Console.WriteLine();
        }
        protected override void HandleInput()
        {
            Console.Write("select the respective number or \"z\" to leave: ");
            string? input = Console.ReadLine();
            bool? wasItemSold = null;

            if (input.StartsWith("z"))
            {
                Player.Actions.Add("merchant shop is now closed");
                Active = false;
            }
            else if (input == "1") wasItemSold = Merchant.SellItem(Player, GameItemType.IncHealth);
            else if (input == "2") wasItemSold = Merchant.SellItem(Player, GameItemType.IncAttack);
            else if (input == "3") wasItemSold = Merchant.SellItem(Player, GameItemType.IncDefense);
            else if (input == "p")
            {
                if (Player.Inventory.Count <= 0)
                    throw new MenuException("PC has nothing in inventory, try picking up a potion");

                Board.ActiveMenu = new MerchantPotionMenu(Merchant, Board); 
                //menu in a menu xd
                ////////////////////////////////////////////////////////////

                Board.ActiveMenu.Display(); //calls parent method?

            }

            if (wasItemSold == false)
                throw new MenuException("Insufficient gold");
            else if (wasItemSold == true)
            {
                Notification = "potion was bought and applied";
            }
            else if (input == "p")
                Notification = "potion service is now closed";
            else
                Notification = "Error: input a number for respective potion";
        }
    }
}

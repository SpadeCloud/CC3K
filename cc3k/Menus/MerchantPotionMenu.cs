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
    public class MerchantPotionMenu : GameMenu
    {
        public Merchant Merchant { get; private set; }
        public Player Player { get; private set; }
        public GameBoard Board { get; private set; }
        public MerchantPotionMenu(Merchant merchant, GameBoard board)
            :base()
        {
            Merchant = merchant;
            Board = board;
            Player = board.GetPlayer() ;
        }
        protected override void DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Merchant's potion identification service");
            Console.WriteLine(); 
            Console.WriteLine($"PC has {Player.Gold} gold");
            Console.WriteLine();

            for (int i = 0; i < Player.Inventory.Count; i++)
            {
                GameItem item = Player.Inventory[i];
                if(!item.IsPotion)
                    continue;

                Potion p = (Potion)item;
                if (p.IsIdentified)
                {
                    Console.WriteLine($"{i+1}. {p.Type}");
                }
                else if (!p.IsIdentified)
                {
                    Console.WriteLine($"{i+1}. unidentified potion");
                }
            }

            Console.WriteLine("\neach identification costs 7 gold");
            Console.WriteLine();

        }

        protected override void HandleInput()
        {
            Console.WriteLine("\nselect the respective number for the potion PC wants to identify");
            Console.Write("\t.. or select the respective number or \"x\" to leave: ");
                //quit to normal merchant menu
            string? input = Console.ReadLine();

            int invCount = Player.Inventory.Count; // invcount= input, invcount-1= index
            int inputNumber;
            bool sorting = int.TryParse(input, out inputNumber);
            bool? wasPotionIdentified = null;
            bool repeatedAction = false;

            if (input == null)
                throw new MenuException("input is never null");
            //^ never actually hits, just to disable annoying green ~~ (this could be null)

            if (input.StartsWith("x"))
            {
                foreach (string s in Player.Actions)
                {
                    if (s == "potion serivce is now closed")
                        repeatedAction = true;
                    //no duplicate merchantpotionmenu closing message
                }
                if (!repeatedAction)
                    Player.Actions.Add("potion serivce is now closed");
                Active = false;
                //breaks
            }
            else if (!sorting) //anything other than 'x' or # is an error
                throw new MenuException("if PC doesnt need potion identification, just leave with \"x\"");
            else if (inputNumber > invCount || inputNumber < 1)
                throw new MenuException("try an acutal potion option/number");
            else //PC inputs correctly
                wasPotionIdentified = Merchant.IdentifyService(Player, inputNumber - 1);


            if (wasPotionIdentified == false)
                throw new MenuException("Insufficient gold, try slaying a monster or something");
            else if (wasPotionIdentified == true)
            {

                Notification = "potion was sucessfully identified";
            }

        }
    }
}

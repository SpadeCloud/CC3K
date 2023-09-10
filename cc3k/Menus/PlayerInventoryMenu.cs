using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Items;
using cc3k.Entities;

namespace cc3k.Menus
{
    public class PlayerInventoryMenu : GameMenu
    {
        public Player Player { get; private set; }

        public PlayerInventoryMenu(Player player)
            : base()
        {
            Player = player;
        }
        protected override void DisplayMenu()
        {
            Console.WriteLine("Welcome to your inventory\n");
            Console.WriteLine($"PC has {Player.Gold} gold");
            Console.WriteLine($"PC has {Player.Inventory.Count} items in inventory \n");
            for (int i = 0; i < Player.Inventory.Count; i++)
            {
                GameItem item = Player.Inventory[i];
                Console.WriteLine($"{i + 1}. {item.ToString()}");
                //potion has an override, all else would print... how?
            }
        }
        protected override void HandleInput()
        {
            Console.Write("\nto use potion, input number for associated potion...\nor \"z\" to leave: ");
            string? input = Console.ReadLine();
            if (input == "z")
            {
                Player.Actions.Add("inventory now closed");
                Active = false;
            }

            int optionNumber;
            if (!int.TryParse(input, out optionNumber)) 
                throw new MenuException("Select a valid option");

            if ((optionNumber > 0) && (optionNumber <= Player.Inventory.Count))
            {
                GameItem item = Player.Inventory[optionNumber - 1];
                // maybe other items will be used?
                //potentials for else if 
                if (item.IsPotion)
                {
                    Potion selectedPotion = (Potion)item;
                    
                    Notification = $"{selectedPotion.Type} was used";
                    selectedPotion.Use(Player);
                    Player.RemovePotion(optionNumber - 1);

                    if (Player.Inventory.Count == 0)
                        Notification = Notification + ", pc has no more potions";
                }
                else
                    throw new MenuException("this item cannot be used");
            }
        }
    }
}

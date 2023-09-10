using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using cc3k.Attributes;
using cc3k.Entities;

namespace cc3k.Menus
{
    public class RaceSelectionMenu : GameMenu
    {
        public PlayerRace SelectedRace { get; private set; }

        public RaceSelectionMenu()
            :base()
        {
            SelectedRace = PlayerRace.Human;
        }
        protected override void DisplayMenu()
        {
            Console.WriteLine("game has started \nwhat race will you select?");
            Console.WriteLine("options are");

            PlayerRace[] races = Enum.GetValues<PlayerRace>();
            
            for (int i = 0; i < races.Length; i++)
            {
                StatsAttribute stats = races[i].GetAttribute<StatsAttribute, PlayerRace>();
                DescriptionAttribute description = races[i].GetAttribute<DescriptionAttribute, PlayerRace>();
              
                Console.WriteLine($"{i+1}. {races[i]}:".PadRight(10, ' ')+ stats.ToString() + $" {description.Description}");
            }
        }
        protected override void HandleInput()
        {
            PlayerRace[] races = Enum.GetValues<PlayerRace>();
            Console.Write("select the respective number: ");
            string? input = Console.ReadLine();
            int classChose;
            if (!int.TryParse(input, out classChose)) //forces number selection
                throw new MenuException("Select using a valid number");
            else
            {
                if (classChose < 1 || classChose > races.Length) // NOT 1-#of raaces
                    throw new MenuException("pick a number representing a class within the list");

                else //forces number within the list
                {
                    string prefix = "a";
                    char firstLetter = races[classChose - 1].ToString()[0];
                    char[] vowels = new char[] { 'A', 'E', 'I', 'O', 'U' };
                    if (vowels.Contains(firstLetter))
                    {
                        prefix = "an";
                    }
                    //Console.WriteLine($"you're now {prefix} {Player.Races[classChose - 1]}");
                    SelectedRace = races[classChose - 1];
                    Active = false; 
                    //ends menu for race selection
                }
            }
        }
    }
}

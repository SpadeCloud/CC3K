using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using cc3k.Attributes;
using cc3k.Entities;
using cc3k.Entities.Monsters;

namespace cc3k.Menus
{
    public class HelpMenu : GameMenu
    {
        public Player Player { get; private set; }
        public HelpMenu(Player player)
            : base()
        {
            Player = player;
        }
        protected override void DisplayMenu()
        {
            Console.WriteLine("welcome to the help screen");

            /////////////////////////////input info
            Console.WriteLine("Game commands/allowed inputs");
            Type type = typeof(GameBoardMenu);
            MethodInfo[] method = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo m in method)
            {
                CommandAttribute? attribute = m.GetCustomAttribute<CommandAttribute>();
                DescriptionAttribute? attribute2= m.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null && attribute2!=null)
                {
                    Console.Write("\t");
                    for(int i = 0; i < attribute.Prefix.Length; i++)
                    {
                        if( i == attribute.Prefix.Length - 1 )
                            Console.Write(attribute.Prefix[i]);
                        else
                            Console.Write(attribute.Prefix[i]+", ");
                    }
                    Console.Write(": ");
                    foreach (var title in attribute2.Description)
                    {
                        Console.Write(title);
                    }
                    Console.WriteLine();
                }
            }

            /////////////////////////////monster info
            Console.Write("Monster information/stats");
            MonsterRace[] race = Enum.GetValues<MonsterRace>();
            foreach(MonsterRace r in race)
            {
                var stats = r.GetAttribute<StatsAttribute, MonsterRace>();
                var des = r.GetAttribute<DescriptionAttribute, MonsterRace>();
                
                Console.WriteLine($"\n\t{r.ToString()[0]}/{r}: {stats.ToString()}");

                if(des.Description.Length > 0)
                Console.WriteLine($"\t{des.Description}");
            }




            Console.WriteLine(); //formatting

        }
        protected override void HandleInput()
        {
            Console.Write("select the respective number or \"z\" to leave: ");
            string? input = Console.ReadLine();
            if (input != null && input.StartsWith("z"))
            {
                Player.Actions.Add("help menu is now closed");
                Active = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k
{
    //where is this used??
    public class ClassInfo
    {
        public int Health, Attack, Defense;
        public string Race, Description;
        public ClassInfo(string race, int health, int attack, int defense, string description = "")
        {
            Race = race;
            Health = health;
            Attack = attack;
            Defense = defense;
            Description = description;
        }
          public override string ToString()
        {
            // Dwarf:  100 HP, 20 Atk, 30 Def, gold is doubled in value
            string temp = Race + ": ";
            temp = temp.PadRight(7, ' ') + Health + " HP, " + Attack + " Atk, " + Defense + " Def. " + Description;
            return temp;
        }
    }
}

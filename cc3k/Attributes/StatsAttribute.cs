using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using cc3k.Entities;


namespace cc3k.Attributes
{
    public class StatsAttribute :Attribute
    {
        public int Health { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }

        public StatsAttribute(int health, int attack, int defense)
        {
            Health = health;
            Attack = attack;
            Defense = defense;
        }

        public override string ToString()
        {
            //where does this get used... 
            string format = $"{this.Health} Hp, {this.Attack} Atk, {this.Defense} Def.";
            return format;
        }
    }
    
}

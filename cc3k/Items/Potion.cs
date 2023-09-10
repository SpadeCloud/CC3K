using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cc3k.Items
{
    public class Potion : GameItem
    {
        public override char MapSymbol { get { return 'P'; } }
        public override ConsoleColor ObjectColor { get { return ConsoleColor.Blue; } }

        private GameItemType _type;
        public override GameItemType Type { get { return _type; } }
        public bool IsIdentified { get; set; }
        public Potion(GameBoard board, GameItemType type)
            :base(board)
        {
            _type=type;
        }
        public override void Pickup(Player player)
        {
            player.Inventory.Add(this);
            player.Actions.Add($"PC picked up an unidentified potion");
            Board.DespawnObject(this);
        }
        public override string ToString()
        {
            if (IsIdentified)
                return Type.ToString();
            else
                return "unidentified potion";
        }
        public override void Use(Player player)
        {
            if (Type == GameItemType.IncHealth || (player.IsElf && Type == GameItemType.DecHealth))
            {
                player.Health += 10;
            }
            else if (Type == GameItemType.IncAttack || (player.IsElf && Type == GameItemType.DecAttack))
            {
                player.FloorAttack += 5;
            }
            else if (Type == GameItemType.IncDefense || (player.IsElf && Type == GameItemType.DecDefense))
            {
                player.FloorDefense += 5;
            }
            else if (Type == GameItemType.DecHealth)
            {
                int hurt = Math.Max(player.Health - 10, 1);
                player.Health = hurt;
            }
            else if (Type == GameItemType.DecAttack)
            {
                player.FloorAttack -= 5;
            }
            else if (Type == GameItemType.DecDefense)
            {
                player.FloorDefense -= 5;
            }
        }
        public override JObject Serialize()
        {
            // potions, type x,y, isIdentified
            JObject serialize = base.Serialize();
            serialize[nameof(IsIdentified)] = IsIdentified;
            return serialize;
        }
        public override void Deserialize(JObject deserialize)
        {
            base.Deserialize(deserialize);
            IsIdentified = (bool)deserialize[nameof(IsIdentified)];
        }
    }
}

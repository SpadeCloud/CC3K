using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Menus;
using cc3k.Entities;
using cc3k.Entities.Monsters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cc3k.Items
{
    public class Gold : GameItem
    {
        public override char MapSymbol { get { return 'G'; } }
        public override ConsoleColor ObjectColor { get { return ConsoleColor.Yellow; } }
        private GameItemType _type;
        public override GameItemType Type { get { return _type; } }
        private Dragon? _dragon;

        public Gold(GameBoard board, GameItemType type)
            :base(board)
        {
            _type = type;
        }

        public override void Pickup(Player player)
        {
            int g = 0;
            if (this.Type == GameItemType.NormalGold)
                g = 1;
            else if (this.Type == GameItemType.SmallHordeGold)
                g = 2;
            else if (this.Type == GameItemType.MerchantHordeGold)
                g = 4;
            else if (this.Type == GameItemType.DragonHordeGold)
            {
                if (_dragon != null && !_dragon.IsDead)
                    throw new MenuException("cannot pickup dragon's gold until you've slain it");
                g = 6;
            }
            player.Gold += g;
            Board.DespawnObject(this);
        }

        public override void Spawn()
        {
            base.Spawn();
            if (Type == GameItemType.DragonHordeGold) //dragonhorde spawns dragon
            {
                if (_dragon == null)
                    _dragon = new Dragon(Board, this);

                if (!_dragon.IsDead)
                    Board.SpawnObject(_dragon);
            }
        }
        public override JObject Serialize()
        {
            if (Type == GameItemType.DragonHordeGold)
            {
                JObject serialize = base.Serialize();
                serialize[nameof(_dragon)] = _dragon.Serialize();
                return serialize;
            }
            return base.Serialize();
        }
        public override void Deserialize(JObject deserialize)
        {
            base.Deserialize(deserialize);
            if (Type == GameItemType.DragonHordeGold)
            {
                JObject? dragonKey = (JObject?)deserialize[nameof(_dragon)];
                Dragon dragon = new Dragon(Board, this);
                dragon.Deserialize(dragonKey);
                _dragon = dragon;
            }
        }
    }
}

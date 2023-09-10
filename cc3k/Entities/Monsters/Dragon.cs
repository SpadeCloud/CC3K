using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Items;
using cc3k.Entities;
using Newtonsoft.Json.Linq;

namespace cc3k.Entities.Monsters
{
    public class Dragon : Monster
    {
        public GameItem DragonHorde { get; private set; }
        public override bool IsDragon {  get { return true; } }
        protected override int DropGoldAmount
        {
            get
            {
                return 0;
            }
        }
        private bool _isHostile;
        public override bool IsHostile
        {
            get
            {
                if (_isHostile)
                    return true;

                IMapObject[] nearby = Board.GetObjectsNearby(DragonHorde);
                foreach(IMapObject obj in nearby)
                {
                    if(obj.ObjectType==MapObjectType.Player)
                        return true;   
                }

                return false;
            }
        }
        public override bool IsSerializable { get { return false; } }
        public Dragon(GameBoard board, GameItem horde)
            :base(board, MonsterRace.Dragon)
        {
            DragonHorde = horde;
        }
        public override void Deserialize(JObject deserialized)
        {
            base.Deserialize(deserialized);
            _isHostile = (bool)deserialized[nameof(IsHostile)];
        }
        public override void Move()
        {
            //no move
        }
        public override void ReceiveDamage(int damage, Player player)
        {
            _isHostile = true;
            base.ReceiveDamage(damage, player);
        }
        public override void Spawn()
        {
            if (X == 0 && Y == 0)
            {
                while (true)
                {
                    int r = Program.RandomGenerator.Next(GameBoard.Directions.Length);
                    int gX, gY;
                    GameBoard.GetDirectionOffset(GameBoard.Directions[r], out gX, out gY);

                    if (Board.IsMovable(DragonHorde.Y + gY, DragonHorde.X + gX, new char[] { '.' }))
                    {
                        X = DragonHorde.X + gX;
                        Y = DragonHorde.Y + gY;
                        break;
                    }
                }
            }
        }
    }
}

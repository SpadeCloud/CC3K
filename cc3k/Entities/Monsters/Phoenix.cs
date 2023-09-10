using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Entities;

namespace cc3k.Entities.Monsters
{
    public class Phoenix : Monster
    {

        public Phoenix(GameBoard board) //****************
            :base(board, MonsterRace.Phoenix)
        {

        }

        public override void ReceiveDamage(int damage, Player player)
        {
            base.ReceiveDamage(damage, player);

            if (IsDead)
            {
                int rY, rX;
                RectangleBounds random = GameBoard.GetDifferentRandomRoom(this.X, this.Y);
                Board.GenerateUnoccupied(random, out rY, out rX);

                Board.SpawnObject(this);

                Y = rY;
                X = rX;
                //"re"spawn?
                player.Actions.Add($"{this.Race} has been reborn elsewhere");
            }

        }
    }
}

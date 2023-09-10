using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Entities;

namespace cc3k.Items
{
    public class Staircase :GameItem
    {
        public override char MapSymbol { get { return '\\'; } }
        public override ConsoleColor ObjectColor { get { return ConsoleColor.White; } }
        public override GameItemType Type { get { return GameItemType.Staircase; } }

        public Staircase(GameBoard board)
            :base(board)
        {
                 
        }
        public override void Spawn() //spawn in any room without PC
        {
            int rY, rX;
            Player player = Board.GetPlayer();
            RectangleBounds random = GameBoard.GetDifferentRandomRoom(player.X, player.Y);
            Board.GenerateUnoccupied(random, out rY, out rX);
            Y = rY;
            X = rX;
        }

        public override void Use(Player player)
        {
            player.Inventory.Clear();
            player.FloorAttack = 0;
            player.FloorDefense = 0;
            player.FloorNumber++;
            Board.SpawnAll(player, true);
        }
    }
}

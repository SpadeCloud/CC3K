using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Items;
using cc3k.Entities;
using cc3k.Menus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cc3k.Entities.Monsters

{
    public class Merchant : Monster
    {
        private static bool MerchantAnger = false;
        public bool IsShopOpen { get; private set; }
        public override bool IsHostile
        {
            get
            {
                return MerchantAnger;
            }
        }
        protected override int DropGoldAmount
        {
            get
            {
                return 4;
            }
        }
        public Merchant(GameBoard board)
            : base(board, MonsterRace.Merchant)
        {
            IsShopOpen = true;
        }
        public override void Spawn()
        {
            base.Spawn();
            Player player = Board.GetPlayer();
            if (player.FloorNumber == 1)
            {
                // spawning merchants on floor 1 implies a new game is starting
                MerchantAnger = false;
            }
        }
        public override void Move()
        {
            // do nothing
        }
        public override void ReceiveDamage(int damage, Player player)
        {
            MerchantAnger = true;
            base.ReceiveDamage(damage, player);
        }
        public bool SellItem(Player player, GameItemType type)
        {
            int cost = 0;
            if (type == GameItemType.IncHealth) cost = 10;
            else if (type == GameItemType.IncAttack) cost = 10;
            else if (type == GameItemType.IncDefense) cost = 5;

            if (player.Gold < cost)
                return false;

            player.Gold -= cost;

            Potion potion = new Potion(Board, type);
            potion.Use(player);

            IsShopOpen = false;
            return true;
        }
        public bool BuyItem(Player player, GameItemType type)
        //PC can sell unwanted potions 
        {

            return false;
        }//not implemented yet
        public bool IdentifyService(Player player, int index)
        //PC pays for potion to be identified
        {
            GameItem item = player.Inventory[index];
            if (!item.IsPotion)
                throw new MenuException("Item at that index is not a potion");

            Potion p = (Potion)item;
            int serviceFee = 7;
            if (player.Gold < serviceFee)
                return false;

            player.Gold -= serviceFee;
            p.IsIdentified = true;
            return true;
        }
        public override void Deserialize(JObject deserialized)
        {
            base.Deserialize(deserialized);
            MerchantAnger = (bool)deserialized[nameof(IsHostile)];
        }
    }
}


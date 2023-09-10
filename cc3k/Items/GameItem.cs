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
    public abstract class GameItem :IMapObject
    {
        public static readonly GameItemType[] PotionTypes =
        {
            GameItemType.IncHealth, GameItemType.IncAttack, GameItemType.IncDefense,
            GameItemType.DecHealth, GameItemType.DecAttack, GameItemType.DecDefense
        };
        public static readonly GameItemType[] GoldTypes =
        {
            GameItemType.NormalGold, GameItemType.SmallHordeGold, GameItemType.MerchantHordeGold,
            GameItemType.DragonHordeGold
        };

        public static GameItemType GeneratePotionType()
        {
            var rng = Program.RandomGenerator;
            int r = rng.Next(PotionTypes.Length);
            return PotionTypes[r];
        } //abstract generate
        public static GameItemType GenerateGoldType()
        {
            var rng = Program.RandomGenerator;
            int r = rng.Next(8);
            if (r < 5)
            {
                return GameItemType.NormalGold;
            }
            else if (r == 5)
            {
                return GameItemType.DragonHordeGold;
            }
            else
            {
                return GameItemType.SmallHordeGold;
            }
        }

        public int X { get; protected set; }
        public int Y { get; protected set; }
        public GameBoard Board { get; protected set; }
        public abstract char MapSymbol { get; }
        public MapObjectType ObjectType { get { return MapObjectType.Item; } }
        public abstract ConsoleColor ObjectColor { get; } 
        public abstract GameItemType Type { get; }
        public bool IsPotion
        {
            get
            {
                return PotionTypes.Contains(this.Type);
            }
        }
        public bool IsGold
        {
            get
            {
                return GoldTypes.Contains(this.Type);
            }
        }
        public bool IsStaircase
        {
            get
            {
                return (Type == GameItemType.Staircase);
            }
        }
        public bool IsSerializable { get { return true; } }

        public GameItem(GameBoard board) //cannot be called new?directly?
        {
            Board = board;
        }

        public virtual void Pickup(Player player)
        {
            Use(player);
        }

        public virtual void Use(Player player)
        {
            return;
        } //pretty much space holders

        public virtual void Spawn() //spawn logic for gold and potion
        {
            if (X == 0 && Y == 0)
            {
                int rY, rX;
                Board.GenerateUnoccupied(GameBoard.GetRandomRoom(), out rY, out rX);
                Y = rY;
                X = rX;
            }
        }
        public virtual JObject Serialize()
        {
            return JObject.FromObject(new
            {
                ObjectType,
                Type,
                X,
                Y
            });
        }
        public virtual void Deserialize(JObject deserialized)
        {
            X = (int)deserialized[nameof(X)];
            Y = (int)deserialized[nameof(Y)];
        }
        public static GameItem Deserialize(string serial, GameBoard board)
        {
            JObject? deserialized = (JObject?)JsonConvert.DeserializeObject(serial);
            GameItem item;
            GameItemType type = (GameItemType)Enum.Parse(typeof(GameItemType), (string)deserialized[nameof(Type)]);
            if (GoldTypes.Contains(type))
                item = new Gold(board, type);
            else if (PotionTypes.Contains(type))
                item = new Potion(board, type);
            else if (type == GameItemType.Staircase)
                item = new Staircase(board);
            else
                throw new ArgumentException("unknown type within serialized data", nameof(serial));

            item.Deserialize(deserialized);
            return item;
        }
    }
}

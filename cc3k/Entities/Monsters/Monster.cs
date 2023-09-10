using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using cc3k.Attributes;
using cc3k.Entities;
using cc3k.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace cc3k.Entities.Monsters
{
    public class Monster : IEntity, IMapObject
    {
        public static Monster Create(GameBoard board, MonsterRace monsterType)
        {
            if (monsterType == MonsterRace.Merchant) return new Merchant(board);
            else if (monsterType == MonsterRace.Phoenix) return new Phoenix(board);
            else if (monsterType == MonsterRace.Dragon) throw new ArgumentException("Use Dragon constructor directly", nameof(monsterType));
            else return new Monster(board, monsterType);

        }

        public MonsterRace Race { get; private set; }
        public int Health { get; protected set; }
        public int BaseAttack { get; private set; }
        public int Defense { get; private set; }
        public GameBoard Board { get; private set; }
        public int Y { get; protected set; }
        public int X { get; protected set; }
        public MapObjectType ObjectType { get { return MapObjectType.Monster; } }
        public ConsoleColor ObjectColor { get { return ConsoleColor.Red; } }
        public bool IsDead { get { return Health <= 0; } }

        protected virtual int DropGoldAmount
        {
            get
            {
                return 1;
            }
        }
        public bool IsMerchant
        {
            get
            {
                //if (Race == "Merchant") return true;
                //return false;

                return (Race == MonsterRace.Merchant);
            }
        }
        public virtual bool IsDragon
        {
            get
            {
                return false;
            }
        }
        public char MapSymbol
        {
            get
            {
                return Race.ToString()[0];
            }
        }

        public virtual bool IsHostile
        {
            get
            { 
                return true; // by default, monsters are hostile
            }
        }
        public virtual bool IsSerializable { get { return true; } }

        public static MonsterRace GenerateMonsterRace()
        {
            var rng = Program.RandomGenerator;
            int r = rng.Next(18);

            if (r < 4) return MonsterRace.Werewolf ; // 4/18
            else if (r < 7) return MonsterRace.Vampire; // 3/18
            else if (r < 12) return MonsterRace.Kobold; // 5/18
            else if (r < 14) return MonsterRace.Troll; // 2/18
            else if (r < 16) return MonsterRace.Phoenix; // 2/18
            else return MonsterRace.Merchant; // 2/18
        }

        // 20 enemies are spawned per oor. Every chamber is equally likely to spawn any particular monster (similarly for oor tiles)

        public Monster(GameBoard board, MonsterRace race)
        {
            Board = board;
            Race = race;

            //StatsAttribute stats = AttributesHelper.GetAttribute<StatsAttribute, MonsterRace>(race);
            var stats = race.GetAttribute<StatsAttribute, MonsterRace>();
            BaseAttack = stats.Attack;
            Defense = stats.Defense;
            Health = stats.Health;
        }

        public virtual void Spawn()
        {

            //pick random room, then coords not adj to cooridor 
            //ideally makes smallest room less crowded

            if (X == 0 && Y == 0)
            {
                int rY;
                int rX;

                while (true)
                {
                    Board.GenerateUnoccupied(GameBoard.GetRandomRoom(), out rY, out rX); //rng room and x,y
                    if (!IsAdjCorridor(rY, rX))
                    {
                        Y = rY;
                        X = rX;
                        break;
                    }
                }
            }
        }

        private Player GetNearbyPlayer()
        {
            IMapObject[] mapObjects = Board.GetObjectsNearby(this);
            foreach (IMapObject thing in mapObjects)
            {
                if (thing.ObjectType == MapObjectType.Player)
                {
                    Player player = (Player)thing;
                    return player;
                }
            }

            return null;
        }
        private bool IsAdjCorridor(int y, int x)//checks all directions
        {
            
            foreach (string dir in GameBoard.Directions)
            {
                int tempY, tempX;
                GameBoard.GetDirectionOffset(dir, out tempY, out tempX);
                if (Board.IsCorridor(y+tempY, x+tempX))
                    return true;
          
            }
           
            return false;
        }

        public virtual void Move()
        {
            Player player = GetNearbyPlayer();
            if (player != null) return;

            int rY, rX;
            while (true) //allows the rr 
            {
                int r = Program.RandomGenerator.Next(GameBoard.Directions.Length);
                string temp = GameBoard.Directions[r];
                GameBoard.GetDirectionOffset(temp, out rX, out rY);
                // ^ random direction gen

                char[] legal = new char[] { '.' }; //only allows monsters on '.' without overlaps
                if (Board.IsMovable(rY + Y, rX + X, legal))
                {

                    if (!IsAdjCorridor(rY + Y, rX + X))//when no nearby coord
                        //potential problem
                    {  //rr until no nearby coord and nothing already occupying 
                        //could cause problem in smallest room/cluster of monsters
                        //potential solution: change spawn odds

                        X = X + rX;
                        Y = Y + rY;
                        break; //if no nearby cooridor, assign and break loop
                    }

                } 
            }        
        }

        public void Attack()
        {
            if (!IsHostile) return; 
            var player = GetNearbyPlayer();
            if(player == null) return;

            bool? tester = Board.IsMovable(player.Y, player.X, new char[] { '.' });

            if (!Board.IsCorridor(player.Y,player.X)) //only attack if player is in room
            {
                int miss = Program.RandomGenerator.Next(2); // 50% chance to miss attack
                if (miss == 0)
                {
                    player.Actions.Add($"{this.Race} has missed");
                    return;
                }
                int damage = (int)Math.Ceiling(100.0 / (100.0 + player.Defense) * this.BaseAttack);

                if (damage > 0)
                {
                    player.ReceiveDamage(damage);
                    player.Actions.Add($"{this.MapSymbol} deals {damage} to PC");
                }
            }
        }
        public virtual void ReceiveDamage(int damage, Player player)     
        {
            Health = Health - damage;
            if (IsDead)
            {
                Board.DespawnObject(this);
                player.Actions.Add($"{this.MapSymbol} has died");
                player.Gold += DropGoldAmount;
            }
        }
        public virtual JObject Serialize()
        {
            return JObject.FromObject(new
            {
                ObjectType,
                Race,
                Health,
                X,
                Y,
                IsHostile
            });
        }
        public virtual void Deserialize(JObject deserialized)
        {
            //string[] temp2 = serial.Split('~');
            Health = (int)deserialized[nameof(Health)];
            X = (int)deserialized[nameof(X)];
            Y = (int)deserialized[nameof(Y)];
                
        }
        public static Monster Deserialize(string serial, GameBoard board, GameItem item = null)
        {
            JObject? deserialized = (JObject?)JsonConvert.DeserializeObject(serial);
            MonsterRace race = (MonsterRace)Enum.Parse(typeof(MonsterRace), (string)deserialized[nameof(Race)]);
            Monster monster;
            
            /*string[] temp3 = serial.Split('~');
            MonsterRace race = (MonsterRace)Enum.Parse(typeof(MonsterRace), temp3[1]);*/
            if (race == MonsterRace.Phoenix)
                monster = new Phoenix(board);
            else if (race == MonsterRace.Dragon)
                monster = new Dragon(board, item);//we dont have dragonhaord info
            else if (race == MonsterRace.Merchant)
                monster = new Merchant(board);
            else
                monster = new Monster(board,race);

            monster.Deserialize(deserialized);
            return monster;
        }
    }
}

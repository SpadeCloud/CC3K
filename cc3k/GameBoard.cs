using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cc3k.Menus;
using cc3k.Items;
using cc3k.Entities;
using cc3k.Entities.Monsters;

namespace cc3k
{
    public class GameBoard
    {
        public static readonly RectangleBounds[] Rooms = new RectangleBounds[]
        {
            new RectangleBounds(2,2,27,5),
            new RectangleBounds(2,14,22,8),
            new RectangleBounds(36,15,41,7),
            new RectangleBounds(37,9,14,4),
            
            new RectangleBounds(38,2,39,5),
            new RectangleBounds(60,7,16,6)
        };

        public static readonly string[] Directions = new string[] { "no", "ea", "so", "we", "ne", "nw", "se", "sw" };
        public static void GetDirectionOffset(string direction, out int x, out int y)
        {
            if (direction == "no") { x = 0; y = -1; }
            else if (direction == "ea") { x = 1; y = 0; }
            else if (direction == "so") { x = 0; y = 1; }
            else if (direction == "we") { x = -1; y = 0; }
            else if (direction == "ne") { x = 1; y = -1; }
            else if (direction == "nw") { x = -1; y = -1; }
            else if (direction == "se") { x = 1;y = 1; }
            else /*if (direction == "sw")*/ { x = -1; y = 1; }
        }
        public static RectangleBounds GetDifferentRandomRoom(int currentX, int currentY)
        {
            while (true)
            {
                RectangleBounds newroom = GameBoard.GetRandomRoom();
                if (!newroom.Contains(currentX, currentY))
                    return newroom;
            }
        }
        public static RectangleBounds GetRandomRoom()
        {
            int r = Program.RandomGenerator.Next(5); // 0 <= r <= 4
            int r2 = 0;
            if (r == 4) r2 = Program.RandomGenerator.Next(2); // 0 <= r2 <= 1

            return Rooms[r + r2];
        }

        public readonly char[][] _board;
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
        //public Player Player1 { get; set; }
        //public List<Monster> MonsterList { get; private set; }

        private List<IMapObject> _objects;
        public IMapObject[] Objects
        {
            get
            {
                return _objects.ToArray();
            }
        }
        public IEntity[] Entities
        {
            get
            {
                var list = new List<IEntity>();
                foreach (var obj in _objects)
                {
                    if (obj.ObjectType == MapObjectType.Player || obj.ObjectType == MapObjectType.Monster)
                    {
                        list.Add((IEntity)obj);
                    }
                }
                return list.ToArray();
            }
        }

        public GameMenu? ActiveMenu {get; set; } //inially null,
                                                 //used to open different menu than gameboardmenu

        public GameBoard(string templatePath)
        {
            //MonsterList = new List<Monster>();

            _objects = new List<IMapObject>();

            string[] floorTemplate = File.ReadAllLines(templatePath);

            MaxY = floorTemplate.Length;
            MaxX = floorTemplate[0].Length;

            _board = new char[MaxY][];
            for (int y = 0; y < MaxY; y++)
            {
                _board[y] = new char[MaxX];
                for (int x = 0; x < MaxX; x++)
                {
                    _board[y][x] = floorTemplate[y][x];
                }
            }

        }
        public void Print()
        {
            for (int y = 0; y < MaxY; y++)
            {
                for (int x = 0; x < MaxX; x++)
                {
                    var entity = _objects.FirstOrDefault(o => o.Y == y && o.X == x);
                    //where o/object y,x == looping y,x 
                    if (entity == null)
                    {
                        Console.Write(_board[y][x]);
                    }
                    else
                    {
                        var existing = Console.ForegroundColor;
                        Console.ForegroundColor = entity.ObjectColor;
                        Console.Write(entity.MapSymbol);
                        Console.ForegroundColor = existing;
                    }
                }
                Console.WriteLine();
            }
        }
        public void Play(Player player)
        {
            //gameboard play creates then calls gameboardmenu (where the playing occurs?)
            GameBoardMenu game = new GameBoardMenu(this);
            while (!player.IsDead && game.Active == true)
            {
                if (ActiveMenu != null) 
                {
                    ActiveMenu.Display();
                    ActiveMenu = null;
                }
                else //only one render at a time
                {
                    game.Display(false);
                }
            }
        }

        public void SpawnObject(IMapObject obj)
        {
            _objects.Add(obj);
            obj.Spawn();
        }

        public void DespawnObject(IMapObject obj)
        {
            _objects.Remove(obj);
        }
        public bool IsMovable(int y, int x, char[] legal)
        {
            MoveError dummy;
            return IsMovable(y, x, legal, out dummy);
        }
        public bool IsMovable(int y, int x, char[] legal, out MoveError error)
        {
            error = MoveError.None;

            if ((y > - 1 && y < MaxY) && (x > -1 && x < MaxX))
            {
                if (legal.Contains(_board[y][x])) 
                {
                    IMapObject search = GetObjectAt(y, x);
                    if (search == null)
                    {
                        return true;
                    }
                    else if (search.ObjectType == MapObjectType.Item)
                    {
                        return true;
                    }
                    else if (search.ObjectType == MapObjectType.Monster)
                    {
                        error = MoveError.MonsterOccupied;
                    }
                }
                else 
                {
                    if (_board[y][x] == '-' || _board[y][x] == '|')
                        error = MoveError.Wall;
                    else if (_board[y][x] == ' ')
                        error = MoveError.InvalidPath;
                }

            }
            return false; 
        }
        public IMapObject GetObjectAt(int y, int x)
        {
            foreach (var obj in _objects)
            {
                if (obj.Y == y && obj.X == x)
                {
                    return obj;
                }
            }

            return null;

            //return _objects.FirstOrDefault(m => m.Y == y && m.X == x); ;
        }
        public bool IsCorridor(int y, int x) //is there another way to do this
        {
            if((_board[y][x] == '+') || (_board[y][x] == '#'))
            {
                return true;
            }
            return false;
        }
        public void GenerateUnoccupied(RectangleBounds bounds, out int y, out int x)
        {
            while(true)
            {
                y= Program.RandomGenerator.Next(bounds.Height);
                y += bounds.Y; 
                x= Program.RandomGenerator.Next(bounds.Width);
                x += bounds.X;
                if (_board[y][x] == '.')
                { 
                    IMapObject obj = GetObjectAt(y, x);
                    if (obj == null) // unoccupied
                        break;

                    /*var entity = Entities.FirstOrDefault(e => e.Y == y && e.X == x);
                    if (entity == null) // this means it's unoccupied
                    {
                        break;
                    }*/
                }
            }
        }
        public IMapObject[] GetObjectsNearby(IMapObject player)
        {
            List<IMapObject> list = new List<IMapObject>(); // list of objects near by
            int tempY, tempX;

            IMapObject search = null;
            for(int i = 0; i < Directions.Length; i++)
            {
                string offset = Directions[i];
                GameBoard.GetDirectionOffset(offset, out tempX, out tempY);
                search = GetObjectAt(tempY+player.Y, tempX+player.X);
                if (search != null) list.Add(search);
            }
            return list.ToArray();
        }
        public Player GetPlayer()
        {
            return this.Objects
                .Where(obj => obj.ObjectType == MapObjectType.Player)
                .Cast<Player>()
                .First();
        }
        public void Tick()
        {
            foreach (IMapObject t in this.Objects)
            {
                if (t.ObjectType == MapObjectType.Monster)
                {
                    Monster monster = (Monster)t;
                    monster.Move();
                    monster.Attack();
                }
            }
        }
        public void SpawnAll(Player player, bool changingFloor)
        {
            _objects.Clear(); //empty

            // player
            if (!changingFloor)
                SpawnObject(player); //place on board and add to _obj
            else
            {
                _objects.Add(player); //only add to _obj
            }

            // staircase
            Staircase staircase = new Staircase(this);
            SpawnObject(staircase);

            // items
            for (int i = 0; i < 10; i++)
            {
                GameItemType potionType = GameItem.GeneratePotionType();
                Potion potion = new Potion(this, potionType);
                SpawnObject(potion);
            }

            // gold
            for (int i = 0; i < 10; i++)
            {
                GameItemType goldType = GameItem.GenerateGoldType();
                Gold gold = new Gold(this, goldType);
                SpawnObject(new Gold(this, goldType));
            }

            // monsters
            for (int i = 0; i < 20; i++)
            {
                MonsterRace monsterRace = Monster.GenerateMonsterRace();
                //Monster monster = new Monster(this, monsterRace);
                Monster monster = Monster.Create(this, monsterRace);
                SpawnObject(monster);
            }
        }
        public IMapObject DeserializeMapObject(string serial)
        {
            //deserializeinventory(serial) will not work
            string type = serial.Split('~')[0];
            IMapObject obj;
            MapObjectType mapObjectType = (MapObjectType)Enum.Parse(typeof(MapObjectType), type);
            if (mapObjectType == MapObjectType.Player)
                obj = Player.Deserialize(serial, this);
            else if (mapObjectType == MapObjectType.Item)
                obj = GameItem.Deserialize(serial, this);
            else if (mapObjectType == MapObjectType.Monster)
                obj = Monster.Deserialize(serial, this);
            else
                throw new ArgumentException("unknown MapObjectType within serialized data", nameof(serial));

            return obj;
        }
    }
}

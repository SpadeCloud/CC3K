using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace cc3k
{
    public enum MapObjectType
    {
        Player,
        Monster,
        Item,
        Inventory
    }

    public interface IMapObject
    {
        int X { get; }
        int Y { get; }
        GameBoard Board { get; }
        char MapSymbol { get; }
        MapObjectType ObjectType { get; }
        ConsoleColor ObjectColor { get; }
        void Spawn();
        JObject Serialize(); 
        void Deserialize(JObject serial);
        bool IsSerializable { get; }
    }
}

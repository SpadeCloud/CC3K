using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc3k
{
    public interface IEntity
    {
        int Health { get; }
        int BaseAttack { get; }
        int Defense { get; }
        int X { get; }
        int Y { get; }
        GameBoard Board { get; }
        char MapSymbol { get; }
    }
}

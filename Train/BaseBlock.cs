using SettlersEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    enum TypeOfBlock
    {
        Path,
        Train,
        Wall,
        Gift,
        Vagon,
        Destroyed_Train,
        Unknown,
    }

    enum Direction
    { 
        Up,
        Down,
        Left,
        Right,
        Stay,
    }

    class BaseBlock :IPathNode<Object>
    {
        public BaseBlock()
        {
        }

        public TypeOfBlock type { get; set; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public bool IsWalkable(Object unused)
        {
            return (type == TypeOfBlock.Path || type == TypeOfBlock.Gift);
        }
    }
}

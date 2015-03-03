using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    class Path
    {
        private Map map;
        private LinkedList<BaseBlock> path;
        private TrainBlock train;
        public int priority { get; set; }

        public Path(Map map, LinkedList<BaseBlock> path, TrainBlock train)
        {
            this.map = map;
            this.path = path;
            this.train = train;
        }

        public int getDistance()
        {
            return path.Count;
        }

        public GiftBlock getFinalGift()
        {
            return (GiftBlock)path.Last.Value;
        }

        public BaseBlock getNext()
        {
            return path.ElementAt(1);
        }

        public Direction getDir()
        {
            if (path == null || path.Count < 2)
                return Direction.Stay;

            var second = path.ElementAt(1);

            Debug.WriteLine("Z: x: " + train.X + " y: " + train.Y);
            Debug.WriteLine("Do: x: " + second.X + " y: " + second.Y);

            if (second.Y < train.Y)
            {
                Debug.WriteLine("Nahoru");
                return Direction.Up; // nahoru
            }
            else if (second.X > train.X)
            {
                Debug.WriteLine("Doprava");
                return Direction.Right; //doprava
            }
            else if (second.Y > train.Y)
            {
                Debug.WriteLine("Dolu");
                return Direction.Down; // dolu
            }
            else if (second.X < train.X)
            {
                Debug.WriteLine("Doleva");
                return Direction.Left; // doleva
            }
            else
            {
                //Debug.WriteLine("Nevim kam mam jit");
                return Direction.Stay;
            }
        }

    }
}

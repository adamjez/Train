using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    class GiftBlock : BaseBlock
    {
        public int distance { get; set; }
        public int wallsAround { get; set; }
        public int closestEnemey { get; set; }
        public int giftsAround { get; set; }
        public int moveNext { get; set; }

        public GiftBlock()
        {
            distance = -1;
            wallsAround = 0;
            type = TypeOfBlock.Gift;
        }
    }
}

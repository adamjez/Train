using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    class TrainBlock : BaseBlock
    {
        private uint color;

        public TrainBlock(uint clr)
        {
            color = clr;
            type = TypeOfBlock.Train;
        }
    }
}

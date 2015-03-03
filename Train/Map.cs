using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    class Map
    {
        private BaseBlock[,] rawData = new BaseBlock[20,12];
        public bool Ended { get; set; }
        public bool Loaded { get; set; }
        public TrainBlock BestTrainEver { get; set; }
        public List<TrainBlock> OtherTrains { get; set; }
        public List<GiftBlock> giftBlocks = new List<GiftBlock>();

        public Map()
        {
            Ended = false;
        }

        public BaseBlock[,] getRaw()
        {
            return rawData;
        }

        public void LoadData(byte[] baseData)
        {
            var i = baseData.GetEnumerator();

            int counter = 0;
            BaseBlock pBlock;
            OtherTrains = new List<TrainBlock>();

            byte data;
            while (i.MoveNext() && counter + 3 < baseData.Count())
            {
                data = (byte)i.Current;

                var num = (uint)data;

                if (num == 255)
                {
                    Ended = true;
                    return;
                }
                else if (num == 0)
                {
                    break; 
                }
                else if (num == 1)
                {
                    pBlock = new BaseBlock();
                    pBlock.type = TypeOfBlock.Wall;
                }
                else if (num == 2)
                {
                    pBlock = new GiftBlock();
                    giftBlocks.Add((GiftBlock)pBlock);
                }
                else if (num % 10 == 0)
                {
                    pBlock = new TrainBlock(num % 10);
                    if (num / 10 == 4)
                        BestTrainEver = (TrainBlock)pBlock;
                    else
                        OtherTrains.Add((TrainBlock)pBlock);

                }
                else if (num % 10 == 1)
                {
                    pBlock = new BaseBlock();
                    pBlock.type = TypeOfBlock.Destroyed_Train;
                }
                else if (num % 10 == 5)
                {
                    pBlock = new BaseBlock();
                    pBlock.type = TypeOfBlock.Vagon;
                }
                else
                {
                    pBlock = new BaseBlock();
                    pBlock.type = TypeOfBlock.Unknown;
                    Debug.WriteLine("Error: Unknown type of block: " + num);
                }

                if (!i.MoveNext())
                    throw (new FormatException("Error: ziskal jsem malo dat"));
                pBlock.X = (byte)i.Current;

                if (!i.MoveNext())
                    throw (new FormatException("Error: ziskal jsem malo dat"));
                pBlock.Y = (byte)i.Current;

                rawData[pBlock.X, pBlock.Y] = (BaseBlock)pBlock;

                counter += 3;
            }

            Debug.WriteLine("Nacetl jsem: " + counter + " bloku");

            // Doplnim prazdne bloky - cestu
            fillPaths();
        }

        private void fillPaths()
        {
            var width = rawData.GetLength(0);
            var height = rawData.GetLength(1);
       

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (rawData[x, y] == null)
                        rawData[x, y] = new PathBlock() { X = x, Y = y };
                }
            }
        }
    }
}

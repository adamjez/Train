using SettlersEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;


namespace Train
{
    class Paths
    {
        private Map currentMap;
        private List<Path> paths;
        private TrainBlock train;

        public Paths(Map map, TrainBlock train)
        {
            currentMap = map;
            this.train = train;
        }

        public void createPaths()
        {
            paths = new List<Path>();
            SpatialAStar<BaseBlock, Object> aStar = new SpatialAStar<BaseBlock, Object>(currentMap.getRaw());
            foreach (var gift in currentMap.giftBlocks)
            {
                var from = new Point(currentMap.BestTrainEver.X, currentMap.BestTrainEver.Y);
                var to = new Point(gift.X, gift.Y);
                LinkedList<BaseBlock> path = aStar.Search(from, to, null);

                if (path == null)
                {
                    Debug.WriteLine("Nenasel jsem zadnou cestu");
                    continue;
                }

                paths.Add(new Path(currentMap, path, train));
            }
        }

        // Only for first debuging
        public Direction getDirByDistance()
        {
            var orderedPaths = paths.OrderBy(x => x.getDistance());
            if (orderedPaths.Count() == 0)
                return Direction.Stay; // STAT
            return orderedPaths.First().getDir();
        }

        public Direction getByPriority()
        {
            var orderedPaths = paths.OrderBy(x => x.priority);
            if (orderedPaths.Count() == 0)
                return Direction.Stay; // STAT

            Debug.WriteLine("Vybral s prioritou: " + orderedPaths.First().priority);
            Debug.WriteLine("Vybral s closestEnemy: " + orderedPaths.First().getFinalGift().closestEnemey);
            Debug.WriteLine("Vybral s distance: " + orderedPaths.First().getFinalGift().distance);
            Debug.WriteLine("Vybral s gifts: " + orderedPaths.First().getFinalGift().giftsAround);



            return orderedPaths.First().getDir();
        }

        public void PathsHeuristic()
        {
            distanceHeuristic();
            wallsAroundHeuristic();
            giftsAround();
            moveOnHeuristic();
            otherTrainHeuristic();
            getPriority();
        }

        private void getPriority()
        {
            foreach (var path in this.paths)
            {
                var gift = path.getFinalGift();
                if (gift.moveNext == 0)
                    path.priority = 0;
                if (currentMap.giftBlocks.Count == 0)
                {
                    continue;
                }

                if (gift.moveNext == 0)
                {
                    path.priority = int.MaxValue;
                    return;
                }

                path.priority = gift.distance
                    + (int)(gift.wallsAround * 0.2)
                    - gift.giftsAround
                    - (int)((double)gift.moveNext / currentMap.giftBlocks.Count) * 2
                    + gift.closestEnemey * 2;
            }
        }

        private void otherTrainHeuristic()
        {
            foreach (var path in this.paths)
            {
                var dist = path.getDistance();
                var gift = path.getFinalGift();
                foreach (var train in this.currentMap.OtherTrains)
                {
                    SpatialAStar<BaseBlock, Object> aStar = new SpatialAStar<BaseBlock, Object>(currentMap.getRaw());

                    var from = new Point(train.X, train.Y);
                    var to = new Point(gift.X, gift.Y);
                    LinkedList<BaseBlock> newPath = aStar.Search(from, to, null);

                    if (newPath != null && newPath.Count < dist)
                    {
                        gift.closestEnemey++;
                    }
                }
            }
        }

        private void moveOnHeuristic()
        {
            foreach (var path in this.paths)
            {
                var gift = path.getFinalGift();
                BaseBlock[,] newData = new BaseBlock[20, 12];

                if (path == null || path.getDistance() < 2)
                    continue;

                var next = path.getNext();

                // Change type
                var orig2d = this.currentMap.getRaw();
                // Copy
                var width = orig2d.GetLength(0);
                var height = orig2d.GetLength(1);


                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        newData[x, y] = orig2d[x, y];
                    }
                }
                newData[next.X, next.Y].type = TypeOfBlock.Wall;


                SpatialAStar<BaseBlock, Object> aStar = new SpatialAStar<BaseBlock, Object>(newData);


                foreach (var giftToFind in currentMap.giftBlocks)
                {
                    if (gift == giftToFind)
                        continue;

                    var from = new Point(next.X, next.Y);
                    var to = new Point(giftToFind.X, giftToFind.Y);
                    LinkedList<BaseBlock> newPath = aStar.Search(from, to, null);

                    if (newPath == null)
                    {
                        //Debug.WriteLine("Nenasel jsem zadnou cestu");
                        continue;
                    }

                    gift.moveNext++;
                }

            }
        }

        private void distanceHeuristic()
        {
            foreach (var path in this.paths)
            {
                var gift = path.getFinalGift();
                gift.distance = path.getDistance();
            }
        }

        private void wallsAroundHeuristic()
        {
            foreach (var gift in currentMap.giftBlocks)
            {
                var data = currentMap.getRaw();

                if (gift.X - 1 >= 0)
                {
                    if (!data[gift.X - 1, gift.Y].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.X + 1 < 20)
                {
                    if (!data[gift.X + 1, gift.Y].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.Y - 1 >= 0)
                {
                    if (!data[gift.X, gift.Y - 1].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.Y + 1 < 12)
                {
                    if (!data[gift.X, gift.Y + 1].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.Y - 1 >= 0 && gift.X - 1 >= 0)
                {
                    if (!data[gift.X - 1, gift.Y - 1].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.Y - 1 >= 0 && gift.X + 1 < 20)
                {
                    if (!data[gift.X + 1, gift.Y - 1].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.Y + 1 < 12 && gift.X - 1 >= 0)
                {
                    if (!data[gift.X - 1, gift.Y + 1].IsWalkable(this))
                        gift.wallsAround++;
                }

                if (gift.Y + 1 < 12 && gift.X + 1 >= 0)
                {
                    if (!data[gift.X + 1, gift.Y + 1].IsWalkable(this))
                        gift.wallsAround++;
                }

            }

        }

        private void giftsAround()
        {
            foreach (var gift in currentMap.giftBlocks)
            {
                var data = currentMap.getRaw();

                if (gift.X - 1 >= 0)
                {
                    if (data[gift.X - 1, gift.Y].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.X + 1 < 20)
                {
                    if (data[gift.X + 1, gift.Y].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.Y - 1 >= 0)
                {
                    if (data[gift.X, gift.Y - 1].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.Y + 1 < 12)
                {
                    if (data[gift.X, gift.Y + 1].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.Y - 1 >= 0 && gift.X - 1 >= 0)
                {
                    if (data[gift.X - 1, gift.Y - 1].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.Y - 1 >= 0 && gift.X + 1 < 20)
                {
                    if (data[gift.X + 1, gift.Y - 1].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.Y + 1 < 12 && gift.X - 1 >= 0)
                {
                    if (data[gift.X - 1, gift.Y + 1].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

                if (gift.Y + 1 < 12 && gift.X + 1 >= 0)
                {
                    if (data[gift.X + 1, gift.Y + 1].type == TypeOfBlock.Gift)
                        gift.giftsAround++;
                }

            }

        }

    }
}

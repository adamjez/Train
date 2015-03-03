using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train
{
    class Program
    {
        const string remoteServer = "192.168.38.161";
        const string server = "127.0.0.1";
        const int sendingPort = 1030;
        const int receivingPort = 1031;
        const int colorNum = 4;

        static void Main(string[] args)
        {
            Procces();
            Console.ReadKey();
        }

        static async void Procces()
        {
            using (Connection conn = new Connection(receivingPort, sendingPort, remoteServer))
            {
                await conn.Connect();

                while (true)
                {
                    var map = new Map();

                    map.LoadData(await conn.Receive());

                    if (map.Ended)
                    {
                        Debug.WriteLine("Game ended!");
                        return;
                    }

                    if (map.BestTrainEver == null)
                    {
                        Debug.WriteLine("Our train isnt on map :(");
                        return;
                    }
                    var logic = new ComputeLogic(map, colorNum);

                    await conn.Sending(directionMapping(logic.nextStep()));
                }
            }
        }

        static byte[] directionMapping(Direction dir)
        { 
            byte[] buffer = new byte[1];
            if (dir == Direction.Up)
                buffer[0] = 20;
            else if (dir == Direction.Down)
                buffer[0] = 40;
            else if (dir == Direction.Left)
                buffer[0] = 50;
            else if (dir == Direction.Right)
                buffer[0] = 30;
            else if (dir == Direction.Stay)
                buffer[0] = 10;
            return buffer;
        }

    }
}

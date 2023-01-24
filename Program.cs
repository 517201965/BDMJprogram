using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BDMJprogram
{
    class Program
    {
        static void Main(string[] args)
        {
            //NamePipe pipe = new NamePipe();
            //pipe.ConnectToServer();

            Game game = new Game();
            if(game.CreateServer())
            {
                game.Initial(ZUOWEI.Nan);
                game.FapaiBaida();
                game.StartLoop();
            }

            Console.ReadKey();
        }

    }


}

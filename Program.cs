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
            Game game = new Game();
            game.Initial();
            game.FapaiBaida();
            Console.ReadKey();
        }

    }
}

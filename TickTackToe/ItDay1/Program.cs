using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItDay1
{
    class Program
    {
        static void Main()
        {
            Board b = new Board();

            int whoseTurnIsIt = 1;
            while(b.IsGameOver() == 0)
            {
                Console.Clear();
                b.Display();
                TryPlay(b, whoseTurnIsIt);
                whoseTurnIsIt = whoseTurnIsIt * -1;
            }
            Console.Clear();
            b.Display();
            if(b.IsGameOver() == -1)
            {
                Console.WriteLine("O's Won!");
            }
            else
            {

                Console.WriteLine("X's Won!");
            }
            Console.ReadLine();
        }
        
        static void TryPlay(Board b, int player)
        {
            Console.WriteLine("Enter your turn (y, x): ");
            int x = PromptForANumber();
            int y = PromptForANumber();

            if(x < 0 || y < 0 || x >= 3 || y >= 3 || !b.cells[x][y].IsOpen)
            {
                Console.WriteLine("You done fucked up");
                TryPlay(b, player);
            }
            else
            {
                b.cells[x][y].IsOpen = false;
                b.cells[x][y].PlayerNumber = player;
            }
        }

        static int PromptForANumber()
        {
            while(true)
            {
                var input = Console.ReadLine();
                if(int.TryParse(input, out int result))
                {
                    return result;
                }
            }
        }
    }
}

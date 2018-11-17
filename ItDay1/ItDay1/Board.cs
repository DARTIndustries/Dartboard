using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItDay1
{
    class Board
    {
        public Cell[][] cells;

        public Board()
        {
            cells = new Cell[3][];
            for (int row = 0; row < 3; row++)
            {
                cells[row] = new Cell[3];
                for(int col = 0; col < 3; col++)
                {
                    cells[row][col] = new Cell();
                }
            }
        }

        public void Display()
        {
            for(int row = 0; row < cells.Length; row++)
            {
                for(int col = 0; col < cells[row].Length; col++)
                {
                    if(cells[row][col].IsOpen)
                    {
                        Console.Write(".");
                    }
                    else if(cells[row][col].PlayerNumber > 0)
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write("O");
                    }
                }
                Console.WriteLine();
            }
        }

        public int IsGameOver()
        {

            int sum = 0;
            for (int row = 0; row < 3; row++)
            {
                sum = 0;
                for (int col = 0; col < 3; col++)
                    sum += cells[row][col].PlayerNumber;
                if (sum == 3 || sum == -3)
                    return Math.Sign(sum);
            }

            for (int col = 0; col < 3; col++)
            {
                sum = 0;
                for (int row = 0; row < 3; row++)
                    sum += cells[row][col].PlayerNumber;
                if (sum == 3 || sum == -3)
                    return Math.Sign(sum);
            }

            sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += cells[i][i].PlayerNumber;
                if (sum == 3 || sum == -3)
                    return Math.Sign(sum);
            }

            sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += cells[i][2-i].PlayerNumber;
                if (sum == 3 || sum == -3)
                    return Math.Sign(sum);
            }

            return 0;
        }
    }
}

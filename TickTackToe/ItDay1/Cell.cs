using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItDay1
{
    class Cell
    {
        // True or false, true if nothing is here
        public bool IsOpen;

        // Playernumber is 1 for X's and -1 for O's
        public int PlayerNumber;

        public Cell()
        {
            IsOpen = true;
            PlayerNumber = 0;
        }
    }
}

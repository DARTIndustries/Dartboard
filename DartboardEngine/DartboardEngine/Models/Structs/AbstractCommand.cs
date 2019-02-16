using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngine.Models.Structs
{
    public struct AbstractCommand
    {
        public readonly ECommandType CommandType;
        public AbstractCommand(ECommandType commandType)
        {
            CommandType = commandType;
        }
    }
}

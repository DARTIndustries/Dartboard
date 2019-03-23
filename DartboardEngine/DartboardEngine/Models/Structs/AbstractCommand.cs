using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngine.Models.Structs
{
    public interface IRobotCommand
    {
        ECommandType GetCommandType();
    }

    public static class Command
    {
        public static readonly Dictionary<ECommandType, Type> CommandTypes;
        static Command()
        {
            CommandTypes = new Dictionary<ECommandType, Type>();
        }
    }
}

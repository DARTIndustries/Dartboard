using DartboardEngine.Models.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardUi.ConnectionInterfaces
{
    interface IHandleData: IHandler
    {
        void HandleCommand(IRobotCommand command);
    }
}

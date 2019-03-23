using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DartboardUi.ConnectionInterfaces
{
    interface IHandleDisconnect: IHandler
    {
        void HandleDisconnect(Exception ex);
    }
}

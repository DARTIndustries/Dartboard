using DartboardEngine.Models;
using DartboardEngine.Models.Structs;
using DartboardEngine.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DartboardEngine.Network
{
    public class PacketParser
    {
        private readonly AccessQueue<byte> _pendingParse;
            
        public PacketParser()
        {
            _pendingParse = new AccessQueue<byte>();
        }

        public IEnumerable<ICommand> Parse(byte[] newData)
        {
            foreach (byte b in newData)
                _pendingParse.Enqueue(b);

            while (_pendingParse.Count >= 4)
            {
                int commandId = BitConverter.ToInt32(_pendingParse.Take(4).ToArray(), 0);
                ECommandType cmdType = (ECommandType)commandId;
                if(!Enum.IsDefined(typeof(ECommandType), cmdType))
                {
                    throw new UnknownCommandException(commandId);
                }

                Type parseType = Command.CommandTypes[cmdType];
                int fullSize = Marshal.SizeOf(parseType);

                if(_pendingParse.Count >= fullSize)
                {
                    var packetData = new byte[fullSize];
                    for(int i = 0; i < packetData.Length; i++)
                    {
                        packetData[i] = _pendingParse.Dequeue();
                    }
                    yield return (ICommand)StructMarshaller.Decode(parseType, packetData);
                }
            }
        }

        public void ClearBuffer()
        {
            _pendingParse.Clear();
        }
    }

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }

    public class UnknownCommandException : ParseException
    {
        public int CommandId { get; }

        public UnknownCommandException(int commandId) : base($"Failed to parse. Unknown command ID: {commandId}")
        {
            CommandId = commandId;
        }
    }

    public class UnknownSizeException : ParseException
    {
        public string TypeName { get; }

        public UnknownSizeException(string typeName) : base($"Failed to parse. Command of type {typeName} has no StructLayoutAttribute")
        {
            TypeName = typeName;
        }
    }
}

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
    /// <summary>
    /// Used to parse packets out of network activity
    /// </summary>
    public class PacketParser
    {
        // Stoes bytes recieved that have not yet been parsed (for incomplete messages)
        private readonly AccessQueue<byte> _pendingParse;
            
        public PacketParser()
        {
            _pendingParse = new AccessQueue<byte>();
        }

        /// <summary>
        /// Tries to parse data, returning a list of commands recieved
        /// </summary>
        public IEnumerable<IRobotCommand> Parse(byte[] newData)
        {
            // Push data to the pending queue
            foreach (byte b in newData)
                _pendingParse.Enqueue(b);

            // We need at least 4 bits for the cmd type header
            while (_pendingParse.Count >= 4)
            {
                // Parse the header
                int commandId = BitConverter.ToInt32(_pendingParse.Take(4).ToArray(), 0);
                ECommandType cmdType = (ECommandType)commandId;
                if(!Enum.IsDefined(typeof(ECommandType), cmdType))
                {
                    // If we don't recognize it, die.
                    throw new UnknownCommandException(commandId);
                }

                // Get the command type
                Type parseType = Command.CommandTypes[cmdType];
                // find its size
                int fullSize = Marshal.SizeOf(parseType);

                // If we have that many bytes, we can parse it
                if(_pendingParse.Count >= fullSize)
                {
                    // Pull off enough packets
                    var packetData = new byte[fullSize];
                    for(int i = 0; i < packetData.Length; i++)
                    {
                        packetData[i] = _pendingParse.Dequeue();
                    }
                    // and return it as a command
                    yield return (IRobotCommand)StructMarshaller.Decode(parseType, packetData);
                }
            }
        }

        // Clears any pending bytes
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

using System;
using DartboardEngine.Models;
using System.Runtime.InteropServices;


namespace DartboardEngine.Models.Structs
{

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 7)]
    public struct LEDCommand
    {
        public LEDCommand(ECommandType CommandType, byte R, byte G, byte B)
        {
            this.CommandType = CommandType;
            this.R = R;
            this.G = G;
            this.B = B;
        }
        [FieldOffset(0)]
        public readonly ECommandType CommandType;
        [FieldOffset(4)]
        public readonly byte R;
        [FieldOffset(5)]
        public readonly byte G;
        [FieldOffset(6)]
        public readonly byte B;
    }




    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 14)]
    public struct ServoCommand
    {
        public ServoCommand(ECommandType CommandType, double Position, ushort ServoId)
        {
            this.CommandType = CommandType;
            this.Position = Position;
            this.ServoId = ServoId;
        }
        [FieldOffset(0)]
        public readonly ECommandType CommandType;
        [FieldOffset(4)]
        public readonly double Position;
        [FieldOffset(12)]
        public readonly ushort ServoId;
    }




    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
    public struct StatusMessage
    {
        public StatusMessage(ECommandType CommandType, double CpuPercent)
        {
            this.CommandType = CommandType;
            this.CpuPercent = CpuPercent;
        }
        [FieldOffset(0)]
        public readonly ECommandType CommandType;
        [FieldOffset(4)]
        public readonly double CpuPercent;
    }
}

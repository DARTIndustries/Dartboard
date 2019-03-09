using DartboardEngine.Models.Structs;
using DartboardEngine.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngineUnitTests
{
    [TestClass]
    public class PacketParserTest
    {
        [TestMethod]
        public void BasicParsing()
        {
            PacketParser parser = new PacketParser();
            LEDCommand testCommand = new LEDCommand(DartboardEngine.Models.ECommandType.LED_COMMAND, 29, 33, 15);
            byte[] encoded = StructMarshaller.Encode(testCommand);

            var result = parser.Parse(encoded).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(testCommand, result[0]);
        }

        [TestMethod]
        public void MultiParsing()
        {
            PacketParser parser = new PacketParser();
            LEDCommand ca = new LEDCommand(DartboardEngine.Models.ECommandType.LED_COMMAND, 29, 33, 15);
            LEDCommand cb = new LEDCommand(DartboardEngine.Models.ECommandType.LED_COMMAND, 13, 44, 67);
            byte[] ea = StructMarshaller.Encode(ca);
            byte[] eb = StructMarshaller.Encode(cb);

            byte[] total = ea.Concat(eb).ToArray();

            var result = parser.Parse(total).ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(ca, result[0]);
            Assert.AreEqual(cb, result[1]);
        }


        [TestMethod]
        public void PartialParsing()
        {
            PacketParser parser = new PacketParser();
            LEDCommand ca = new LEDCommand(DartboardEngine.Models.ECommandType.LED_COMMAND, 29, 33, 15);
            byte[] ea = StructMarshaller.Encode(ca);
            int splitPoint = 3;

            byte[] beforeSplit = new byte[splitPoint];
            byte[] afterSplit = new byte[ea.Length - splitPoint];

            Array.ConstrainedCopy(ea, 0, beforeSplit, 0, splitPoint);
            Array.ConstrainedCopy(ea, splitPoint, afterSplit, 0, afterSplit.Length);

            var result = parser.Parse(beforeSplit).ToArray();
            Assert.AreEqual(0, result.Length);
            result = parser.Parse(afterSplit).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(ca, result[0]);
        }
    }
}

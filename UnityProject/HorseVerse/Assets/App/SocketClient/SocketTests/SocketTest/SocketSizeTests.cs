using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using io.hverse.game.protogen;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SocketSizeTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void SocketSizeTestsSimplePasses()
    {
        byte[] data;
        GameMessage ms = new GameMessage();
        ms.MsgType = GameMessageType.PingMessage;
        ms.WriteTo(CreateOutputStream(ms.CalculateSize(), out data));
            
        CodedOutputStream CreateOutputStream(int messageSerializeSize, out byte[] sendData)
        { 
            int varintSize = CodedOutputStream.ComputeRawVarint32Size((uint)(messageSerializeSize));
            int ackSize = varintSize + messageSerializeSize;
            sendData = new byte[ackSize];
            CodedOutputStream co = new CodedOutputStream(sendData);
            co.WriteUInt32((uint)(messageSerializeSize));
            return co;
        }

        var size = 0U;
        var byteArray = data.TakeWhile(x => (x & 0x80) != 0)
            .Append(data.FirstOrDefault(x => (x & 0x80) == 0))
            .ToArray();

        CodedInputStream codedInputStream = new CodedInputStream(byteArray);
        size = codedInputStream.ReadUInt32();
        
        // var byteArray2 = BitConverter.GetBytes(1002);
        // size = BitConverter.ToUInt32(ToInt32ByteArray(byteArray), 0) - (uint)byteArray.Length;
        Assert.True(size == 2);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SocketSizeTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}

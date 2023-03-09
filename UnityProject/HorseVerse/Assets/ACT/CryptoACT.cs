using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class CryptoACT
{
    private readonly byte[] key = new byte[16] { 25, 197, 154, 211, 240, 223, 182, 197, 205, 16, 147, 190, 55, 126, 152, 47 }; // The secret key to use for the symmetric algorithm.
    private readonly byte[] iv = new byte[16] { 101, 56, 27, 187, 183, 147, 214, 82, 84, 133, 193, 167, 118, 147, 253, 18 }; // The initialization vector to use for the symmetric algorithm.
    private readonly Aes aes;
    private ICryptoTransform encryptor;

    private static CryptoACT instance;
    private ICryptoTransform decryptor;
    private static CryptoACT Instance => instance ??= new CryptoACT();
    
    private CryptoACT()
    {
         aes = Aes.Create();
         
    }
    
    public static byte[] Encode(byte[] bytes)
    {
        return Instance.EncodeInternal(bytes);
    }
    
    private byte[] EncodeInternal(byte[] bytes)
    {
        encryptor = aes.CreateEncryptor(key, iv);
        var memoryStream = new MemoryStream();

        var encodeStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        encodeStream.Write(bytes, 0, bytes.Length);
        encodeStream.Close();
        return memoryStream.ToArray();
    }

    public static byte[] Decode(byte[] bytes)
    {
        return Instance.DecodeInternal(bytes);
    }

    private byte[] DecodeInternal(byte[] bytes)
    {
        decryptor = aes.CreateDecryptor(key, iv);
        var memoryStream = new MemoryStream();

var        decodeStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

        decodeStream.Write(bytes, 0, bytes.Length);
        decodeStream.Close();
        return memoryStream.ToArray();
    }
}

public class CryptoField<T>  where T : IConvertible
{
    private byte[] bytes;
    
    public T Value
    {
        get => ((T)Convert.ChangeType(System.Text.Encoding.UTF8.GetString(CryptoACT.Decode(bytes)), typeof(T)));

        set =>  bytes = CryptoACT.Encode(System.Text.Encoding.UTF8.GetBytes(value.ToString()));
    }
}

public class ScrambleField<T>  where T : IConvertible
{
    private string scrambleField;
    private readonly int key = 123123123;

    public T Value
    {
        get => ((T)Convert.ChangeType(scrambleField.DeShuffle(key), typeof(T)));

        set => scrambleField = value.ToString().Shuffle(key);
    }
}

public static class ShuffleExtensions
{
    private static int[] GetShuffleExchanges(int size, int key)
    {
        var exchanges = new int[size - 1];
        var rand = new Random((uint)key);
        for (var i = size - 1; i > 0; i--)
        {
            var n = rand.NextInt(i + 1);
            exchanges[size - 1 - i] = n;
        }
        return exchanges;
    }

    public static string Shuffle(this string toShuffle, int key)
    {
        var size = toShuffle.Length;
        var chars = toShuffle.ToArray();
        var exchanges = GetShuffleExchanges(size, key);
        for (var i = size - 1; i > 0; i--)
        {
            var n = exchanges[size - 1 - i];
            (chars[i], chars[n]) = (chars[n], chars[i]);
        }
        return new string(chars);
    }

    public static string DeShuffle(this string shuffled, int key)
    {
        var size = shuffled.Length;
        var chars = shuffled.ToArray();
        var exchanges = GetShuffleExchanges(size, key);
        for (var i = 1; i < size; i++)
        {
            var n = exchanges[size - i - 1];
            (chars[i], chars[n]) = (chars[n], chars[i]);
        }
        return new string(chars);
    }
}
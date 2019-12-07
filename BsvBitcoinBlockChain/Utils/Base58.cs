﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Math;

namespace BsvBitcoinBlockChain.Utils
{
    public static class Base58
    {
        private static readonly BigInteger _base = BigInteger.ValueOf(58L);
        private const string _alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static string Encode(byte[] input)
        {
            BigInteger bigInteger = new BigInteger(1, input);
            StringBuilder stringBuilder = new StringBuilder();
            BigInteger n;
            for (; bigInteger.CompareTo(Base58._base) >= 0; bigInteger = bigInteger.Subtract(n).Divide(Base58._base))
            {
                n = bigInteger.Mod(Base58._base);
                stringBuilder.Insert(0, new char[1]
                {
                    "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"[n.IntValue]
                });
            }

            stringBuilder.Insert(0, new char[1]
            {
                "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"[bigInteger.IntValue]
            });
            byte[] numArray = input;
            for (int index = 0; index < numArray.Length && numArray[index] == (byte) 0; ++index)
                stringBuilder.Insert(0, new char[1]
                {
                    "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"[0]
                });
            return stringBuilder.ToString();
        }

        public static byte[] Decode(string input)
        {
            byte[] byteArray = Base58.DecodeToBigInteger(input).ToByteArray();
            bool flag = byteArray.Length > 1 && byteArray[0] == (byte) 0 && byteArray[1] >= (byte) 128;
            int destinationIndex = 0;
            for (int index = 0;
                (int) input[index] == (int) "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"[0];
                ++index)
                ++destinationIndex;
            byte[] numArray = new byte[byteArray.Length - (flag ? 1 : 0) + destinationIndex];
            Array.Copy((Array) byteArray, flag ? 1 : 0, (Array) numArray, destinationIndex,
                numArray.Length - destinationIndex);
            return numArray;
        }

        public static BigInteger DecodeToBigInteger(string input)
        {
            BigInteger bigInteger = BigInteger.ValueOf(0L);
            for (int index = input.Length - 1; index >= 0; --index)
            {
                int num = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".IndexOf(input[index]);
                if (num == -1)
                    throw new Exception("Illegal character " + (object) input[index] + " at " +
                                                     (object) index);
                bigInteger = bigInteger.Add(BigInteger.ValueOf((long) num)
                    .Multiply(Base58._base.Pow(input.Length - 1 - index)));
            }

            return bigInteger;
        }

        public static byte[] DecodeChecked(string input)
        {
            byte[] numArray1 = Base58.Decode(input);
            if (numArray1.Length < 4)
                throw new Exception("Input too short");
            byte[] numArray2 = new byte[4];
            Array.Copy((Array) numArray1, numArray1.Length - 4, (Array) numArray2, 0, 4);
            byte[] input1 = new byte[numArray1.Length - 4];
            Array.Copy((Array) numArray1, 0, (Array) input1, 0, numArray1.Length - 4);
            byte[] numArray3 = HashUtils.DoubleDigest(input1);
            byte[] numArray4 = new byte[4];
            Array.Copy((Array) numArray3, 0, (Array) numArray4, 0, 4);
            if (!((IEnumerable<byte>) numArray4).SequenceEqual<byte>((IEnumerable<byte>) numArray2))
                throw new Exception("Checksum does not validate");
            return input1;
        }
    }
}
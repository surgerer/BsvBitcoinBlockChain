﻿using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;

namespace BsvBitcoinBlockChain.Utils
{
    public static class HashUtils
    {
        public static byte[] Ripemd160(byte[] input)
        {
            byte[] hash = new SHA256Managed().ComputeHash(input);
            RipeMD160Digest ripeMd160Digest = new RipeMD160Digest();
            ripeMd160Digest.BlockUpdate(hash, 0, hash.Length);
            byte[] output = new byte[20];
            ripeMd160Digest.DoFinal(output, 0);
            return output;
        }
        public static byte[] DoubleDigest(byte[] input)
        {
            return DoubleDigest(input, 0, input.Length);
        }

        public static byte[] DoubleDigest(byte[] input, int offset, int length)
        {
            SHA256Managed shA256Managed = new SHA256Managed();
            byte[] hash = shA256Managed.ComputeHash(input, offset, length);
            return shA256Managed.ComputeHash(hash);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace PoseidonSharp
{
    public class Eddsa
    {
        private BigInteger OriginalHash { get; set; }
        private BigInteger PrivateKey { get; set; }

        private static BigInteger JUBJUB_E = BigInteger.Parse("21888242871839275222246405745257275088614511777268538073601725287587578984328");
        private static BigInteger JUBJUB_C = BigInteger.Parse("8");
        private static BigInteger JUBJUB_L = BigInteger.DivRem(JUBJUB_E, JUBJUB_C, out JUBJUB_L);


        public Eddsa(BigInteger _originalHash, string _privateKey)
        {
            OriginalHash = _originalHash;
            BigInteger privateKeyBigInteger = BigInteger.Parse(_privateKey.Substring(2, _privateKey.Length - 2), NumberStyles.AllowHexSpecifier);
            if(privateKeyBigInteger.Sign == -1) //hex parse in big integer can make a negative number so we need to convert as below
            {
                string privateKeyAsPositiveHexString = "0" + _privateKey.Substring(2, _privateKey.Length - 2); //add a zero to the front of the string to make it positive
                privateKeyBigInteger = BigInteger.Parse(privateKeyAsPositiveHexString, NumberStyles.AllowHexSpecifier);
            }
            PrivateKey = privateKeyBigInteger;
        }

        public string Sign(object _points = null)
        {
            (BigInteger x, BigInteger y) B;
            if (_points != null)
            {
                B = ((BigInteger x, BigInteger))_points;
            }
            else
            {
                B = Point.Generator();
            }

            (BigInteger x, BigInteger y) A = Point.Multiply(PrivateKey, B);
            BigInteger r = HashPrivateKey(PrivateKey, OriginalHash);
            (BigInteger x, BigInteger y) R = Point.Multiply(r, B);
            BigInteger t = HashPublic(R, A, OriginalHash);
            BigInteger S = (r + (PrivateKey * t)) % JUBJUB_E;
            if (S.Sign == -1)
            {
                S = S + JUBJUB_E;
            }

            Signature signature = new Signature(R, S);
            SignedMessage signedMessage = new SignedMessage(A, signature, OriginalHash);
            string rX = signedMessage.Signature.R.x.ToString("x").PadLeft(64,'0');
            string rY = signedMessage.Signature.R.y.ToString("x").PadLeft(64, '0');
            string rS = signedMessage.Signature.S.ToString("x").PadLeft(64, '0');
            string finalSignedMessage = "0x" + rX + rY + rS;
            return finalSignedMessage;
        }

        private BigInteger HashPublic((BigInteger x, BigInteger y) r, (BigInteger x, BigInteger y) a, BigInteger m)
        {
            BigInteger[] inputs = { r.x, r.y, a.x, a.y, m};
            Poseidon poseidon = new Poseidon(6, 6, 52, "poseidon", 5, _securityTarget: 128);
            return poseidon.CalculatePoseidonHash(inputs);
        }

        private BigInteger HashPrivateKey(BigInteger privateKey, BigInteger originalHash)
        {
            var secretBytes = CalculateNumberOfBytesAndReturnByteArray(privateKey);
            var originalHashBytes = CalculateNumberOfBytesAndReturnByteArray(originalHash);
            byte[] originalHashPaddedBytes = null;
            if (originalHashBytes.Length < 32) //Pad out byte array to 32 bytes as the original hash can sometimes give less than a 32 byte array
            {
                originalHashPaddedBytes = new byte[originalHashBytes.Length + 1];
                Array.Copy(originalHashBytes, originalHashPaddedBytes, originalHashBytes.Length);
            }
            else
            {
                originalHashPaddedBytes = originalHashBytes;
            }
            var combinedPrivateKeyAndPoseidonHashBytes = CombineBytes(secretBytes, originalHashPaddedBytes);
            byte[] sha512HashBytes;
            SHA512 sha512Managed = new SHA512Managed();
            sha512HashBytes = sha512Managed.ComputeHash(combinedPrivateKeyAndPoseidonHashBytes);

            BigInteger sha512HashedNumber = new BigInteger(sha512HashBytes);
            if(sha512HashedNumber.Sign == -1) //sha512 in bytes is a hex number so sometimes can return negative
            {
                string sha512HexString = "0" + sha512HashedNumber.ToString("x"); //add a zero to the front of the hex string to make it a  positive number
                sha512HashedNumber = BigInteger.Parse(sha512HexString, NumberStyles.AllowHexSpecifier);
            }

            BigInteger result = sha512HashedNumber %  JUBJUB_L;
            if (result.Sign == -1)
            {
                result = result + JUBJUB_L;
            }

            return result;
        }

        private byte[] CalculateNumberOfBytesAndReturnByteArray(BigInteger bigIntegerValue) //Don't really need to calcuate the number of bytes but is helpful for debugging
        {
            BigInteger numberOfBits = (BigInteger)Math.Ceiling(BigInteger.Log(bigIntegerValue, 2));
            numberOfBits += 8 - (numberOfBits % 8);
            BigInteger numberOfBytes = new BigInteger();
            numberOfBytes = BigInteger.DivRem(numberOfBits, 8, out numberOfBytes); //We want 32 bytes otherwise we will have to pad out the byte array
            return bigIntegerValue.ToByteArray();
        }

        public static byte[] CombineBytes(byte[] first, byte[] second)
        {
            return first.Concat(second).ToArray();
        }

    }
}
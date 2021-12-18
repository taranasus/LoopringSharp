using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace PoseidonSharp
{
    public static class SHA256Helper
    {
        private static BigInteger SNARK_SCALAR_FIELD = BigInteger.Parse("21888242871839275222246405745257275088548364400416034343698204186575808495617");

        public static BigInteger CalculateSHA256HashNumber(string requestText)
        {
            byte[] requestBytes = Encoding.UTF8.GetBytes(requestText);

            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] sha256HashBytes = sha256Managed.ComputeHash(requestBytes);
            string sha256HashString = string.Empty;
            foreach (byte x in sha256HashBytes)
            {
                sha256HashString += String.Format("{0:x2}", x);
            }

            BigInteger sha256HashNumber = BigInteger.Parse(sha256HashString, NumberStyles.AllowHexSpecifier);
            if (sha256HashNumber.Sign == -1)
            {
                string sha256HashAsPositiveHexString = "0" + sha256HashNumber.ToString("x2");
                sha256HashNumber = BigInteger.Parse(sha256HashAsPositiveHexString, NumberStyles.AllowHexSpecifier);
            }

            BigInteger result = sha256HashNumber % SNARK_SCALAR_FIELD;
            if (result.Sign == -1)
            {
                result = result + SNARK_SCALAR_FIELD;
            }
            return result;
        }
    }
}

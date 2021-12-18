using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PoseidonSharp
{
    public class SignedMessage
    {
        public (BigInteger x, BigInteger y) A = (BigInteger.Parse("0"), BigInteger.Parse("0"));
        public Signature Signature { get; set; }

        public BigInteger Message { get; set; }

        public SignedMessage((BigInteger x, BigInteger y) _a, Signature _s, BigInteger _message)
        {
            A = _a;
            Signature = _s;
            Message = _message;
        }

    }
}

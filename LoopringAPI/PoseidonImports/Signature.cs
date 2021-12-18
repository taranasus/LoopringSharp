using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace PoseidonSharp
{
    public class Signature
    {
        public (BigInteger x, BigInteger y) R = (BigInteger.Parse("0"), BigInteger.Parse("0"));
        public BigInteger S { get; set; }
        public Signature((BigInteger x, BigInteger y) _r, BigInteger _s)
        {
            R.x = _r.x;
            R.y = _r.y;
            S = _s;
        }
    }
}

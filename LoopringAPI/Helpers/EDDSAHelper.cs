using PoseidonSharp;
using System.Numerics;

namespace LoopringAPI
{
    public static class EDDSAHelper
    {
        public static string EDDSASign(BigInteger[] inputs, string loopringAddress)
        {
            var signer = new Eddsa(PoseidonHelper.GetPoseidonHash(inputs), loopringAddress);
            return signer.Sign();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3;

namespace LoopringAPI
{
    public class EIP712Helper
    {
        private Domain exchangeDomain;
        private BigInteger _chainId;

        public EIP712Helper(string name, string version, BigInteger chainId, string verifyingContract)
        {
            exchangeDomain = new Domain();
            exchangeDomain.Name = name;
            exchangeDomain.Version = version;
            exchangeDomain.ChainId = chainId;
            _chainId = chainId;
            exchangeDomain.VerifyingContract = verifyingContract;
        }

        public string GenerateTransactionXAIPSIG(ApiTransferRequest transferRequest, string ethPrivateKey)
        {
            Eip712TypedDataSigner singer = new Eip712TypedDataSigner();

            string primaryTypeName = "Transfer";

            TypedData data = new TypedData();
            data.Domain = exchangeDomain;
            data.PrimaryType = primaryTypeName;
            data.Types = new Dictionary<string, MemberDescription[]>()
            {
                ["EIP712Domain"] = new[]
                    {
                        new MemberDescription {Name = "name", Type = "string"},
                        new MemberDescription {Name = "version", Type = "string"},
                        new MemberDescription {Name = "chainId", Type = "uint256"},
                        new MemberDescription {Name = "verifyingContract", Type = "address"},
                    },
                [primaryTypeName] = new[]
                    {
                        new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                        new MemberDescription {Name = "to", Type = "address"},              // payeeAddr
                        new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                        new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                        new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                        new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                        new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                        new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                    },

            };
            data.Message = new[]
            {
                new MemberValue {TypeName = "address", Value = transferRequest.payerAddr},
                new MemberValue {TypeName = "address", Value = transferRequest.payeeAddr},
                new MemberValue {TypeName = "uint16", Value = transferRequest.token.tokenId},
                new MemberValue {TypeName = "uint96", Value = long.Parse(transferRequest.token.volume)},
                new MemberValue {TypeName = "uint16", Value = transferRequest.maxFee.tokenId},
                new MemberValue {TypeName = "uint96", Value = long.Parse(transferRequest.maxFee.volume)},
                new MemberValue {TypeName = "uint32", Value = transferRequest.validUntil},
                new MemberValue {TypeName = "uint32", Value = transferRequest.storageId},
            };

            var signerKey = new Nethereum.Signer.EthECKey(ethPrivateKey.Replace("0x", ""));
            var encrypted2 = singer.EncodeTypedData(data);
            var signature = signerKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encrypted2));
            var thing2 = EthECDSASignature.CreateStringSignature(signature);

            return thing2 + "02";
        }

        public string HahsPacked(string clsEIP191Header, string domainHahs, string dataHash)
        {
            string keccakstring = clsEIP191Header + domainHahs + dataHash;
            string afterhash = Web3.Sha3(keccakstring);
            return afterhash;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }


}

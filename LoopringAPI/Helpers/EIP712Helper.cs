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
    public static class EIP712Helper
    {
        public static string GenerateTransferSignature(BigInteger chainId, ApiTransferRequest transferRequest, string ethPrivateKey)
        {             
            Domain exchangeDomain = new Domain();
            exchangeDomain.Name = Constants.EIP721DomainName;
            exchangeDomain.Version = Constants.EIP721DomainVersion;
            exchangeDomain.ChainId = chainId;
            exchangeDomain.VerifyingContract = transferRequest.exchange;            

            string primaryTypeName = "Transfer";

            TypedData eip712TypedData = new TypedData();
            eip712TypedData.Domain = exchangeDomain;
            eip712TypedData.PrimaryType = primaryTypeName;
            eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
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
            eip712TypedData.Message = new[]
            {
                new MemberValue {TypeName = "address", Value = transferRequest.payerAddr},
                new MemberValue {TypeName = "address", Value = transferRequest.payeeAddr},
                new MemberValue {TypeName = "uint16", Value = transferRequest.token.tokenId},
                new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(transferRequest.token.volume)},
                new MemberValue {TypeName = "uint16", Value = transferRequest.maxFee.tokenId},
                new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(transferRequest.maxFee.volume)},
                new MemberValue {TypeName = "uint32", Value = transferRequest.validUntil},
                new MemberValue {TypeName = "uint32", Value = transferRequest.storageId},
            };

            return GenerateSignature(eip712TypedData, ethPrivateKey);
        }

        public static string GenerateSignature(TypedData eip712TypedData, string ethPrivateKey)
        {
            Eip712TypedDataSigner singer = new Eip712TypedDataSigner();
            var ethECKey = new Nethereum.Signer.EthECKey(ethPrivateKey.Replace("0x", ""));
            var encodedTypedData = singer.EncodeTypedData(eip712TypedData);
            var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
            var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);

            return serializedECDRSASignature + "0" + (int)EthSignType.EIP_712;
        }

        public static string GetPublicAddress(string privateKey)
        {
            var key = new EthECKey(privateKey.HexToByteArray(), true);
            return key.GetPublicAddress();
        }
    }
}

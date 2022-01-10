using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Nethereum.Web3;

namespace LoopringSharp
{
    public static class ECDSAHelper
    {
        public static string CreateSerializedTransferTypedData(BigInteger chainId, ApiTransferRequest transferRequest)
        {
            var result = $"{{\"domain\":{{\"chainId\": {chainId},\"name\": \"{Constants.EIP721DomainName}\",\"verifyingContract\":\"{transferRequest.exchange}\",\"version\":\"{Constants.EIP721DomainVersion}\"}},";
            result += $"\"message\":{{\"from\":\"{transferRequest.payerAddr}\",\"to\":\"{transferRequest.payeeAddr}\",\"tokenID\":{transferRequest.token.tokenId},\"amount\":\"{BigInteger.Parse(transferRequest.token.volume)}\"," +
                $"\"feeTokenID\":{transferRequest.maxFee.tokenId},\"maxFee\":\"{BigInteger.Parse(transferRequest.maxFee.volume)}\",\"validUntil\":{transferRequest.validUntil},\"storageID\":{transferRequest.storageId}}}," +
                $"\"primaryType\":\"Transfer\"," +
                $"\"types\":{{" +
                "\"EIP712Domain\":[{\"name\":\"name\",\"type\":\"string\"},{\"name\":\"version\",\"type\":\"string\"},{\"name\":\"chainId\",\"type\":\"uint256\"},{\"name\":\"verifyingContract\",\"type\":\"address\"}]," +
                "\"Transfer\":[{\"name\":\"from\",\"type\":\"address\"},{\"name\":\"to\",\"type\":\"address\"},{\"name\":\"tokenID\",\"type\":\"uint16\"},{\"name\":\"amount\",\"type\":\"uint96\"},{\"name\":\"feeTokenID\",\"type\":\"uint16\"},{\"name\":\"maxFee\",\"type\":\"uint96\"},{\"name\":\"validUntil\",\"type\":\"uint32\"},{\"name\":\"storageID\",\"type\":\"uint32\"}]" +
                $"}}}}";

            return result;
        }

        public static (TypedData, AccountUpdateTypedData) GenerateAccountUpdateTypedData(BigInteger chainId, ApiUpdateEDDSARequest accountUpdateRequest)
        {
            string primaryTypeName = "AccountUpdate";
            AccountUpdateTypedData typedData = new AccountUpdateTypedData()
            {
                domain = new AccountUpdateTypedData.Domain()
                {
                    name = Constants.EIP721DomainName,
                    version = Constants.EIP721DomainVersion,
                    chainId = chainId,
                    verifyingContract = accountUpdateRequest.exchange,
                },
                message = new AccountUpdateTypedData.Message()
                {
                    owner = accountUpdateRequest.owner,
                    accountID = accountUpdateRequest.accountId,
                    feeTokenID = accountUpdateRequest.maxFee.tokenId,
                    maxFee = accountUpdateRequest.maxFee.volume,
                    publicKey = accountUpdateRequest.publicKey.y,
                    validUntil = accountUpdateRequest.validUntil,
                    nonce = accountUpdateRequest.nonce
                },
                primaryType = primaryTypeName,
                types = new AccountUpdateTypedData.Types()
                {
                    EIP712Domain = new List<Type>()
                    {
                        new Type(){ name = "name", type = "string"},
                        new Type(){ name="version", type = "string"},
                        new Type(){ name="chainId", type = "uint256"},
                        new Type(){ name="verifyingContract", type = "address"},
                    },
                    AccountUpdate = new List<Type>()
                    {
                        new Type(){ name = "owner", type = "address"},
                        new Type(){ name = "accountID", type = "uint32"},
                        new Type(){ name = "feeTokenID", type = "uint16"},
                        new Type(){ name = "maxFee", type = "uint96"},
                        new Type(){ name = "publicKey", type = "uint256"},
                        new Type(){ name = "validUntil", type = "uint32"},
                        new Type(){ name = "nonce", type = "uint32"},
                    }
                }
            };

            TypedData eip712TypedData = new TypedData();
            eip712TypedData.Domain = new Domain()
            {
                Name = Constants.EIP721DomainName,
                Version = Constants.EIP721DomainVersion,
                ChainId = chainId,
                VerifyingContract = accountUpdateRequest.exchange,
            };
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
                        new MemberDescription {Name = "owner", Type = "address"},            // payerAddr
                        new MemberDescription {Name = "accountID", Type = "uint32"},              // payeeAddr
                        new MemberDescription {Name = "feeTokenID", Type = "uint16"},          // token.tokenId 
                        new MemberDescription {Name = "maxFee", Type = "uint96"},           // token.volume 
                        new MemberDescription {Name = "publicKey", Type = "uint256"},       // maxFee.tokenId
                        new MemberDescription {Name = "validUntil", Type = "uint32"},           // maxFee.volume
                        new MemberDescription {Name = "nonce", Type = "uint32"},       // validUntill
                    },

            };
            eip712TypedData.Message = new[]
            {
                new MemberValue {TypeName = "address", Value = accountUpdateRequest.owner},
                new MemberValue {TypeName = "uint32", Value = accountUpdateRequest.accountId},
                new MemberValue {TypeName = "uint16", Value = accountUpdateRequest.maxFee.tokenId},
                new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(accountUpdateRequest.maxFee.volume)},
                new MemberValue {TypeName = "uint256", Value = accountUpdateRequest.publicKey.y},
                new MemberValue {TypeName = "uint32", Value = accountUpdateRequest.validUntil},
                new MemberValue {TypeName = "uint32", Value = accountUpdateRequest.nonce},
            };

            return (eip712TypedData, typedData);
        }

        public static (TypedData, TransferTypedData) GenerateTransferTypedData(BigInteger chainId, ApiTransferRequest transferRequest)
        {
            string primaryTypeName = "Transfer";

            TypedData eip712TypedData = new TypedData();
            eip712TypedData.Domain = new Domain()
            {
                Name = Constants.EIP721DomainName,
                Version = Constants.EIP721DomainVersion,
                ChainId = chainId,
                VerifyingContract = transferRequest.exchange,
            };
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

            TransferTypedData typedData = new TransferTypedData()
            {
                domain = new TransferTypedData.Domain()
                {
                    name = Constants.EIP721DomainName,
                    version = Constants.EIP721DomainVersion,
                    chainId = chainId,
                    verifyingContract = transferRequest.exchange,
                },
                message = new TransferTypedData.Message()
                {
                    from = transferRequest.payerAddr,
                    to = transferRequest.payeeAddr,
                    tokenID = transferRequest.token.tokenId,
                    amount = transferRequest.token.volume,
                    feeTokenID = transferRequest.maxFee.tokenId,
                    maxFee = transferRequest.maxFee.volume,
                    validUntil = transferRequest.validUntil,
                    storageID = transferRequest.storageId
                },
                primaryType = primaryTypeName,
                types = new TransferTypedData.Types()
                {
                    EIP712Domain = new List<Type>()
                    {
                        new Type(){ name = "name", type = "string"},
                        new Type(){ name="version", type = "string"},
                        new Type(){ name="chainId", type = "uint256"},
                        new Type(){ name="verifyingContract", type = "address"},
                    },
                    Transfer = new List<Type>()
                    {
                        new Type(){ name = "from", type = "address"},
                        new Type(){ name = "to", type = "address"},
                        new Type(){ name = "tokenID", type = "uint16"},
                        new Type(){ name = "amount", type = "uint96"},
                        new Type(){ name = "feeTokenID", type = "uint16"},
                        new Type(){ name = "maxFee", type = "uint96"},
                        new Type(){ name = "validUntil", type = "uint32"},
                        new Type(){ name = "storageID", type = "uint32"},
                    }
                }
            };

            return (eip712TypedData, typedData);
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

        public static string GenerateSignature(TypedData eip712TypedData)
        {
            Eip712TypedDataSigner singer = new Eip712TypedDataSigner();            
            var encodedTypedData = singer.EncodeTypedData(eip712TypedData);
            var sha256Managed = new SHA256Managed();            
            return WalletConnectSharp.Core.Utils.Hex.ToHex(sha256Managed.ComputeHash(encodedTypedData));

        }

        public static string GetPublicAddress(string privateKey)
        {
            var key = new EthECKey(privateKey.HexToByteArray(), true);
            return key.GetPublicAddress();
        }
    }
}

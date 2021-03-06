using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WalletConnectSharp.Core.Events;
using WalletConnectSharp.Core.Events.Request;
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Core.Models.Ethereum;
using WalletConnectSharp.Core.Network;
using WalletConnectSharp.Core.Utils;

namespace WalletConnectSharp.Core
{
    // Copied from https://github.com/WalletConnect/WalletConnectSharp
    // DID Not fork due to lack of maintenance on the project and disagreeance with coding standards
    // All credit for base functionality goes to original authors,
    // I wanted a bit more granular control over this implementation

    public class WalletConnectSession : WalletConnectProtocol
    {        
        private string _handshakeTopic;
        
        private long _handshakeId;
        
        public event EventHandler<WalletConnectSession> OnSessionConnect;
        public event EventHandler OnSessionDisconnect;
        public event EventHandler<WalletConnectSession> OnSend;
        public event EventHandler<WCSessionData> SessionUpdate;

        public int NetworkId { get; private set; }
        
        public string[] Accounts { get; private set; }
        
        public bool ReadyForUserPrompt { get; private set; }

        public int ChainId { get; private set; }

        private string clientId = "";

        public string URI
        {
            get
            {
                var topicEncode = WebUtility.UrlEncode(_handshakeTopic);
                var versionEncode = WebUtility.UrlEncode(Version);
                var bridgeUrlEncode = WebUtility.UrlEncode(_bridgeUrl);
                var keyEncoded = WebUtility.UrlEncode(_key);

                return "wc:" + topicEncode + "@" + versionEncode + "?bridge=" + bridgeUrlEncode + "&key=" + keyEncoded;
            }
        }
      
        public WalletConnectSession(ClientMeta clientMeta, string bridgeUrl = null, ITransport transport = null, ICipher cipher = null, int chainId = 1, EventDelegator eventDelegator = null) : base(transport, cipher, eventDelegator)
        {
            if (clientMeta == null)
            {
                throw new ArgumentException("clientMeta cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(clientMeta.Description))
            {
                throw new ArgumentException("clientMeta must include a valid Description");
            }
            
            if (string.IsNullOrWhiteSpace(clientMeta.Name))
            {
                throw new ArgumentException("clientMeta must include a valid Name");
            }
            
            if (string.IsNullOrWhiteSpace(clientMeta.URL))
            {
                throw new ArgumentException("clientMeta must include a valid URL");
            }
            
            if (clientMeta.Icons == null || clientMeta.Icons.Length == 0)
            {
                throw new ArgumentException("clientMeta must include an array of Icons the Wallet app can use. These Icons must be URLs to images. You must include at least one image URL to use");
            }
            
            if (bridgeUrl == null)
            {
                bridgeUrl = DefaultBridge.ChooseRandomBridge();
            }

            bridgeUrl = DefaultBridge.GetBridgeUrl(bridgeUrl);
            
            if (bridgeUrl.StartsWith("https"))
                bridgeUrl = bridgeUrl.Replace("https", "wss");
            else if (bridgeUrl.StartsWith("http"))
                bridgeUrl = bridgeUrl.Replace("http", "ws");
            
            this.DappMetadata = clientMeta;
            this.ChainId = chainId;
            this._bridgeUrl = bridgeUrl;

            this.SessionConnected = false;
            
            CreateNewSession();
        }

        public void CreateNewSession(bool force = false)
        {
            if (SessionConnected && !force)
            {
                throw new IOException("You must disconnect the current session before you can create a new one");
            }

            var topicGuid = Guid.NewGuid();

            _handshakeTopic = topicGuid.ToString();

            clientId = Guid.NewGuid().ToString();

            GenerateKey();
        }
        
        private void GenerateKey()
        {
            //Generate a random secret
            byte[] secret = new byte[32];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(secret);

            this._keyRaw = secret;

            //Convert hex 
            this._key = this._keyRaw.ToHex().ToLower();
        }

        public virtual async Task<WCSessionData> ConnectSession()
        {
            if (!base.TransportConnected)
            {
                await base.SetupTransport();
            }

            ReadyForUserPrompt = false;
            await SubscribeAndListenToTopic(this.clientId);
            
            ListenToTopic(this._handshakeTopic);

            WCSessionData result;
            if (!SessionConnected)
            {
                result = await CreateSession();
                //Reset this back after we have established a session
                ReadyForUserPrompt = false;
                
            }
            else
            {
                result = new WCSessionData()
                {
                    accounts = Accounts,
                    approved = true,
                    chainId = ChainId,
                    networkId = NetworkId,
                    peerId = PeerId,
                    peerMeta = WalletMetadata
                };
            }
            
            if (OnSessionConnect != null)
                OnSessionConnect(this, this);

            return result;
        }
        
        public override async Task Connect()
        {
            await base.Connect();

            await ConnectSession();
        }

        public async Task DisconnectSession(string disconnectMessage = "Session Disconnected", bool createNewSession = true)
        {
            var request = new WCSessionUpdate(new WCSessionData()
            {
                approved = false,
                chainId = 0,
                accounts = null,
                networkId = 0
            });

            await SendRequest(request);
            
            await base.Disconnect();
            
            HandleSessionDisconnect(disconnectMessage, "disconnect", createNewSession);
        }

        public override async Task Disconnect()
        {
            await DisconnectSession();
        }
        
        public async Task<string> EthSign(string address, string message, Encoding messageEncoding = null)
        {
            if (!message.IsHex())
            {
                var encoding = messageEncoding;
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                
                message = "0x" + encoding.GetBytes(message).ToHex();
            }
            
            var request = new EthPersonalSign(address, message);

            var response = await BetterSend(request);

            return response.Result;
        }   

        public async Task<string> EthSignTypedDataV4(string address,string serializedData)
        {
            var request = new EthSignTypedDataV4(address, serializedData);
            ;
            var response = await BetterSend(request);

            return response.Result;
        }      

        public static EthResponse receivedResponse;

        public async Task<EthResponse> BetterSend(EthPersonalSign data) 
        {
            while(receivedResponse!=null)
            {
                System.Threading.Thread.Sleep(100);
            }

            TaskCompletionSource<EthResponse> eventCompleted = new TaskCompletionSource<EthResponse>(TaskCreationOptions.None);

            Events.ListenForResponse<EthResponse>(data.ID, (sender, @event) =>
            {
                var response = @event.Response;
                if (response.IsError)
                {
                    eventCompleted.SetException(new IOException(response.Error.Message));
                }
                else
                {
                    receivedResponse = response;
                    eventCompleted.SetResult(response);                    
                }

            });

            await SendRequest(data);

            while(receivedResponse ==null)
            {
                System.Threading.Thread.Sleep(100);
            }
            var resturn = Newtonsoft.Json.JsonConvert.DeserializeObject<EthResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(receivedResponse));
            receivedResponse = null;
            return resturn;
        }

        public async Task<EthResponse> BetterSend(EthSignTypedDataV4 data)
        {
            while (receivedResponse != null)
            {
                System.Threading.Thread.Sleep(100);
            }

            TaskCompletionSource<EthResponse> eventCompleted = new TaskCompletionSource<EthResponse>(TaskCreationOptions.None);

            Events.ListenForResponse<EthResponse>(data.ID, (sender, @event) =>
            {
                var response = @event.Response;
                if (response.IsError)
                {
                    eventCompleted.SetException(new IOException(response.Error.Message));
                }
                else
                {
                    receivedResponse = response;
                    eventCompleted.SetResult(response);
                }

            });

            await SendRequest(data);

            while (receivedResponse == null)
            {
                System.Threading.Thread.Sleep(100);
            }
            var resturn = Newtonsoft.Json.JsonConvert.DeserializeObject<EthResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(receivedResponse));
            receivedResponse = null;
            return resturn;
        }

        /// <summary>
        /// Create a new WalletConnect session with a Wallet.
        /// </summary>
        /// <returns></returns>
        private async Task<WCSessionData> CreateSession()
        {
            var data = new WcSessionRequest(DappMetadata, clientId, ChainId);

            this._handshakeId = data.ID;

            await SendRequest(data, this._handshakeTopic);

            TaskCompletionSource<WCSessionData> eventCompleted =
                new TaskCompletionSource<WCSessionData>(TaskCreationOptions.None);

            //Listen for the _handshakeId response
            //The response will be of type WCSessionRequestResponse
            Events.ListenForResponse<WCSessionRequestResponse>(this._handshakeId, HandleSessionResponse);
            
            //Listen for wc_sessionUpdate requests
            Events.ListenFor("wc_sessionUpdate",
                (object sender, GenericEvent<WCSessionUpdate> @event) =>
                    HandleSessionUpdate(@event.Response.parameters[0]));

            //Listen for the "connect" event triggered by 'HandleSessionResponse' above
            //This will have the type WCSessionData
            Events.ListenFor<WCSessionData>("connect",
                (sender, @event) =>
                {
                    eventCompleted.TrySetResult(@event.Response);
                });
            
            //Listen for the "session_failed" event triggered by 'HandleSessionResponse' above
            //This will have the type failure reason
            Events.ListenFor<ErrorResponse>("session_failed",
                delegate(object sender, GenericEvent<ErrorResponse> @event)
                {
                    if (@event.Response.Message == "Not Approved" || @event.Response.Message == "Session Rejected")
                    {
                        eventCompleted.TrySetCanceled();
                    }
                    else
                    {
                        eventCompleted.TrySetException(
                            new IOException("WalletConnect: Session Failed: " + @event.Response.Message));
                    }
                });
            
            ReadyForUserPrompt = true;

            var response = await eventCompleted.Task;

            return response;
        }

        private void HandleSessionResponse(object sender, JsonRpcResponseEvent<WCSessionRequestResponse> jsonresponse)
        {
            var response = jsonresponse.Response.result;

            if (response != null && response.approved)
            {
                HandleSessionUpdate(response);
            }
            else if (jsonresponse.Response.IsError)
            {
                HandleSessionDisconnect(jsonresponse.Response.Error.Message, "session_failed");
            }
            else
            {
                HandleSessionDisconnect("Not Approved", "session_failed");
            }
        }

        private void HandleSessionUpdate(WCSessionData data)
        {
            if (data == null) return;

            bool wasConnected = SessionConnected;

            //We are connected if we are approved
            SessionConnected = data.approved;

            ChainId = data.chainId;

            NetworkId = data.networkId;

            Accounts = data.accounts;

            if (!wasConnected)
            {
                PeerId = data.peerId;

                WalletMetadata = data.peerMeta;

                Events.Trigger("connect", data);
            }
            else
            {
                Events.Trigger("session_update", data);
            }

            if (SessionUpdate != null)
                SessionUpdate(this, data);
        }

        private void HandleSessionDisconnect(string msg, string topic = "disconnect", bool createNewSession = true)
        {
            SessionConnected = false;

            Events.Trigger(topic, new ErrorResponse(msg));

            if (TransportConnected)
            {
                DisconnectTransport();
            }

            CreateNewSession();
            
            _activeTopics.Clear();
            
            Events.Clear();

            if (OnSessionDisconnect != null)
                OnSessionDisconnect(this, EventArgs.Empty);
        }
        
        
        
        /// <summary>
        /// Creates and returns a serializable class that holds all session data required to resume later
        /// </summary>
        /// <returns></returns>
        public SavedSession SaveSession()
        {
            if (!SessionConnected)
            {
                return null;
            }
            
            return new SavedSession(clientId, _bridgeUrl, _key, _keyRaw, PeerId, NetworkId, Accounts, ChainId, DappMetadata, WalletMetadata);
        }

        /// <summary>
        /// Save the current session to a Stream. This function will write a GZIP Compressed JSON blob
        /// of the contents of SaveSession()
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="leaveStreamOpen">Whether to leave the stream open</param>
        /// <exception cref="IOException">If there is currently no session active, or if writing to the stream fails</exception>
        public void SaveSession(Stream stream, bool leaveStreamOpen = true)
        {
            //We'll save the current session as a GZIP compressed JSON blob
            var data = SaveSession();

            if (data == null)
            {
                throw new IOException("No session is active to save");
            }

            var json = JsonConvert.SerializeObject(data);

            byte[] encodedJson = Encoding.UTF8.GetBytes(json);
            
            using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress, leaveStreamOpen))
            {
                byte[] sizeEncoded = BitConverter.GetBytes(encodedJson.Length);
                
                gZipStream.Write(sizeEncoded, 0, sizeEncoded.Length);
                gZipStream.Write(encodedJson, 0, encodedJson.Length);
            }
        }

        /// <summary>
        /// Reads a GZIP Compressed JSON blob of a SavedSession object from a given Stream. This is the reverse of
        /// SaveSession(Stream)
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="leaveStreamOpen">Whether to leave the stream open</param>
        /// <exception cref="IOException">If reading from the stream fails</exception>
        /// <returns>A SavedSession object</returns>
        public static SavedSession ReadSession(Stream stream, bool leaveStreamOpen = true)
        {
            string json;
            using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress, leaveStreamOpen))
            {
                byte[] sizeEncoded = new byte[4];

                gZipStream.Read(sizeEncoded, 0, 4);

                int size = BitConverter.ToInt32(sizeEncoded, 0);

                byte[] jsonEncoded = new byte[size];

                gZipStream.Read(jsonEncoded, 0, size);

                json = Encoding.UTF8.GetString(jsonEncoded);
            }

            return JsonConvert.DeserializeObject<SavedSession>(json);
        }
    }
}
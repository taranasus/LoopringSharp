using System;

namespace LoopringSharp
{
    public class OperationResult
    {
        public OperationResult(ApiTransferResult apiTransfer)
        {
            hash = apiTransfer.hash;
            isIdempotent = apiTransfer.isIdempotent;
            status = (Status)Enum.Parse(typeof(Status), apiTransfer.status, true);
        }

        public ApiTransferResult ToApiTransferResult()
        {
            return new ApiTransferResult()
            {
                hash = hash,
                isIdempotent = isIdempotent,
                status = status.ToString()
            };
        }

        public string hash { get; set; }
        public Status status { get; set; }
        public bool isIdempotent { get; set; }
    }
}

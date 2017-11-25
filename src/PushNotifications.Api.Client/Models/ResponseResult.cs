using System.Collections.Generic;

namespace PushNotifications.Api.Client.Models
{
    public class ResponseResult
    {
        public ResponseResult()
        {
            Errors = new List<string>();
        }

        public IEnumerable<string> Errors { get; }
        public bool IsSuccess { get; }
    }


    public class ResponseResult<T> : ResponseResult
    {
        public ResponseResult() : base()
        {

        }

        public T Result { get; private set; }
    }
}

using System.Collections.Generic;

namespace PushNotifications.Api.Client.Models
{
    public class ResponseResult
    {
        public ResponseResult()
        {
            Errors = new List<string>();
        }

        public IEnumerable<string> Errors { get; private set; }
        public bool IsSuccess { get; private set; }
    }


    public class ResponseResult<T> : ResponseResult
    {
        public ResponseResult() : base()
        {

        }

        public T Result { get; private set; }
    }
}

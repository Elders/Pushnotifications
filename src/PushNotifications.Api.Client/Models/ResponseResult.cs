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
}

using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Delivery
{
    public class SubscribeUnsubscribeResult
    {
        SubscribeUnsubscribeResult()
        {
            Errors = Enumerable.Empty<string>();
        }

        SubscribeUnsubscribeResult(IEnumerable<string> errors) : this()
        {
            Errors = errors;
        }

        SubscribeUnsubscribeResult(string error)
        {
            Errors = new List<string> { error };
        }

        public IEnumerable<string> Errors { get; private set; }

        public bool IsSuccess
        {
            get { return Errors.Any() == false; }
        }

        public bool HasInvalidTokens
        {
            get
            {
                return Errors.Any(x => x.Equals("invalid-argument") || x.Equals("registration-token-not-registered"));
            }
        }

        public static SubscribeUnsubscribeResult Successful() => new SubscribeUnsubscribeResult();

        public static SubscribeUnsubscribeResult Unsuccessful(IEnumerable<string> errors) => new SubscribeUnsubscribeResult(errors);
        public static SubscribeUnsubscribeResult Unsuccessful(string error) => new SubscribeUnsubscribeResult(error);

    }
}

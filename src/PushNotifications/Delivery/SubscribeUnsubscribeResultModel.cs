using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Delivery
{
    public class SubscribeUnsubscribeHandler
    {
        SubscribeUnsubscribeHandler()
        {
            Errors = Enumerable.Empty<string>();
        }

        SubscribeUnsubscribeHandler(IEnumerable<string> errors) : this()
        {
            Errors = errors;
        }

        public IEnumerable<string> Errors { get; private set; }

        public bool IsSuccess
        {
            get { return Errors.Any() == false; }
        }

        public bool InvalidTokens
        {
            get
            {
                return Errors.Any(x => x.Equals("invalid-argument") || x.Equals("registration-token-not-registered"));
            }
        }

        public static SubscribeUnsubscribeHandler Successful() => new SubscribeUnsubscribeHandler();

        public static SubscribeUnsubscribeHandler Unsuccessful(IEnumerable<string> errors) => new SubscribeUnsubscribeHandler(errors);
    }
}

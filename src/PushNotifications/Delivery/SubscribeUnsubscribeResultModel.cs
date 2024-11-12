using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Delivery
{
    public class SubscribeUnsubscribeResultModel
    {
        SubscribeUnsubscribeResultModel()
        {
            Errors = Enumerable.Empty<string>();
        }

        SubscribeUnsubscribeResultModel(IEnumerable<string> errors) : this()
        {
            Errors = errors;
        }

        SubscribeUnsubscribeResultModel(string error)
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

        public static SubscribeUnsubscribeResultModel Successful() => new SubscribeUnsubscribeResultModel();

        public static SubscribeUnsubscribeResultModel Unsuccessful(IEnumerable<string> errors) => new SubscribeUnsubscribeResultModel(errors);
        public static SubscribeUnsubscribeResultModel Unsuccessful(string error) => new SubscribeUnsubscribeResultModel(error);

    }
}

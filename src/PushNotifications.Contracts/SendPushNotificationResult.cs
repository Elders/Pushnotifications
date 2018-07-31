using System.Collections.Generic;

namespace PushNotifications.Contracts
{
    public class SendTokensResult
    {
        private List<SubscriptionToken> failedTokens;

        public SendTokensResult(IEnumerable<SubscriptionToken> failedTokens)
        {
            this.failedTokens = new List<SubscriptionToken>(failedTokens);
        }

        public IReadOnlyCollection<SubscriptionToken> FailedTokens { get { return failedTokens.AsReadOnly(); } }

        public bool HasFailedTokens { get { return FailedTokens?.Count > 0; } }

        public static SendTokensResult Success = new SendTokensResult(new List<SubscriptionToken>());
    }
}

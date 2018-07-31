using System;
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

        /// <summary>
        /// For some reason the request to the 3rd party failed. However we do nothing in this case for now.
        /// Ideally we want to execute a retry strategy with Hystrix but it MUST NOT be based on this `Failed` status.
        /// </summary>
        public static SendTokensResult Failed = new SendTokensResult(new List<SubscriptionToken>());
    }
}

﻿using PushNotifications.Subscriptions;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.PushNotifications
{
    public class SendTokensResult
    {
        private readonly List<SubscriptionToken> failedTokens;

        public SendTokensResult(IEnumerable<SubscriptionToken> failedTokens)
        {
            this.failedTokens = new List<SubscriptionToken>(failedTokens);
        }

        public IReadOnlyCollection<SubscriptionToken> FailedTokens { get { return failedTokens.AsReadOnly(); } }

        public bool HasFailedTokens { get { return FailedTokens?.Count > 0; } }

        public static SendTokensResult Success = new SendTokensResult(new List<SubscriptionToken>());

        /// <summary>
        /// For some reason the request to the 3rd party failed. However we do nothing in this case for now.
        /// </summary>
        public static SendTokensResult Failed = new SendTokensResult(new List<SubscriptionToken>());

        public static SendTokensResult operator +(SendTokensResult left, SendTokensResult right) => new SendTokensResult(left.FailedTokens.Union(right.FailedTokens));
    }
}

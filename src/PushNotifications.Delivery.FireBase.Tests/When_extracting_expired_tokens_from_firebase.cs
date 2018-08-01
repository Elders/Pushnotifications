using Machine.Specifications;
using PushNotifications.Contracts;
using PushNotifications.Contracts.Subscriptions;
using PushNotifications.Delivery.FireBase.Models;
using System.Collections.Generic;

namespace PushNotifications.Delivery.FireBase.Tests
{
    [Subject(nameof(FireBaseDelivery))]
    public class When_extracting_expired_tokens_from_firebase
    {
        Establish context = () =>
        {
            subscriptionType = SubscriptionType.FireBase;
            notWorkingSubscriptionToken = new SubscriptionToken("firebase_subscription_token_not_working", subscriptionType);
            workingSubscriptionToken = new SubscriptionToken("firebase_subscription_token_working", subscriptionType);

            var notRegisteredError = new FireBaseResponseModel.FireBaseResponseResultModel() { Error = "NotRegistered" };
            var successfullToken = new FireBaseResponseModel.FireBaseResponseResultModel() { Error = "id_123123" };

            responseFromFirebase = new List<FireBaseResponseModel.FireBaseResponseResultModel>()
            {
                notRegisteredError, successfullToken,
                notRegisteredError, successfullToken
            };

            subscriptionTokens = new List<SubscriptionToken>()
            {
                notWorkingSubscriptionToken, workingSubscriptionToken,
                notWorkingSubscriptionToken, workingSubscriptionToken
            };
        };

        Because of = () => result = FireBaseResponseModel.ExpiredTokensDetector.GetNotRegisteredTokens(subscriptionTokens, responseFromFirebase);

        It should_have_failed_tokens = () => result.HasFailedTokens.ShouldBeTrue();

        It should_have_two_failed_tokens = () => result.FailedTokens.ShouldContainOnly(notWorkingSubscriptionToken, notWorkingSubscriptionToken);

        It should_have_failed_tokens_not_empty = () => result.FailedTokens.ShouldNotBeEmpty();

        private static SubscriptionType subscriptionType;
        private static List<FireBaseResponseModel.FireBaseResponseResultModel> responseFromFirebase;
        private static List<SubscriptionToken> subscriptionTokens;
        private static SendTokensResult result;
        private static SubscriptionToken notWorkingSubscriptionToken;
        private static SubscriptionToken workingSubscriptionToken;
    }
}

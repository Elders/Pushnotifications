using Elders.Cronus.DomainModeling;
using Elders.Web.Api;
using PushNotifications.Contracts.PushNotifications;
using PushNotifications.Contracts.PushNotifications.Commands;
using System;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;

namespace PushNotifications.Api.Controllers.PushNotifications
{
    [ScopeAuthorize("admin")]
    [RoutePrefix("PushNotifications")]
    public class PushNotificationsController : ApiController
    {
        public IPublisher<ICommand> Publisher { get; set; }

        [HttpPost, Route("Send")]
        public IHttpActionResult Send(PushNotificationModel model)
        {
            var result = new ResponseResult(Constants.InvalidCommand);

            var command = model.AsCommand();
            if (command.IsValid())
            {
                result = Publisher.Publish(command)
                    ? new ResponseResult<ResponseResult>(new ResponseResult())
                    : new ResponseResult(Constants.CommandPublishFailed);
            }
            return result.IsSuccess
                ? this.Accepted(result)
                : this.NotAcceptable(result);
        }
    }

    public class PushNotificationModel
    {
        public string UserId { get; set; }

        public string Json { get; set; }

        public string Text { get; set; }

        public string Sound { get; set; }

        public string Icon { get; set; }

        public string Category { get; set; }

        public int Badge { get; set; }

        public bool IsSilent { get; set; }

        public SendPushNotification AsCommand()
        {
            var id = new PushNotificationId(Guid.NewGuid());
            return new SendPushNotification(id, UserId, Json, Text, Sound, Icon, Category, Badge, IsSilent);
        }
    }
}

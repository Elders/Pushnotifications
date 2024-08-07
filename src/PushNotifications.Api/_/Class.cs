﻿using Elders.Cronus;
using Elders.Cronus.MessageProcessing;
using Microsoft.AspNetCore.Http;
using PushNotifications.Subscriptions;
using System;
using System.Linq;
using System.Security.Claims;

namespace PushNotifications.Api
{
    public class ApiContext
    {
        private readonly ICronusContextAccessor cronusContextAccessor;

        public ApiContext(IHttpContextAccessor httpContextAccessor, ICronusContextAccessor cronusContextAccessor)
        {
            this.HttpContextAccessor = httpContextAccessor;
            this.cronusContextAccessor = cronusContextAccessor;
        }

        public IHttpContextAccessor HttpContextAccessor { get; }

        public CurrentUser CurrentUser => new CurrentUser(this);

        public string Tenant => cronusContextAccessor.CronusContext.Tenant;

        public string Application => GetApplication();

        private string GetApplication()
        {
            string application = HttpContextAccessor.HttpContext.User.Claims
                .Where(c => c.Type.Equals(AuthorizeClaimType.Application, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault()?.Value;

            if (string.IsNullOrEmpty(application))
                throw new Exception("Koceto da mu misli v IAA. Add the app claim.");

            return application;
        }
    }

    public class CurrentUser
    {
        private readonly ApiContext apiContext;

        public CurrentUser(ApiContext apiContext)
        {
            this.apiContext = apiContext;
        }

        public DeviceSubscriberId UserId => GetUserIdFromHttpContext();

        public bool HasRole(string role)
        {
            if (string.IsNullOrEmpty(role)) throw new ArgumentNullException(nameof(role));

            return apiContext.HttpContextAccessor.HttpContext.User.IsInRole(role);
        }

        private DeviceSubscriberId GetUserIdFromHttpContext()
        {
            var claim = apiContext.HttpContextAccessor.HttpContext.User.Claims
                .Where(c => c.Type.Equals(AuthorizeClaimType.Subject, StringComparison.OrdinalIgnoreCase) || c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();

            if (claim is null)
            {
                return default;
            }

            if (Urn.IsUrn(claim.Value))
            {
                return new DeviceSubscriberId(apiContext.Tenant, AggregateRootId.Parse(claim.Value).Id, apiContext.Application);
            }
            else
            {
                return new DeviceSubscriberId(apiContext.Tenant, claim.Value, apiContext.Application);
            }
        }
    }

    public static class AuthorizeClaimType
    {
        public const string Subject = "sub";
        public const string Tenant = "tenant";
        public const string Application = "application";
        public const string TenantClient = "client_tenant";
        public const string Organizations = "organizations";
    }
}

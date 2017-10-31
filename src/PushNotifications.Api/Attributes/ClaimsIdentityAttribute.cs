using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Api.Attributes
{
    public class ClaimsIdentityAttribute : Attribute
    {
        public List<string> ClaimTypes { get; private set; }

        public ClaimsIdentityAttribute(params string[] claims)
        {
            ClaimTypes = claims.ToList();
        }
    }
}

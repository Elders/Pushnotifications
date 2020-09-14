using Elders.Cronus;
using System;
using System.ComponentModel.DataAnnotations;

namespace PushNotifications.Api
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IsUrnAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is null) return true;

            return Urn.IsUrn(value as string);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The field {name} is not a valid Urn. Urn fields must comply to the pattern: {UrnRegex.Pattern}";
        }
    }
}

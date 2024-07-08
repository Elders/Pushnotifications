namespace PushNotifications.Api
{
    public class MustHaveAtLeastOneElementAttribute : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var collection = value as System.Collections.IEnumerable;
            if (collection != null && collection.GetEnumerator().MoveNext())
            {
                return true;
            }
            return false;
        }
    }
}

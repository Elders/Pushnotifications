using System.Collections.Generic;

namespace PushNotifications.Delivery.Pushy.Models
{
    public class PushySendDataModel
    {
        public PushySendDataModel(string title, string body, string sound, string badge, Dictionary<string, object> data)
        {
            Data = new Dictionary<string, object>();

            Data.Add("title", title);
            Data.Add("body", body);
            Data.Add("sound", sound);
            Data.Add("badge", badge);

            foreach (var item in data)
            {
                if (data.ContainsKey(item.Key) == true)
                    data[item.Key] = item.Value;
                else
                    data.Add(item.Key, item.Value);
            }
        }

        public Dictionary<string, object> Data { get; private set; }
    }
}

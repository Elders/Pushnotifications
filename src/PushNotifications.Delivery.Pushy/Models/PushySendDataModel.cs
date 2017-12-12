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
                if (Data.ContainsKey(item.Key) == true)
                    Data[item.Key] = item.Value;
                else
                    Data.Add(item.Key, item.Value);
            }
        }

        public Dictionary<string, object> Data { get; private set; }
    }
}

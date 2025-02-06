using Newtonsoft.Json;


namespace APIInterfaces
{
    public static class JsonDataHandler
    {
        public static string JsonSerialize<T> (T data)
        {
            return JsonConvert.SerializeObject(data);
        }
        public static T JsonDeserialize<T> (string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

    }
}

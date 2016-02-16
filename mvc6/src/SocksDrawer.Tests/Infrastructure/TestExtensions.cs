using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SocksDrawer.Tests.Infrastructure
{
    public static class TestExtensions
    {
        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string requestUri, object content)
        {
            var postContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUri, postContent);
        }

        public static T BodyAs<T>(this HttpResponseMessage response)
        {
            if (typeof(T) == typeof(string))
                return (T)(object)response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }
    }
}

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SocksDrawer.Tests.Infrastructure
{
    public static class TestExtensions
    {
        public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string requestUri, T obj)
        {
            var param = JsonConvert.SerializeObject(obj);
            var contentPost = new StringContent(param, Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUri, contentPost);
        }

        public static T BodyAs<T>(this HttpResponseMessage response)
        {
            if (typeof(T) == typeof(string))
                return (T)(object)response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }
    }
}

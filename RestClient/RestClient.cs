using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace RestClient
{
    public class RestClient
    {
        public HttpStatusCode ResponseStatusCode { get; private set; }

        private readonly string endpoint;
        private static HttpClient HttpClient { get; set; }
        public RestClient(string endpoint)
        {
            this.endpoint = endpoint;
            HttpClient = new HttpClient();
        }

        public static RestClient CreateClient(string endpoint)
        {
            return new RestClient(endpoint);
        }

        #region Base
        public async Task ExecuteAsync(string url, HttpMethod method, object body = null)
        {
            using (HttpRequestMessage request = GetHttpRequestMessage(method, url, body))
            {
                var response = await SendRequestAsync(request);
            }
        }

        public async Task<T> ExecuteAsync<T>(string url, HttpMethod method, object body = null)
        {
            using (HttpRequestMessage request = GetHttpRequestMessage(method, url, body))
            {
                var response = await SendRequestAsync(request);

                if(response.Content == null)
                {
                    return default;
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<T>(responseBody);
                }
            }
        }

        public void Execute(string url, HttpMethod method, object body = null)
        {
            using (HttpRequestMessage request = GetHttpRequestMessage(method, url, body))
            {
                var response = SendRequest(request);
            }
        }
        public T Execute<T>(string url, HttpMethod method, object body = null)
        {
            using (HttpRequestMessage request = GetHttpRequestMessage(method, url, body))
            {
                var response = SendRequest(request);

                if (response.Content == null)
                {
                    return default;
                }
                else
                {
                    var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonConvert.DeserializeObject<T>(responseBody);
                }
            }
        }
        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            ResponseStatusCode = response.StatusCode;
            return response;
        }
        private HttpResponseMessage SendRequest(HttpRequestMessage request)
        {
            var response = HttpClient.SendAsync(request).GetAwaiter().GetResult();
            ResponseStatusCode = response.StatusCode;
            return response;
        }

        private HttpRequestMessage GetHttpRequestMessage(HttpMethod method, string url, object body = null) {
            var request  =  new HttpRequestMessage(){
                Method = method,
                RequestUri = new Uri($"{endpoint}/{url}")
            };
            if(body != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, new ContentType("application/json").MediaType);
            }
            return request;
        }
        #endregion
        #region GET
        public T ExecuteGet<T>(string url)
        {
            return Execute<T>(url, HttpMethod.Get);
        }

        public async Task<T> ExecuteGetAsync<T>(string url)
        {
            return await ExecuteAsync<T>(url, HttpMethod.Get);
        }
        #endregion GET
        #region POST
        public T ExecutePost<T>(string url, object body = null)
        {
            return Execute<T>(url, HttpMethod.Post, body);
        }

        public async Task<T> ExecutePostAsync<T>(string url, object body = null)
        {
            return await ExecuteAsync<T>(url, HttpMethod.Post, body);
        }

        public void ExecutePost(string url, object body = null)
        {
            Execute(url, HttpMethod.Post, body);
        }

        public async Task ExecutePostAsync(string url, object body = null)
        {
            await ExecuteAsync(url, HttpMethod.Post, body);
        }
        #endregion
        #region PUT
        public void ExecutePut(string url, object body = null)
        {
            Execute(url, HttpMethod.Put, body);
        }

        public async Task ExecutePutAsync(string url, object body = null)
        {
            await ExecuteAsync(url, HttpMethod.Put, body);
        }
        #endregion
        #region DELETE
        public void ExecuteDelete(string url)
        {
            Execute(url, HttpMethod.Delete);
        }

        public async Task ExecuteDeleteAsync(string url)
        {
            await ExecuteAsync(url, HttpMethod.Delete);
        }
        #endregion
    }
}

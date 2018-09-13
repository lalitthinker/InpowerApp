using ModernHttpClient;
using Newtonsoft.Json;
using PCL.Common;
using PCL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PCL.Helper
{
    public class HttpClientHelper
    {
        public async Task<List<T>> GetAll<T>(string url)
        {
            var lst = new List<T>();
            using (var client = new HttpClient(new NativeMessageHandler()))
            {

                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);
                if (!string.IsNullOrEmpty(GlobalConstant.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalConstant.AccessToken);
                }
                else
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "" + url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<List<T>>(content);

                }
            }
            return lst;
        }

        public async Task<T> Get<T>(string url)
        {
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);
                if (!string.IsNullOrEmpty(GlobalConstant.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalConstant.AccessToken);
                }

                else
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }
            }
            return default(T);
        }

        public async Task<InpowerResult> Post<T>(T obj, string url)
        {
            InpowerResult resp = new InpowerResult { Status = 0, Message = "", Response = null };
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);

                var json = JsonConvert.SerializeObject(obj);
                var sendContent = new StringContent(json, Encoding.UTF8, "application/json");
                if (!string.IsNullOrEmpty(GlobalConstant.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalConstant.AccessToken);
                }
                else
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    response = await client.PostAsync(client.BaseAddress + url, sendContent);
                }
                catch (Exception ex)
                {
                    resp.Message = ex.Message;
                }

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
            }
        }
        public async Task<InpowerResult> PostList<T>(List<T> obj, string url)
        {
            InpowerResult resp = new InpowerResult { Status = 0, Message = "", Response = null };
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);

                var json = JsonConvert.SerializeObject(obj);
                var sendContent = new StringContent(json, Encoding.UTF8, "application/json");
                if (!string.IsNullOrEmpty(GlobalConstant.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalConstant.AccessToken);
                }
                else
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    response = await client.PostAsync(client.BaseAddress + url, sendContent);
                }
                catch (Exception ex)
                {
                    resp.Message = ex.Message;
                }

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
            }
        }
        public async Task<string> PostToken(TokenRequestViewModel obj)
        {

            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.TokenURL);


                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("userName", obj.UserName));
                nvc.Add(new KeyValuePair<string, string>("Password", obj.password));
                nvc.Add(new KeyValuePair<string, string>("grant_type", "password"));

                var sendContent = new FormUrlEncodedContent(nvc);


                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    response = await client.PostAsync(client.BaseAddress, sendContent);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (content);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return (content);
                }
            }
        }
        public async Task<InpowerResult> Put<T>(T obj, string url)
        {
            InpowerResult resp = new InpowerResult { Status = 0, Message = "", Response = null };
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);
                var json = JsonConvert.SerializeObject(obj);
                var sendContent = new StringContent(json, Encoding.UTF8, "application/json");
                string AccessToken = GlobalConstant.AccessToken;
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsync(client.BaseAddress + url, sendContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);

                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
            }

        }

        public async Task<InpowerResult> PutWithoutContent(string url)
        {
            InpowerResult resp = new InpowerResult { Status = 0, Message = "", Response = null };
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);
                string AccessToken = GlobalConstant.AccessToken;
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                }
                else
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PutAsync(client.BaseAddress + url, null);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);

                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
            }
        }

        public async Task<InpowerResult> Delete<T>(string url)
        {
            InpowerResult resp = new InpowerResult { Status = 0, Message = "", Response = null };
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                client.BaseAddress = new Uri(GlobalConstant.BaseUrl);
                if (!string.IsNullOrEmpty(GlobalConstant.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GlobalConstant.AccessToken);
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.DeleteAsync(client.BaseAddress + url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<InpowerResult>(content);
                }

            }

        }
    }
}


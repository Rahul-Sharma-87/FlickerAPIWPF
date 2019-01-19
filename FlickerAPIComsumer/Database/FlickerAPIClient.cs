using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FlickerAPIComsumer.Services
{
    public static class FlickerAPIClient {
        static List<Item> itemList = new List<Item>();
        private const string flickerUrl ="https://api.flickr.com/services/rest/?method=flickr.photos.getRecent&api_key=2f4cc4b4cd4f9a5dae36b1219c032ff1&format=json&nojsoncallback=1";
        public static IList<Item> GetResposeFromAPI(string imagesPerPage = "10", string page ="1") {
            HttpClient client = new HttpClient();
            string url = string.Format("{0}&per_page={1}&page={2}", flickerUrl, imagesPerPage, page);
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response =  client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode) {
                itemList.Clear();
                var result  = response.Content.ReadAsStringAsync().Result;
                JToken token = JToken.Parse(result);
                JArray photos = (JArray)token.SelectToken("photos.photo");
                foreach (var photo in photos) {
                    var item = new Item() {
                        ItemID = photo["id"].ToString(),
                        Path = "https://farm" + photo["farm"] + ".staticflickr.com/" + photo["server"] + '/' +
                               photo["id"] + '_' + photo["secret"] + ".jpg",
                        Title = photo["title"].ToString()
                    };
                    itemList.Add(item);
                }
            }
            return itemList;
        }

        public class Item
        {
            public string ItemID { get; internal set; }
            public string Path { get; internal set; }
            public string Title { get; internal set; }
        }
    }
}

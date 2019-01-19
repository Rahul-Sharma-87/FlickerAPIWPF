using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using FlickerAPIComsumer.Custom_Control;
using Newtonsoft.Json.Linq;

namespace FlickerAPIComsumer.Model {
    public class FlickerModel : IPaginationContract<FlickerImageItem> {

        private const string flickerUrl =
            "https://api.flickr.com/services/rest/?method=flickr.photos.getRecent&api_key=2f4cc4b4cd4f9a5dae36b1219c032ff1&format=json&nojsoncallback=1";

        public IList<FlickerImageItem> GetItemsFromDataSource(uint page, uint recordsPerPage, string searchString)
        {
            return GetResposeFromAPI(recordsPerPage.ToString(), page.ToString());
        }

        public int GetTotalRecords(string searchString)
        {
            return GetTotalSizeFromAPI();
        }
        
        private static IList<FlickerImageItem> GetResposeFromAPI(string imagesPerPage, string page) {
            var itemList = new List<FlickerImageItem>();
            var client = new HttpClient();
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
                    var item = new FlickerImageItem() {
                        ImageId = photo["id"].ToString(),
                        ImageUrl = "https://farm" + photo["farm"] + ".staticflickr.com/" + photo["server"] + '/' +
                                photo["id"] + '_' + photo["secret"] + ".jpg",
                        Description = photo["title"].ToString()
                    };
                    itemList.Add(item);
                }
            }
            return itemList;
        }

        private static int GetTotalSizeFromAPI() {
            var client = new HttpClient();
            client.BaseAddress = new Uri(flickerUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response =  client.GetAsync(flickerUrl).Result;
            if (response.IsSuccessStatusCode) {
                var result = response.Content.ReadAsStringAsync().Result;
                JToken token = JToken.Parse(result);
                JArray photos = (JArray) token.SelectToken("photos.photo");
                return photos.Count;
            }
            return 0;
        }

    }
}

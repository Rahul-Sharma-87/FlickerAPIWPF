using FlickerAPIComsumer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlickerAPIComsumer.Services;

namespace FlickerAPIComsumer.Database
{
    public class FlickerDatabase 
    {
        private const string apiKey = "2f4cc4b4cd4f9a5dae36b1219c032ff1";
        List<FlickerAPIClient.Item> itemList = new List<FlickerAPIClient.Item>();
        private static int pageSize = 10;
       
        #region IPageControlContract Members

        public uint GetTotalCount()
        {
            return 500;
        }

        public ICollection<object> GetRecordsBy(uint StartingIndex, uint NumberOfRecords, object FilterTag)
        {
            //Flicker Nuget package

            //FlickerAPIClient.GetResposeFromAPI();
            //itemList.Clear();
            //Flickr flickr = new Flickr(apiKey);
            //var options = new PhotoSearchOptions { Tags = "colorful", PerPage = pageSize, Page = Convert.ToInt32(StartingIndex)/pageSize };
            //PhotoCollection photos = flickr.PhotosSearch(options); 
            //foreach (Photo photo in photos) {
            //    itemList.Add(new Item { ItemID = photo.PhotoId, Path = photo.LargeUrl, Title = photo.Title });
            //}
            //return itemList.ToList<object>();

            //Using API version
            return FlickerAPIClient.GetResposeFromAPI().ToList<object>();
        }
        #endregion
    }
}

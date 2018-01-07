using ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ElasticSearchService<T> where T : class
    {
        ElasticSearch.IESManager<T> IeManager;
        public ElasticSearchService(IESManager<T> ieManager)
        {
            IeManager = ieManager;
        }

        public Tuple<bool,string> PostToElasticSearch(List<T> itemList)
        {
            var result = IeManager.CreateNewIndex();
            if (result.Item1)
            {
                IeManager.FillIndex(itemList);
            }
            return result;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IElasticSearchService<T> where T : class
    {
 
        Tuple<bool, string> PostToElasticSearch(List<T> itemList);

        Tuple<bool, List<T>, string> SearchFromElasticSearch(string[] textList, string field);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch
{
    public interface IESManager<T> where T : class
    {
        Tuple<bool, string> CreateNewIndex();

        void FillIndex(List<T> itemList);
        Tuple<bool,List<T>, string> Search(string[] textList, string field);
    }
}

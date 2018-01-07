using ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ElasticSearchService<T>: IElasticSearchService<T> where T : class
    {
        //ElasticSearch.IESManager<T> IeManager;
        IESManager<T> esManager;
        /// <summary>
        /// kullanılacak olan ESManager'i impelemente eder
        /// </summary>
        /// <param name="aliasName">elastic search index alias name</param>
        /// <param name="indexName">elastic search index name</param>
        public ElasticSearchService(string aliasName, string indexName)
        {
            esManager = new ESMananger<T>(aliasName, indexName);
        }

        /// <summary>
        /// elastic search manageri kullanarak indexleten fonksiyon, index oluşmamışsa önce index oluşturur
        /// </summary>
        /// <param name="itemList">indexlenecek olan item listesi</param>
        /// <returns></returns>
        public Tuple<bool,string> PostToElasticSearch(List<T> itemList)
        {
            var result = esManager.CreateNewIndex();
            if (result.Item1)
            {
                esManager.FillIndex(itemList);
            }
            return result;
        }

        /// <summary>
        /// elastic search manageri kullanarak search ettiren fonksiyon
        /// </summary>
        /// <param name="textList">aranacak kelime listesi</param>
        /// <param name="field">kelimelerin aranacağı alan</param>
        /// <returns></returns>
        public Tuple<bool,List<T>,string> SearchFromElasticSearch(string[] textList, string field)
        {

            var result = esManager.Search(textList, field);
            return result;
        }

    }
}

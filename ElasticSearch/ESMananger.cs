
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearch
{
    public class ESMananger<T>: IESManager<T> where T : class
    {
        private string AliasName { get; set; }
        private string IndexName { get; set; }
        private Uri node { get; set; }
        private ConnectionSettings settings { get; set; }
        protected ElasticClient client { get; set; }
        public ESMananger(string aliasName, string indexName)
        {
            IndexName = indexName;
            AliasName = aliasName;
            node = new Uri("http://localhost:9200/");
            settings = new ConnectionSettings(node);
            settings.DefaultIndex("defaultindex")
          .MapDefaultTypeIndices(m => m.Add(typeof(T), aliasName));
            client = new ElasticClient(settings);

        }
        /// <summary>
        /// elastic search indexi yoksa oluşturur
        /// </summary>
        /// <returns>elastic search apisinden hata alırsa false ile birlikte hatayı döner</returns>
        public Tuple<bool, string> CreateNewIndex()
        {
            var createIndexDescriptor = new CreateIndexDescriptor(IndexName)
          .Mappings(ms => ms
                          .Map<T>(m => m.AutoMap())
                   ).Aliases(a => a.Alias(AliasName));
  
            var request = new IndexExistsRequest(IndexName);
            var result = client.IndexExists(request);

            if (!result.Exists)
            {
                var response = client.CreateIndex(createIndexDescriptor);
                if (!response.IsValid)
                {
                    return new Tuple<bool, string>(false, response.ServerError.Error.ToString());
                }
            }

            return new Tuple<bool, string>(true, "");

        }
        /// <summary>
        /// elastic searche gelen parametreyi bulk olarak indexler
        /// </summary>
        /// <param name="itemList">indexlenecek bulk liste</param>
        public void FillIndex(List<T> itemList)
        {
            client.IndexMany<T>(itemList, AliasName);
        }

        /// <summary>
        /// girilen parametrelere göre elastic search üzerinde arama yapar
        /// </summary>
        /// <param name="textList">aranacak kelimeler</param>
        /// <param name="field">kelimelerin index içinde hangi alanda aranacağı bilgisi</param>
        /// <returns></returns>
        public Tuple<bool, List<T>, string> Search(string[] textList, string field)
        {

            var mustClauses = new List<QueryContainer>();

            foreach (var item in textList)
            {
                mustClauses.Add(new TermQuery
                {
                    Field = new Field(field),
                    Value = item.ToLower()
                });
            }

            var searchRequest = new SearchRequest<T>(AliasName)
            {
                Size = 30,
                From = 0,
                Query = new BoolQuery { Must = mustClauses }
            };

            var searchResponse = client.Search<T>(searchRequest);

            if (searchResponse.IsValid)
            {
                return new Tuple<bool, List<T>, string>(true, searchResponse.Documents.ToList(), "");
            }
            else
            {
                return new Tuple<bool, List<T>, string>(false, null, searchResponse.ServerError.Error.ToString());
            }
        }


    }
}

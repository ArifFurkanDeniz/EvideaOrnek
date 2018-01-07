
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
        private Uri node { get; set; }
        private ConnectionSettings settings { get; set; }
        private ElasticClient client { get; set; }
        public ESMananger()
        {
            node = new Uri("http://localhost:9200/");
            settings = new ConnectionSettings(node);
            settings.DefaultIndex("defaultindex")
          .MapDefaultTypeIndices(m => m.Add(typeof(T), "evidea6"));
            client = new ElasticClient(settings);

        }
        public Tuple<bool, string> CreateNewIndex()
        {
            var createIndexDescriptor = new CreateIndexDescriptor("evidea_urun6")
          .Mappings(ms => ms
                          .Map<T>(m => m.AutoMap())
                   ).Aliases(a => a.Alias("evidea6"));
  
            var request = new IndexExistsRequest("evidea_urun6");
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

        public void FillIndex(List<T> itemList)
        {
            //foreach (var item in itemList)
            //{
            //    client.Index<T>(item, idx => idx.Index("evidea5"));
            //}
            client.IndexMany<T>(itemList, "evidea6");
        }

    }
}

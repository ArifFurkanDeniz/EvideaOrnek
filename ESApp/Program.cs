
using ElasticSearch;
using Service;
using Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program Başladı");
            //ElasticSearch.ESMananger _esManager = new ESMananger();
            //_esManager.CreateNewIndex();
            //_esManager.FillIndex();

            List<string> urlList = new List<string>();

            urlList.Add("/duzen/c/103693");
            urlList.Add("/banyo/c/13");
            urlList.Add("/aydinlatma/c/3");

            var _getUrunlerAndPostElasticSearch = new List<Task>();
            try
            {
                foreach (var item in urlList)
                {
                    _getUrunlerAndPostElasticSearch.Add(GetUrunlerAndPostElasticSearch(item));
                }

                Task.WhenAll(_getUrunlerAndPostElasticSearch).Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Hata:{0}", ex.Message));
            }

            Console.ReadLine();
        }

        public static async Task GetUrunlerAndPostElasticSearch(string urlPath)
        {
            HtmlParserHelper _htmlParserManager = new HtmlParserHelper(urlPath);
            ElasticSearchService<Urun> _ElasticSearchService = new ElasticSearchService<Urun>(new ESMananger<Urun>());


            var urunler = await _htmlParserManager.GetUrunListesi();
           var result =  _ElasticSearchService.PostToElasticSearch(urunler);

            if (result.Item1)
            {
                Console.WriteLine(string.Format("{0} için Elastic Search indexlemesi başarılı", urlPath));
            }
            else
            {
                Console.WriteLine(string.Format("{0} için Elastic Search indexlemesi başarısız. Hata: {1}", urlPath, result.Item2));
            }

        }

      
    }
}



using Service;
using Service.Helper;
using Service.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESApp
{
    class Program
    {
        /// <summary>
        /// indexlemeyi başlatan program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Program Başladı");     

            var urls = new List<string>(ConfigurationManager.AppSettings["urls"].Split(';'));

            var _getUrunlerAndPostElasticSearch = new List<Task>();
            try
            {
                foreach (var item in urls)
                {
                    _getUrunlerAndPostElasticSearch.Add(GetUrunlerAndPostElasticSearch(item));
                }

                Task.WhenAll(_getUrunlerAndPostElasticSearch).Wait();

            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Hata:{0}", ex.Message));
            }

            Console.WriteLine("Program Bitti");
            Console.ReadLine();
        }

        /// <summary>
        /// ürün bilgilerini elde edip elastic searche gönderen fonksiyon
        /// </summary>
        /// <param name="urlPath">hangi site adresilerinin indexlemede kullanılacağı parametre</param>
        /// <returns></returns>
        public static async Task GetUrunlerAndPostElasticSearch(string urlPath)
        {
            IHtmlParserHelper _htmlParserManager = new HtmlParserHelper(ConfigurationManager.AppSettings["baseAddress"],urlPath);
            IElasticSearchService<Urun> _elasticSearchService = new ElasticSearchService<Urun>(ConfigurationManager.AppSettings["aliasName"], ConfigurationManager.AppSettings["indexName"]);

            var urunler = await _htmlParserManager.GetUrunListesi();
            var result = _elasticSearchService.PostToElasticSearch(urunler);

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

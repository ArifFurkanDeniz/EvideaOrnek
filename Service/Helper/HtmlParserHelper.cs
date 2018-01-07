using HtmlAgilityPack;
using Service.Helper;
using Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class HtmlParserHelper : IHtmlParserHelper
    {
        HttpClient client;
        string Url;

        /// <summary>
        /// Htm parserın kullanacak olduğu apiyi impelemente eder
        /// </summary>
        /// <param name="baseAddress">gidielecek domain adresi</param>
        /// <param name="url">gidilecek link adresi</param>
        public HtmlParserHelper(string baseAddress,string url)
        {
            Url = url;
            client = new HttpClient();
            // client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Trepas_WebAboneNetAPI"]);
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
 
        /// <summary>
        /// adrese gidip ürün sayfalarını asenkron olarak çalıştırıp ürün bilgilerini alan fonksiyon
        /// </summary>
        /// <returns>alınan ürünlerin ham verilerini döner</returns>
        public async Task<List<Urun>> GetUrunListesi()
        {
            var urunLinkleri = await GetUrunLinkleri();
            List<Urun> urunListesi = new List<Urun>();

            var _getGtmlFromSite = new List<Task<string>>();
            foreach (var item in urunLinkleri)
            {
                _getGtmlFromSite.Add(GetHtmlFromSite(item));  
            }


            foreach (var html in  await Task.WhenAll(_getGtmlFromSite))
            {

                HtmlDocument htmldoc = new HtmlDocument();
                htmldoc.LoadHtml(html);

                HtmlNode urunTitle = htmldoc.DocumentNode.SelectSingleNode("//title");
                HtmlNode urunDescription = htmldoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
                HtmlNode urunAdi = htmldoc.DocumentNode.SelectSingleNode("//h1[@class='urundetay_231']");
                HtmlNode urunFiyati = htmldoc.DocumentNode.SelectSingleNode("//span[@class='current-price']");

                string _title = urunTitle.InnerText;
                string _description = urunDescription.Attributes["content"].Value;
                string _urunAdi = urunAdi.InnerText;
                string _urunFiyati = urunFiyati.InnerText;

                urunListesi.Add(new Urun
                {
                    Title = _title,
                    Description = _description,
                    UrunAdi = _urunAdi,
                    UrunFiyati = _urunFiyati
                });
            }

                return urunListesi;
        }

        /// <summary>
        /// adres üzerinden her ürün sayfasının linklerini parse eden fonksiyon
        /// </summary>
        /// <returns>ürün linklerini döner</returns>
        private async Task<List<string>> GetUrunLinkleri()
        {
            string html = await GetHtmlFromSite(Url);
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(html);

            HtmlNodeCollection urunDivs = htmldoc.DocumentNode.SelectNodes("//div[@class='urunlist2']");

            List<string> urunLinkleri = new List<string>();
            foreach (HtmlAgilityPack.HtmlNode item in urunDivs)
            {
                HtmlNode h2Node = item.SelectSingleNode("./div[@class='urunlist2_1']");
                HtmlNode h3Node = h2Node.SelectSingleNode("./div[@class='urunlist2_11']");
                HtmlNode h4Node = h3Node.SelectSingleNode("./a");

                string urunLinki = h4Node.Attributes.Where(x => x.Name == "href").Select(x => x.Value).FirstOrDefault();

                urunLinkleri.Add(urunLinki);
            }

            return urunLinkleri; 
        }
        /// <summary>
        /// sayfanın html bilgisini metin şeklinde alan fonksiyon
        /// </summary>
        /// <param name="url">gidilecek sayfa adresi</param>
        /// <returns>sayfa linklerini döner</returns>
        private async Task<string> GetHtmlFromSite(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                var htmlString = await response.Content.ReadAsStringAsync();
                return htmlString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
    }
}

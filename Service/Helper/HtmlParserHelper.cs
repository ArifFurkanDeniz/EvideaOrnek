using HtmlAgilityPack;
using Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class HtmlParserHelper
    {
        HttpClient client;
        string Url;

        public HtmlParserHelper(string url)
        {
            Url = url;
            client = new HttpClient();
            // client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Trepas_WebAboneNetAPI"]);
            client.BaseAddress = new Uri("http://www.evidea.com");
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
 
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
                var log = log4net.LogManager.GetLogger(typeof(HtmlParserHelper));
               log.Error(string.Format("{0}-{1}-{2}", ex.Message, ex.InnerException.Message, ex.StackTrace));
                throw ex;
            }
          
        }
    }
}

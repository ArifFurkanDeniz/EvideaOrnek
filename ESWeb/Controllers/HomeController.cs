using ESWeb.Models.Home;
using Service;
using Service.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        /// <summary>
        /// aramanın yapılacağı sayfa
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new IndexViewModel();
            return View(model);
        }

        /// <summary>
        /// arama sonuçlarını dönen sayfa
        /// </summary>
        /// <param name="inModel">post edilen sayfanın modeli</param>
        /// <returns></returns>
        public ActionResult AramaSonuclari(IndexViewModel inModel)
        {
            IElasticSearchService<Urun> _elasticSearchService = new ElasticSearchService<Urun>(ConfigurationManager.AppSettings["aliasName"], ConfigurationManager.AppSettings["indexName"]);
            var outModel = new AramaSonuclariViewModel();

            if (!string.IsNullOrEmpty(inModel.Text))
            {
                var textList = inModel.Text.Split(' ');

                var result = _elasticSearchService.SearchFromElasticSearch(textList, "urunAdi");
                if (result.Item1)
                {
                    outModel.UrunListesi = result.Item2;
                }
                else
                {
                    outModel.HataMesaji = result.Item3;
                }
              
            }
           
            return View(outModel);
        }
    }
}
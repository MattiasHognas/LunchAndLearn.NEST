using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NEST.UI.Web.Models;
using Nest;

namespace NEST.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        public const string IndexName = "lunch_and_learn";
        public static Uri ServerUri = new Uri("http://localhost:9200");

        private static ElasticClient ElasticClient
        {
            get
            {
                var settings = new ConnectionSettings(ServerUri);
                settings.SetDefaultIndex(IndexName);
                return new ElasticClient(settings);
            }
        }

        public ActionResult CreateIndex()
        {
            ConnectionStatus connectionStatus;
            if (ElasticClient.TryConnect(out connectionStatus))
            {
                if (ElasticClient.IndexExists(IndexName).Exists)
                    ElasticClient.DeleteIndex(IndexName);
                var indexSettings = new IndexSettings();
                //indexSettings.Analysis.Analyzers.Add("analyzer", new LanguageAnalyzer(Language.Swedish));
                //indexSettings.Analysis.Tokenizers.Add("tokenizer", new EdgeNGramTokenizer());
                ElasticClient.CreateIndex(IndexName, indexSettings);
                var products = new List<Product>
                    {
                        new Product {Id = 1, Name = "Alfa", Content = "lorem ipsum doremi fasoleti"},
                        new Product {Id = 2, Name = "Beta", Content = "one ring to rule them all"},
                        new Product {Id = 3, Name = "Gamma", Content = "Lunch & Learn är sjukt spännande och roligt"}
                    };
                ElasticClient.IndexMany(products, IndexName);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Index(SearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                ConnectionStatus connectionStatus;
                if (ElasticClient.TryConnect(out connectionStatus))
                {
                    var result = ElasticClient
                        .Search<Product>(s => s
                            .Explain(true)
                            //.MatchAll()
                            //.Query(q => q
                            //    .Fuzzy(fz => fz.OnField(f => f.Content).MinSimilarity(0.01)))
                            //.Query(q => q.FuzzyLikeThis(flt => flt.Analyzer("analyze").OnFields(f => f.Name, f => f.Content)))
                            .QueryString(model.Search));
                    model.Documents = result.Documents;
                    model.Highlights = result.Highlights;
                    model.MaxScore = result.MaxScore;
                    var a = result.DocumentsWithMetaData;
                    model.Total = result.Total;
                }
            }
            return View(model);
        }
    }
}

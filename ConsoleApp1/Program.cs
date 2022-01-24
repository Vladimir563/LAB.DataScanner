using System;
using System.Collections.Generic;
using System.Threading;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace ConsoleApp1
{
    internal class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();
            service.IgnoreSslErrors = true;
            service.LoadImages = false;
            service.HideCommandPromptWindow = true;

            var url = @"https://www.epam.com/careers/job-listings?query=2&country=Russia";

            PhantomJSDriver driver = new PhantomJSDriver(service);

            driver.Navigate().GoToUrl(url);

            var y = "var i = 1; function myLoop(){setTimeout(function() {window.scrollTo({ top: document.body.scrollHeight / 10 * i, behavior: \"smooth\"}); i++; if (i < 10){myLoop();}}, 1000)}myLoop();";

            driver.ExecuteScript(y);

            //for (int i = 1; i <= 10; i++)
            //{
            //    //string jsCode = "window.scrollTo({top: document.body.scrollHeight / 10 * " + i + ", behavior: \"smooth\"});";
            //    string jsCode = "$('html, body').animate({scrollTop:100 * " + i + "},'10');";
            //    driver.ExecuteScript(jsCode);
            //    //Running js code using the IJavaScript Executor interface
            //    //IJavaScriptExecutor js = driver;
            //    //js.ExecuteScript(jsCode);
            //    //Pause rolling
            //    //Thread.Sleep(1000);
            //}

            string html = driver.PageSource;//Page Html

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(html);//Resolving Html strings

            string imgPath = "//*[@class='search-result__item']";

            IEnumerable<HtmlNode> nodes = doc.DocumentNode.SelectNodes(imgPath);

            foreach (var item in nodes)
            {
                Console.WriteLine(item.OuterHtml + "\n\n");
            }

            //Console.WriteLine("end of scripts");

            ////var node = driver.FindElements(By.XPath("//*[@class='search-result__item']"));

            //var node = driver.PageSource;

            //Console.WriteLine(node);

            ////foreach (var item in node)
            ////{
            ////    Console.WriteLine(item);
            ////}

            //Console.WriteLine("End");

            //driver.Quit();

            //Console.ReadKey();
        }
    }
}

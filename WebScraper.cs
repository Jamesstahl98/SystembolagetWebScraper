using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Windows;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using System.Security.Policy;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SystembolagetWebScraper.Model;
using System.Collections.ObjectModel;

namespace SystembolagetWebScraper
{
    internal class WebScraper
    {
        private static string productEnclosingClassName = "css-145u7id e1qhsejf0";
        private static string productNameClassName = "css-1i86311 e1iq8b8k0";
        private static string productPriceClassName = "css-1k0oafj eqfj59s0";
        private static string productCountryVolumeAlcoholClassName = "css-e42h23 e1g7jmpl0";
        private string _url = "https://www.systembolaget.se/sortiment/?p=";
        private HttpClient httpClient = new HttpClient();

        public string Url => _url;
        public static ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        public WebScraper()
        {
            Task.Run(async () => await ScrapeProductsAsync(Url));
        }

        static async Task ScrapeProductsAsync(string url)
        {
            await Task.Run(() =>
            {
                try
                {
                    var options = new ChromeOptions();

                    using (var driver = new ChromeDriver(options))
                    {
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                        driver.Navigate().GoToUrl(url + "1");
                        IWebElement ageConfirmationButton = wait.Until(d => d.FindElement(By.XPath("//a[contains(text(), 'fyllt')]")));
                        ageConfirmationButton.Click();

                        IWebElement acceptCookiesButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'acceptera')]")));
                        acceptCookiesButton.Click();

                        while (true)
                        {
                            for (int i = 1; i < 30; i++)
                            {
                                driver.Navigate().GoToUrl(url + i.ToString());
                                wait.Until(d => d.FindElements(By.XPath($"//a[@class='{productEnclosingClassName}']")).Count > 0);

                                var productElements = driver.FindElements(By.XPath($"//a[@class='{productEnclosingClassName}']"));

                                foreach (var element in productElements)
                                {
                                    var countryVolumeAlcoholElements = element.FindElements(By.XPath($".//p[@class='{productCountryVolumeAlcoholClassName}']"));

                                    var product = new Product(
                                        element.FindElement(By.XPath($".//p[@class='{productNameClassName}']")).Text,
                                        Single.Parse(element.FindElement(By.XPath($".//p[@class='{productPriceClassName}']")).Text.Split(':')[0]),
                                        countryVolumeAlcoholElements[0].Text,
                                        GetVolume(countryVolumeAlcoholElements[1].Text),
                                        Single.Parse(countryVolumeAlcoholElements[2].Text.Split(' ')[0].Replace(',', '.'))
                                    );

                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        Products.Add(product);
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            });
        }
        public static string RemoveWhitespace(string source)
        {
            return new string(source.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static int GetVolume(string volumeString)
        {
            if (volumeString.Contains("flaskor"))
            {
                var splitString = volumeString.Split(' ');
                var splitVolume = splitString[3].Split('m');
                return Int32.Parse(splitString[0]) * Int32.Parse(splitVolume[0]);
            }
            else
            {
                return Int32.Parse(RemoveWhitespace(volumeString.Split('m')[0]));
            }
        }
    }
}

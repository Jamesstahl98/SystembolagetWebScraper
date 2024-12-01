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
using SeleniumExtras.WaitHelpers;
using SystembolagetWebScraper.Model;
using System.Collections.ObjectModel;
using System.Collections;

namespace SystembolagetWebScraper
{
    enum ProductType { Beer, Red_Wine, White_Wine, Whiskey, SparklingWine, Cider, Vodka, Liqueur, Other }
    internal class WebScraper
    {
        private static string productEnclosingClassName = "css-145u7id e1qhsejf0";
        private static string brandNameClassName = "css-1njx6qf e1iq8b8k0";
        private static string productNameClassName = "css-1hdv0wt e1iq8b8k0";
        private static string productTypeClassName = "css-4oiqd8 eqfj59s0";
        private static string productPriceClassName = "css-a2frwy eqfj59s0";
        private static string productCountryVolumeAlcoholClassName = "css-rp7p3f e1g7jmpl0";
        private static string imageSourceClassName = "css-g98gbd e1ydxtsp0";
        private static string productNumberClassName = "css-su700l e1iq8b8k0";
        private string _url = "https://www.systembolaget.se/sortiment/?p=";
        private HttpClient httpClient = new HttpClient();

        public string Url => _url;
        public static ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public static HashSet<string> productNumbers = new HashSet<string>();

        public async Task InitializeAsync()
        {
            await ScrapeProductsAsync(Url);
        }

        static async Task ScrapeProductsAsync(string url)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var options = new ChromeOptions();

                    using (var driver = new ChromeDriver(options))
                    {
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                        driver.Navigate().GoToUrl(url + "1");
                        IWebElement ageConfirmationButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//a[contains(text(), 'fyllt')]")
                        ));
                        ageConfirmationButton.Click();

                        IWebElement acceptCookiesButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(), 'acceptera')]")));
                        acceptCookiesButton.Click();

                        for (int i = 1; i < 856; i++)
                        {
                            driver.Navigate().GoToUrl(url + i.ToString());
                            wait.Until(d => d.FindElements(By.XPath($"//a[@class='{productEnclosingClassName}']")).Count > 0);

                            var productElements = driver.FindElements(By.XPath($"//a[@class='{productEnclosingClassName}']"));

                            foreach (var element in productElements)
                            {
                                string productNumber = element.FindElement(By.XPath($".//p[@class='{productNumberClassName}']")).Text;

                                if(productNumbers.Contains(productNumber))
                                {
                                    continue;
                                }

                                productNumbers.Add(productNumber);

                                var countryVolumeAlcoholElements = element.FindElements(By.XPath($".//p[@class='{productCountryVolumeAlcoholClassName}']"));

                                string productTypeString = element.FindElement(By.XPath($".//p[@class='{productTypeClassName}']")).Text;

                                var product = new Product(
                                    element.FindElement(By.XPath($".//p[@class='{brandNameClassName}']")).Text,
                                    await GetProductNameAsync(element),
                                    productTypeString,
                                    await GetProductTypeAsync(productTypeString),
                                    await GetPriceAsync(element.FindElement(By.XPath($".//p[@class='{productPriceClassName}']")).Text),
                                    countryVolumeAlcoholElements[0].Text,
                                    await GetVolume(countryVolumeAlcoholElements[1].Text),
                                    Single.Parse(countryVolumeAlcoholElements[2].Text.Split(' ')[0]),
                                    await GetImageSourceAsync(element.FindElement(By.XPath($".//img[@class='{imageSourceClassName}']"))),
                                    element.GetAttribute("href")
                                );

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Products.Add(product);
                                });
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

        static async Task<ProductType> GetProductTypeAsync(string productTypeString)
        {
            return await Task.Run(() =>
            {
                if (productTypeString.Contains("ÖL"))
                {
                    return ProductType.Beer;
                }
                else if (productTypeString.Contains("VITT VIN"))
                {
                    return ProductType.White_Wine;
                }
                else if (productTypeString.Contains("RÖTT VIN"))
                {
                    return ProductType.Red_Wine;
                }
                else if (productTypeString.Contains("CIDER"))
                {
                    return ProductType.Cider;
                }
                else if (productTypeString.Contains("WHISKY"))
                {
                    return ProductType.Whiskey;
                }
                else if (productTypeString.Contains("MOUSSERANDE"))
                {
                    return ProductType.SparklingWine;
                }
                else if (productTypeString.Contains("LIKÖR"))
                {
                    return ProductType.Liqueur;
                }
                else if (productTypeString.Contains("VODKA"))
                {
                    return ProductType.Vodka;
                }
                return ProductType.Other;
            });
        }
        static async Task<string?> GetProductNameAsync(IWebElement productElement)
        {
            var bla = await Task.Run(() => productElement.FindElements(By.XPath($".//p[@class='{productNameClassName}']")));
            if (bla.Count > 0)
            {
                return bla[0].Text;
            }
            return null;
        }

        static async Task<string?> GetImageSourceAsync(IWebElement imageElement)
        {
            try
            {
                var srcSet = await Task.Run(() => imageElement.GetAttribute("srcset"));
                if (srcSet[0] == '/')
                {
                    return null;
                }
                return srcSet.Split(' ')[0];
            }
            catch
            {
                return null;
            }
        }

        static async Task<string> RemoveWhitespaceAsync(string source)
        {
            return await Task.Run(() => new string(source.Where(c => !char.IsWhiteSpace(c)).ToArray()));
        }

        static async Task<int> GetVolume(string volumeString)
        {
            if (volumeString.Contains("flaskor")
                || volumeString.Contains("burkar")
                || volumeString.Contains("påsar"))
            {
                var splitString = volumeString.Split(' ');
                var splitVolume = splitString[3].Split('m');
                return await Task.Run(() => Int32.Parse(splitString[0]) * Int32.Parse(splitVolume[0]));
            }
            else
            {
                return Int32.Parse(await RemoveWhitespaceAsync(volumeString.Split('m')[0]));
            }
        }

        static async Task<float> GetPriceAsync(string priceString)
        {
            if (priceString.Contains(':'))
            {
                return Single.Parse(await RemoveWhitespaceAsync(priceString.Split(':')[0]));
            }
            else
            {
                return Single.Parse(await RemoveWhitespaceAsync(priceString.Split('*')[0]));
            }
        }
    }
}

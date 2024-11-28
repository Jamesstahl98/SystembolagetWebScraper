﻿using System;
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
        private static string imageSource = "css-g98gbd e1ydxtsp0";
        private static ProductType ProductType;
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
                                var countryVolumeAlcoholElements = element.FindElements(By.XPath($".//p[@class='{productCountryVolumeAlcoholClassName}']"));

                                string productTypeString = element.FindElement(By.XPath($".//p[@class='{productTypeClassName}']")).Text;

                                var product = new Product(
                                    element.FindElement(By.XPath($".//p[@class='{brandNameClassName}']")).Text,
                                    GetProductName(element),
                                    productTypeString,
                                    GetProductType(productTypeString),
                                    GetPrice(element.FindElement(By.XPath($".//p[@class='{productPriceClassName}']")).Text),
                                    countryVolumeAlcoholElements[0].Text,
                                    GetVolume(countryVolumeAlcoholElements[1].Text),
                                    Single.Parse(countryVolumeAlcoholElements[2].Text.Split(' ')[0]),
                                    GetImageSource(element.FindElement(By.XPath($".//img[@class='{imageSource}']"))),
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

        public static ProductType GetProductType(string productTypeString)
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

        }
        public static string? GetProductName(IWebElement productElement)
        {
            try
            {
                return productElement.FindElement(By.XPath($".//p[@class='{productNameClassName}']")).Text;
            }
            catch
            {
                return null;
            }
        }

        public static string? GetImageSource(IWebElement imageElement)
        {
            if (imageElement.GetAttribute("srcset")[0] == '/')
            {
                return null;
            }
            return imageElement.GetAttribute("srcset").Split(' ')[0];
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

        public static float GetPrice(string priceString)
        {
            if (priceString.Contains(':'))
            {
                return Single.Parse(RemoveWhitespace(priceString.Split(':')[0]));
            }
            else
            {
                return Single.Parse(RemoveWhitespace(priceString.Split('*')[0]));
            }
        }
    }
}

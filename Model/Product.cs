using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SystembolagetWebScraper.Model
{
    internal class Product
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public string Country { get; set; }
        public int Volume { get; set; }
        public float Alcohol { get; set; }
        public float APK { get => (Alcohol/100)*Volume/Price; }
        public string ImageSource { get; set; }

        public Product(string name, float price, string country, int volume, float alcohol, string imageSource)
        {
            Name = name;
            Price = price;
            Country = country;
            Volume = volume;
            Alcohol = alcohol;
            ImageSource = imageSource;
        }
    }
}

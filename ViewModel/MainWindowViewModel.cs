using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemBolagetWebScraper.Model;

namespace SystemBolagetWebScraper.ViewModel
{
    internal class MainWindowViewModel
    {
        private WebScraper webScraper;
        public ObservableCollection<Product> Products { get => WebScraper.Products; }
        public MainWindowViewModel()
        {
            webScraper = new WebScraper();
        }
    }
}

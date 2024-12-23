using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SystembolagetWebScraper.Command;
using SystembolagetWebScraper.Model;
using static OpenQA.Selenium.BiDi.Modules.Session.ProxyConfiguration;

namespace SystembolagetWebScraper.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private WebScraper webScraper;
        private ICollectionView _productsView;
        private Product? _activeProduct;

        public ProductType ProductTypes { get; set; }
        public ObservableCollection<Product> Products { get => WebScraper.Products; }
        public ObservableCollection<string> UniqueCountries { get; set; } = new ObservableCollection<string>();
        public ICollectionView ProductsView
        {
            get => _productsView;
            set
            {
                _productsView = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ApplyCountryFilterCommand { get; }
        public DelegateCommand ApplyProductTypeFilterCommand { get; }
        public DelegateCommand StartWebScrapeCommand { get; }

        public Product? ActiveProduct
        {
            get => _activeProduct;
            set
            {
                _activeProduct = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            webScraper = new WebScraper();
            ProductsView = CollectionViewSource.GetDefaultView(Products);

            Products.CollectionChanged += OnProductsCollectionChanged;

            UniqueCountries.Add("All");
            ApplyCountryFilterCommand = new DelegateCommand(ApplyCountryFilter);
            ApplyProductTypeFilterCommand = new DelegateCommand(ApplyProductTypeFilter);

            StartWebScrapeCommand = new DelegateCommand(StartWebScrape);
        }

        private async void StartWebScrape(object obj)
        {
            await webScraper.InitializeAsync();
        }

        private void OnProductsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Product newProduct in e.NewItems)
                {
                    if (!UniqueCountries.Contains(newProduct.Country))
                        UniqueCountries.Add(newProduct.Country);
                }
            }
        }
        private void ApplyCountryFilter(object obj)
        {
            string selectedCountry = obj as string;

            ProductsView.Filter = item =>
            {
                if (item is Product product)
                {
                    if (selectedCountry == "All")
                        return true;

                    return product.Country == selectedCountry;
                }
                return false;
            };

            ProductsView.Refresh();
        }

        private void ApplyProductTypeFilter(object obj)
        {
            ProductType selectedProductType = (ProductType)obj;

            ProductsView.Filter = item =>
            {
                if (item is Product product)
                {
                    //if (selectedProductType == "All")
                    //    return true;

                    return product.ProductType == selectedProductType;
                }
                return false;
            };

            ProductsView.Refresh();
        }
    }
}
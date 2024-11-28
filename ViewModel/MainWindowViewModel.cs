using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        private string _selectedSortOption;
        private ICollectionView _productsView;
        private Product? _activeProduct;

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

        public ObservableCollection<string> SortOptions { get; } = new ObservableCollection<string>
        {
            "Brand Name (Ascending)",
            "Brand Name (Descending)",
            "Country (Ascending)",
            "Country (Descending)",
            "Volume (Ascending)",
            "Volume (Descending)",
            "Alcohol (Ascending)",
            "Alcohol (Descending)",
            "Price (Ascending)",
            "Price (Descending)",
            "APK (Ascending)",
            "APK (Descending)"
        };
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    RaisePropertyChanged();
                    ApplySort();
                }
            }
        }

        public DelegateCommand ApplyCountryFilterCommand { get; }

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

            SelectedSortOption = SortOptions[0];

            UniqueCountries.Add("All");
            ApplyCountryFilterCommand = new DelegateCommand(ApplyCountryFilter);
        }

        private void ApplySort()
        {
            if (ProductsView == null) return;

            ProductsView.SortDescriptions.Clear();

            switch (SelectedSortOption)
            {
                case "Brand Name (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("BrandName", ListSortDirection.Ascending));
                    break;
                case "Brand Name (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("BrandName", ListSortDirection.Descending));
                    break;
                case "Country (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Country", ListSortDirection.Ascending));
                    break;
                case "Country (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Country", ListSortDirection.Descending));
                    break;
                case "Volume (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Volume", ListSortDirection.Ascending));
                    break;
                case "Volume (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Volume", ListSortDirection.Descending));
                    break;
                case "Alcohol (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Alcohol", ListSortDirection.Ascending));
                    break;
                case "Alcohol (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Alcohol", ListSortDirection.Descending));
                    break;
                case "Price (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
                    break;
                case "Price (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
                    break;
                case "APK (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("APK", ListSortDirection.Ascending));
                    break;
                case "APK (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("APK", ListSortDirection.Descending));
                    break;
            }
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
    }
}
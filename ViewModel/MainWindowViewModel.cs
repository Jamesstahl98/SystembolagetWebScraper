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
        private HashSet<string> _uniqueCountries = new HashSet<string>();
        private string _selectedFilterOption;
        private ICollectionView _productsView;
        private Product? _activeProduct;

        private ObservableCollection<string> _filterOptions;
        public ObservableCollection<string> FilterOptions
        {
            get => _filterOptions;
            set
            {
                _filterOptions = value;
                RaisePropertyChanged();
            }
        }

        public string SelectedFilterOption
        {
            get => _selectedFilterOption;
            set
            {
                if (_selectedFilterOption != value)
                {
                    _selectedFilterOption = value;
                    RaisePropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public ObservableCollection<Product> Products { get => WebScraper.Products; }

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

            PopulateFilterOptions();

            Products.CollectionChanged += OnProductsCollectionChanged;

            SelectedSortOption = SortOptions[0];
            SelectedFilterOption = FilterOptions[0];
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
        private void ApplyFilter()
        {
            ProductsView.Filter = item =>
            {
                if (item is Product product)
                {
                    if (SelectedFilterOption == "All")
                        return true;
                    return product.Country == SelectedFilterOption;
                }
                return false;
            };
        }

        private void OnProductsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Product newProduct in e.NewItems)
                    _uniqueCountries.Add(newProduct.Country);
            }

            if (e.OldItems != null)
            {
                foreach (Product oldProduct in e.OldItems)
                {
                    if (Products.All(p => p.Country != oldProduct.Country))
                        _uniqueCountries.Remove(oldProduct.Country);
                }
            }

            PopulateFilterOptions();
        }

        private void PopulateFilterOptions()
        {
            var options = new List<string> { "All" };
            options.AddRange(_uniqueCountries.OrderBy(c => c));

            FilterOptions = new ObservableCollection<string>(options);
        }
    }
}

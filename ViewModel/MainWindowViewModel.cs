using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SystembolagetWebScraper.Model;

namespace SystembolagetWebScraper.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private WebScraper webScraper;

        public ObservableCollection<Product> Products { get => WebScraper.Products; }

        private ICollectionView _productsView;
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
        "Name (Ascending)",
        "Name (Descending)",
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

        private string _selectedSortOption;
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

        public MainWindowViewModel()
        {
            webScraper = new WebScraper();
            ProductsView = CollectionViewSource.GetDefaultView(Products);
            SelectedSortOption = SortOptions[0];
        }

        private void ApplySort()
        {
            if (ProductsView == null) return;

            ProductsView.SortDescriptions.Clear();

            switch (SelectedSortOption)
            {
                case "Name (Ascending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;
                case "Name (Descending)":
                    ProductsView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
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
    }
}

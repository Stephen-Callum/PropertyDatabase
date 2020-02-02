using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PropertyDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<Listing> listings = new List<Listing>();
        static Listing listing = new Listing();
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        void Initialize()
        {
            // Clean code wrapper
            using (var db = new PropertyDatabaseEntities())
            {
                listings = db.Listings.ToList();
            }
            // Set text boxes to read only
            TextBoxListingID.IsReadOnly = true;
            TextBoxListingAddress.IsReadOnly = true;
            TextBoxListingPrice.IsReadOnly = true;
            TextBoxBedrooms.IsReadOnly = true;

            // Edit button starts as disabled
            btnEditListing.IsEnabled = false;
        }

        private void btnShowListings_Click(object sender, RoutedEventArgs e)
        {
            ListViewPropertyListings.ItemsSource = listings;
        }

        private void ListViewPropertyListings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewPropertyListings.SelectedItem != null)
            {
                listing = (Listing)ListViewPropertyListings.SelectedItem;
                TextBoxListingID.Text = listing.ListingID.ToString();
                TextBoxListingAddress.Text = listing.ListingAddress;
                TextBoxListingPrice.Text = listing.ListingPrice.ToString();
                TextBoxBedrooms.Text = listing.NumBedrooms.ToString();
            }

            TextBoxListingAddress.IsReadOnly = true;
            TextBoxListingPrice.IsReadOnly = true;
            TextBoxBedrooms.IsReadOnly = true;

            btnEditListing.IsEnabled = true;
        }

        private void btnAddListing_Click(object sender, RoutedEventArgs e)
        {
            if (btnAddListing.Content.ToString() == "Add Listing")
            {
                // text boxes become editable
                TextBoxListingAddress.IsReadOnly = false;
                TextBoxListingPrice.IsReadOnly = false;
                TextBoxBedrooms.IsReadOnly = false;

                // clear all text boxes
                TextBoxListingID.Text = "";
                TextBoxListingAddress.Text = "";
                TextBoxListingPrice.Text = "";

                btnAddListing.Content = "Save";
            }
            else
            {
                if (TextBoxListingAddress.Text.Length > 0)
                {
                    Int32.TryParse(TextBoxListingPrice.Text, out int price);
                    Int32.TryParse(TextBoxBedrooms.Text, out int numBeds);
                    var listingToAdd = new Listing()
                    {
                        ListingAddress = TextBoxListingAddress.Text,
                        ListingPrice = price,
                        NumBedrooms = numBeds
                    };

                    // get database
                    using (var db = new PropertyDatabaseEntities())
                    {
                        db.Listings.Add(listingToAdd);
                        db.SaveChanges();

                        // update list, set to null first
                        ListViewPropertyListings.ItemsSource = null;
                        // re-read listings fresh from database
                        listings = db.Listings.ToList();
                        ListViewPropertyListings.ItemsSource = listings;
                    }
                }
                btnAddListing.Content = "Add Listing";
            }
        }

        private void btnEditListing_Click(object sender, RoutedEventArgs e)
        {
            if (listing != null)
            {
                if (btnEditListing.Content.ToString() == "Edit Listing")
                {
                    var shader = new SolidColorBrush(Color.FromRgb(207, 255, 210));
                    TextBoxBedrooms.Background = shader;
                    TextBoxListingAddress.Background = shader;
                    TextBoxListingPrice.Background = shader;

                    // make boxed editable
                    TextBoxListingPrice.IsReadOnly = false;
                    TextBoxListingAddress.IsReadOnly = false;
                    TextBoxBedrooms.IsReadOnly = false;
                    btnEditListing.Content = "Save";
                }
            }
            else
            {
                if (TextBoxListingAddress.Text.Length > 0)
                {
                    Int32.TryParse(TextBoxListingPrice.Text.ToString(), out int price);
                    Int32.TryParse(TextBoxBedrooms.Text.ToString(), out int numBeds);
                    using (var db = new PropertyDatabaseEntities())
                    {
                        var listingToUpdate = db.Listings.Find(listing.ListingID);
                        listingToUpdate.ListingAddress = TextBoxListingAddress.Text;
                        listingToUpdate.ListingPrice = price;
                        listingToUpdate.NumBedrooms = numBeds;

                        // update database
                        db.SaveChanges();

                        ListViewPropertyListings = null;
                        listings = db.Listings.ToList();
                        ListViewPropertyListings.ItemsSource = listings;
                    }
                }
                // put back to default
                TextBoxBedrooms.Background = Brushes.White;
                TextBoxListingAddress.Background = Brushes.White;
                TextBoxListingPrice.Background = Brushes.White;

                TextBoxListingPrice.IsReadOnly = true;
                TextBoxListingAddress.IsReadOnly = true;
                TextBoxBedrooms.IsReadOnly = true;
                btnEditListing.Content = "Edit Listing";
            }
        }

        private void ListViewPropertyListings_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // check if listing selected
            if (listing != null)
            {
                using (var db = new PropertyDatabaseEntities())
                {
                    // find listing in database which has the same ID as selected listing
                    var listingToDelete = db.Listings.Find(listing.ListingID);
                    var result = MessageBox.Show($"Delete listing {listing.ListingID}? Are you sure?",
                        "Greener Pastures", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        db.Listings.Remove(listingToDelete);
                        db.SaveChanges();
                        ListViewPropertyListings.ItemsSource = null;
                        listings = db.Listings.ToList();
                        ListViewPropertyListings.ItemsSource = listings;
                    }
                }
            }
        }
    }
}

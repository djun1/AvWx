using AvWx.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace AvWx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private static String ChosenProductName;
        private static String ChosenRegionName;
        private static String ChosenStationOrTypeName;
        private static String ChosenDirName;
        private static String ChosenURL;
        private static String ChosenSatelliteType;
        //private static string ChosenCityName;
        //private static string ChosenSatelliteType;
        private static XMLParserClass ProductXML = new XMLParserClass("Products.xml");
        //private static XMLParserClass CityCodeXML = new XMLParserClass("CityCodes.xml");

        public OptionsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            switch (GenericCodeClass.FileDownloadPeriod)
            {
                case 1:
                    DurationRadioButton3.IsChecked = true;
                    break;
                case 3:
                    DurationRadioButton1.IsChecked = true;
                    break;
                case 6:
                    DurationRadioButton2.IsChecked = true;
                    break;
            }

            switch (GenericCodeClass.LoopInterval.Milliseconds)
            {
                case 500:
                    LoopTimerRadioButton2.IsChecked = true;
                    break;
                case 100:
                    LoopTimerRadioButton1.IsChecked = true;
                    break;
                case 1000:
                    LoopTimerRadioButton3.IsChecked = true;
                    break;
            }

            //switch (GenericCodeClass.ChosenRegionString)
            //{
            //    case "ir4":
            //    case "alir":
            //    case "1070":
            //    case "03":
            //        ProductRadioButton1.IsChecked = true;
            //        break;
            //    case "rb":
            //    case "avn":
            //        ProductRadioButton2.IsChecked = true;
            //        break;
            //    case "rgb":
            //        ProductRadioButton3.IsChecked = true;
            //        break;
            //    case "vis":
            //    case "alvs":
            //    case "visible":
            //    case "nir":
            //        ProductRadioButton4.IsChecked = true;
            //        break;
            //}

            //CountryRadioButton1.IsChecked = GenericCodeClass.CanadaSelected;
            //CountryRadioButton2.IsChecked = !GenericCodeClass.CanadaSelected;

            //if (GenericCodeClass.CanadaSelected)
            //{
            //    ProductXML.SetSourceFile("ProvinceCities.xml");
            //    CityCodeXML.SetSourceFile("CityCodes.xml");
            //}
            //else
            //{
            //    ProductXML.SetSourceFile("USStateCities.xml");
            //    CityCodeXML.SetSourceFile("USCityCodes.xml");
            //}

            PopulateProductBox(true);
            //PopulateRegionBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), true);
            //SetOptions();
            //CountryRadioButton1.Checked += CountryRadioButton_CheckedHandler;
            //CountryRadioButton2.Checked += CountryRadioButton_CheckedHandler;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            //bool IsPolarSelected = ProductComboBox.Items[ProductComboBox.SelectedIndex].Equals("Polar Imagery");
            //bool IsNEPacSelected = RegionComboBox.Items[RegionComboBox.SelectedIndex].Equals("Northeast Pacific");
            //bool IsNWTerritoriesSelected = RegionComboBox.Items[RegionComboBox.SelectedIndex].Equals("Northwest Territories/Nunavut");
            //ChosenCityName = RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString();
            ChosenProductName = ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString();
            ChosenRegionName = RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString();
            string HomeURL = null;
            

            //if (ProductRadioButton1.IsChecked == true)
            //{
            //    if (IsNEPacSelected)
            //        ChosenSatelliteType = "alir";
            //    else if (IsPolarSelected && IsNWTerritoriesSelected)
            //        ChosenSatelliteType = "ir";
            //    else if (IsPolarSelected && !IsNWTerritoriesSelected)
            //        ChosenSatelliteType = "03";
            //    else
            //        ChosenSatelliteType = "ir4";
            //}
            //else if (ProductRadioButton2.IsChecked == true)
            //{
            //    if (GenericCodeClass.CanadaSelected)
            //        ChosenSatelliteType = "rb";
            //    else
            //        ChosenSatelliteType = "avn";
            //}
            //else if (ProductRadioButton3.IsChecked == true)
            //{
            //    ChosenSatelliteType = "rgb";
            //}
            //else if (ProductRadioButton4.IsChecked == true)
            //{
            //    if (IsNEPacSelected)
            //        ChosenSatelliteType = "alvs";
            //    else if (RegionComboBox.Items[RegionComboBox.SelectedIndex].Equals("Eastern Canada"))
            //        ChosenSatelliteType = "visible";
            //    else if (IsPolarSelected)
            //        ChosenSatelliteType = "nir";
            //    else
            //        ChosenSatelliteType = "vis";
            //}

            GenericCodeClass.SettingsChanged = !ChosenProductName.Equals(GenericCodeClass.ChosenProductString)
                                                    || !ChosenRegionName.Equals(GenericCodeClass.ChosenRegionString);

            if (GenericCodeClass.SettingsChanged)
            {
                if (ProductComboBox.SelectedItem.Equals("Graphical Area Forecasts") || ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                {
                    ChosenStationOrTypeName = StationOrTypeComboBox.Items[StationOrTypeComboBox.SelectedIndex].ToString();
                    GenericCodeClass.SettingsChanged = GenericCodeClass.SettingsChanged || !ChosenStationOrTypeName.Equals(GenericCodeClass.ChosenStationOrTypeString);
                }

                if (ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                {
                    ChosenDirName = DirComboBox.Items[DirComboBox.SelectedIndex].ToString();
                    GenericCodeClass.SettingsChanged = GenericCodeClass.SettingsChanged || !ChosenRegionName.Equals(GenericCodeClass.ChosenDirString);
                }

                if (GenericCodeClass.SettingsChanged)
                {
                    GenericCodeClass.ChosenProductString = ChosenProductName;
                    GenericCodeClass.ChosenRegionString = ChosenRegionName;
                    if (ProductComboBox.SelectedItem.Equals("Graphical Area Forecasts") || ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                        GenericCodeClass.ChosenStationOrTypeString = ChosenStationOrTypeName;

                    if (ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                        GenericCodeClass.ChosenDirString = ChosenDirName;

                    GenericCodeClass.RegionCodeString = ProductXML.ReadCode(ChosenProductName, ChosenRegionName, ChosenStationOrTypeName);

                    HomeURL = ProductXML.ReadBaseURL(ChosenProductName);
                    GenericCodeClass.HomeStation = HomeURL.Replace("{CODE}", GenericCodeClass.RegionCodeString);
                    //string HomeStation;

                    //if (RegionComboBox != null)
                    //{
                    //    GenericCodeClass.HomeStationCodeString = CityCodeXML.GetCityCode(RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString()); //Change this to ChosenCityCode?
                    //    HomeStation = CityCodeXML.GetHomeURL(RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString()); //Change this to ChosenCityCode?
                    //    HomeStation = HomeStation.Replace("{SC}", GenericCodeClass.HomeStationCodeString);
                    //    GenericCodeClass.HomeStation = HomeStation.Replace("{OPTION}", ChosenSatelliteType);
                    //}

                    //GenericCodeClass.ChosenProductString = RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString();
                    //GenericCodeClass.ChosenProductName = ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString();
                    ////GenericCodeClass.ChosenRegionString = ChosenSatelliteType;
                }
            }

            //Better to check for existing download intervals before setting new times?
            //if (DurationRadioButton1.IsChecked == true)
            //    GenericCodeClass.FileDownloadPeriod = 3;
            //else if (DurationRadioButton2.IsChecked == true)
            //    GenericCodeClass.FileDownloadPeriod = 6;
            //else if (DurationRadioButton3.IsChecked == true)
            //    GenericCodeClass.FileDownloadPeriod = 1;

            if (LoopTimerRadioButton1.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 100);
            else if (LoopTimerRadioButton2.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 500);
            else
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 1000);

            GenericCodeClass.SaveAppData(true);
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ashwin - Set the eneabled options on all combo boxes and then populate the region combo box. The next two levels will be
            //populated after being triggered from the selection changed events from region combobox
            if (ProductComboBox != null)
            {
                SetOptions();
                PopulateRegionBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), false);
            }
        }

        private void SetOptions()
        {
            //bool IsPolarSelected = ProductComboBox.Items[ProductComboBox.SelectedIndex].Equals("Polar Imagery");
            //bool AreProvincesSelected = ProductComboBox.Items[ProductComboBox.SelectedIndex].Equals("British Columbia") ||
            //                            ProductComboBox.Items[ProductComboBox.SelectedIndex].Equals("Ontario") ||
            //                            ProductComboBox.Items[ProductComboBox.SelectedIndex].Equals("New Brunswick");
            //bool IsRegionalSelected = ProductComboBox.Items[ProductComboBox.SelectedIndex].Equals("Regional Imagery");
            //bool IsNEPacSelected = RegionComboBox.Items[RegionComboBox.SelectedIndex].Equals("Northeast Pacific");
            //bool IsWestCanSelected = RegionComboBox.Items[RegionComboBox.SelectedIndex].Equals("Western Canada/USA");

            ////IR
            //ProductRadioButton1.IsEnabled = true;
            ////Rainbow
            //ProductRadioButton2.IsEnabled = !GenericCodeClass.CanadaSelected || (GenericCodeClass.CanadaSelected && AreProvincesSelected);
            ////RGB
            //ProductRadioButton3.IsEnabled = !GenericCodeClass.CanadaSelected || (GenericCodeClass.CanadaSelected && AreProvincesSelected);
            ////visible
            //ProductRadioButton4.IsEnabled = !GenericCodeClass.CanadaSelected || (GenericCodeClass.CanadaSelected && (IsPolarSelected || AreProvincesSelected)) || (GenericCodeClass.CanadaSelected && IsRegionalSelected && !IsWestCanSelected);
            
            //ProductRadioButton1.IsChecked = (bool)ProductRadioButton1.IsChecked || (!(bool)ProductRadioButton4.IsChecked && !ProductRadioButton2.IsEnabled) || ((bool)ProductRadioButton4.IsChecked && !ProductRadioButton4.IsEnabled);

            //DurationRadioButton1.IsEnabled = !GenericCodeClass.CanadaSelected || (IsRegionalSelected || !IsPolarSelected || AreProvincesSelected);  //3h
            //DurationRadioButton2.IsEnabled = !GenericCodeClass.CanadaSelected || (IsRegionalSelected || !IsPolarSelected || AreProvincesSelected);  //6h
            //DurationRadioButton3.IsEnabled = true;  //Latest
            //DurationRadioButton3.IsChecked = (bool)DurationRadioButton3.IsChecked || !DurationRadioButton1.IsEnabled;

            //LoopTimerRadioButton1.IsEnabled = DurationRadioButton1.IsEnabled || DurationRadioButton2.IsEnabled;
            //LoopTimerRadioButton2.IsEnabled = DurationRadioButton1.IsEnabled || DurationRadioButton2.IsEnabled;
            //LoopTimerRadioButton3.IsEnabled = DurationRadioButton1.IsEnabled || DurationRadioButton2.IsEnabled;

            //if (GenericCodeClass.CanadaSelected)
            //    ProductRadioButton2.Content = "Rainbow";
            //else
            //    ProductRadioButton2.Content = "Aviation";
            StationOrTypeComboBox.IsEnabled = ProductComboBox.SelectedItem.Equals("Graphical Area Forecasts") ||
                                                    ProductComboBox.SelectedItem.Equals("Weather Cameras");
            DirComboBox.IsEnabled = ProductComboBox.SelectedItem.Equals("Weather Cameras");

            if (!StationOrTypeComboBox.IsEnabled)
                StationOrTypeComboBox.Items.Clear();

            if (!DirComboBox.IsEnabled)
                DirComboBox.Items.Clear();
        }

        private void PopulateRegionBox(int ProductBoxIndex, string ProductName, bool UseHomeStationValue)
        {

            if (RegionComboBox != null)
            {
                List<string> RegionNames = new List<string>();

                RegionComboBox.SelectionChanged -= RegionComboBox_SelectionChanged;

                if (ProductName.Contains('&'))
                    ProductName = ProductName.Substring(0, 12);

                ProductXML.ReadProductOptionList(ProductName, RegionNames);
                if (RegionComboBox != null)
                {
                    RegionComboBox.Items.Clear();
                    foreach (string Option in RegionNames)
                        RegionComboBox.Items.Add(Option);
                }

                RegionComboBox.SelectionChanged += RegionComboBox_SelectionChanged;
                if (UseHomeStationValue)
                    RegionComboBox.SelectedItem = GenericCodeClass.ChosenRegionString;
                else
                    RegionComboBox.SelectedIndex = 0;
            }
        }

        private void PopulateStationOrTypeBox(int ProductBoxIndex, string ProductName, string StationName, bool UseHomeStationValue)
        {

            if (StationOrTypeComboBox != null)
            {
                List<string> Option2Names = new List<string>();

                StationOrTypeComboBox.SelectionChanged -= StationOrTypeComboBox_SelectionChanged;

                if (ProductName.Contains('&'))
                    ProductName = ProductName.Substring(0, 12);

                ProductXML.ReadChoiceList(ProductName, StationName, Option2Names);
                if (StationOrTypeComboBox != null)
                {
                    StationOrTypeComboBox.Items.Clear();
                    foreach (string Option in Option2Names)
                        StationOrTypeComboBox.Items.Add(Option);
                }

                StationOrTypeComboBox.SelectionChanged += StationOrTypeComboBox_SelectionChanged;
                if (UseHomeStationValue)
                    StationOrTypeComboBox.SelectedItem = GenericCodeClass.ChosenStationOrTypeString;
                else
                    StationOrTypeComboBox.SelectedIndex = 0;

            }
        }
        //private void PopulateStationBox(int ProvinceBoxIndex, string ProvinceName, bool UseHomeStationValue)
        //{

        //    if (RegionComboBox != null)
        //    {
        //        List<string> CityNames = new List<string>();

        //        RegionComboBox.SelectionChanged -= RegionComboBox_SelectionChanged;

        //        if (ProvinceName.Contains('&'))
        //            ProvinceName = ProvinceName.Substring(0, 12);

        //        ProductXML.ReadCitiesInProvince(ProvinceName, CityNames);
        //        if (RegionComboBox != null)
        //        {
        //            RegionComboBox.Items.Clear();
        //            foreach (string City in CityNames)
        //            {
        //                RegionComboBox.Items.Add(City);
        //            }
        //        }

        //        if (UseHomeStationValue)
        //            RegionComboBox.SelectedItem = GenericCodeClass.ChosenProductString;
        //        else
        //            RegionComboBox.SelectedIndex = 0;

        //        RegionComboBox.SelectionChanged += RegionComboBox_SelectionChanged;
        //    }
        //}

        //private void CountryRadioButton_CheckedHandler(object sender, RoutedEventArgs e)
        //{
        //    if (sender == CountryRadioButton1)
        //    {
        //        if (GenericCodeClass.CanadaSelected)
        //            return;
        //        ProductXML.SetSourceFile("ProvinceCities.xml");
        //        CityCodeXML.SetSourceFile("CityCodes.xml");
        //        GenericCodeClass.CanadaSelected = true;
        //    }
        //    else if (sender == CountryRadioButton2)
        //    {
        //        if (!GenericCodeClass.CanadaSelected)
        //            return;
        //        ProductXML.SetSourceFile("USStateCities.xml");
        //        CityCodeXML.SetSourceFile("USCityCodes.xml");
        //        GenericCodeClass.CanadaSelected = false;
        //    }
        //    PopulateProductBox(false);
        //}

        private void PopulateDirBox(int ProductBoxIndex, string ProductName, string StationName, string TypeName, bool UseHomeStationValue)
        {

            if (DirComboBox != null)
            {
                List<string> DirList = new List<string>();

                //DirComboBox.SelectionChanged -= DirComboBox_SelectionChanged;

                if (ProductName.Contains('&'))
                    ProductName = ProductName.Substring(0, 12);

                ProductXML.ReadDirList(ProductName, StationName, TypeName, DirList);
                if (DirComboBox != null)
                {
                    DirComboBox.Items.Clear();
                    foreach (string Option in DirList)
                        DirComboBox.Items.Add(Option);
                }


                if (UseHomeStationValue)
                    DirComboBox.SelectedItem = GenericCodeClass.ChosenDirString;
                else
                    DirComboBox.SelectedIndex = 0;
                //DirComboBox.SelectionChanged += DirComboBox_SelectionChanged;
            }
        }

        private void PopulateProductBox(bool UseHomeStationVlaue)
        {
            List<string> ProductList = ProductXML.ReadProductList();

            if (ProductComboBox != null && ProductList != null)
            {
                ProductComboBox.SelectionChanged -= ProductComboBox_SelectionChanged;
                ProductComboBox.Items.Clear();

                foreach (string str in ProductList)
                    ProductComboBox.Items.Add(str);

                ProductComboBox.SelectionChanged += ProductComboBox_SelectionChanged;
                if (UseHomeStationVlaue)
                    ProductComboBox.SelectedItem = GenericCodeClass.ChosenProductString;
                else
                    ProductComboBox.SelectedIndex = 0;
                SetOptions();   //Set enabled properties for all boxes
            }
        }

        private void RegionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductComboBox != null && RegionComboBox != null)
            {
                if (ProductComboBox.SelectedItem.Equals("Graphical Area Forecasts") || ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                {
                    //StationOrTypeComboBox.IsEnabled = true;
                    PopulateStationOrTypeBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString(), false);
                    //SetOptions();
                }
                //else
                //    StationOrTypeComboBox.IsEnabled = false;

            }
        }

        private void StationOrTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StationOrTypeComboBox.IsEnabled && ProductComboBox != null && RegionComboBox != null & StationOrTypeComboBox != null)
            {
                if (ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                {
                    //DirComboBox.IsEnabled = true;
                    PopulateDirBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString(), StationOrTypeComboBox.Items[StationOrTypeComboBox.SelectedIndex].ToString(), false);
                }
                //else
                //{
                //    DirComboBox.Items.Clear();
                //    DirComboBox.IsEnabled = false;
                //}
                //SetOptions();
            }
        }

        private void DirComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}

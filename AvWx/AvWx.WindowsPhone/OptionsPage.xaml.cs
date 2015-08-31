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
        private static XMLParserClass ProductXML = new XMLParserClass("Products.xml");


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

            PopulateProductBox(true);
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
            ChosenProductName = ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString();
            ChosenRegionName = RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString();
            string HomeURL = null;

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
                }
            }

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

        private void PopulateDirBox(int ProductBoxIndex, string ProductName, string StationName, string TypeName, bool UseHomeStationValue)
        {

            if (DirComboBox != null)
            {
                List<string> DirList = new List<string>();

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
                    PopulateStationOrTypeBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString(), false);
            }
        }

        private void StationOrTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StationOrTypeComboBox.IsEnabled && ProductComboBox != null && RegionComboBox != null & StationOrTypeComboBox != null)
            {
                if (ProductComboBox.SelectedItem.Equals("Weather Cameras"))
                {
            
                    PopulateDirBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString(), StationOrTypeComboBox.Items[StationOrTypeComboBox.SelectedIndex].ToString(), false);
                }
            }
        }

        private void DirComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}

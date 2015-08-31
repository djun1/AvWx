using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace AvWx
{
    public sealed partial class OptionsPage : SettingsFlyout
    {
        public event EventHandler SettingsChanged;
        private static String ChosenProductName;
        private static String ChosenRegionName;
        private static String ChosenStationOrTypeName;
        private static String ChosenDirName;
        private static String ChosenURL;
        private static String ChosenSatelliteType;
        private static XMLParserClass ProductXML = new XMLParserClass("Products.xml");
        private static XMLParserClass CityCodeXML = new XMLParserClass("CityCodes.xml");

        public OptionsPage()
        {
            this.InitializeComponent();            
        }

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

        private void OptionsPage_Unloaded(object sender, RoutedEventArgs e)
        {

            ChosenProductName = ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString();
            ChosenRegionName = RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString();
            string HomeURL = null;

            GenericCodeClass.SettingsChanged = !ChosenProductName.Equals(GenericCodeClass.ChosenProductString)
                                                    || !ChosenRegionName.Equals(GenericCodeClass.ChosenRegionString);

            if(ProductComboBox.SelectedItem.Equals("Graphical Area Forecasts") || ProductComboBox.SelectedItem.Equals("Weather Cameras"))
            {
                ChosenStationOrTypeName = StationOrTypeComboBox.Items[StationOrTypeComboBox.SelectedIndex].ToString();
                GenericCodeClass.SettingsChanged = GenericCodeClass.SettingsChanged || !ChosenStationOrTypeName.Equals(GenericCodeClass.ChosenStationOrTypeString);
            }
                
            if(ProductComboBox.SelectedItem.Equals("Weather Cameras"))
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
            
            
            if (LoopTimerRadioButton1.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 100);
            else if (LoopTimerRadioButton2.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 500);
            else
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 1000);
            
            if (SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
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
                case 0:
                    if (GenericCodeClass.LoopInterval.Seconds == 1)
                        LoopTimerRadioButton3.IsChecked = true;
                    break;
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
                {
                    PopulateStationOrTypeBox(ProductComboBox.SelectedIndex, ProductComboBox.Items[ProductComboBox.SelectedIndex].ToString(), RegionComboBox.Items[RegionComboBox.SelectedIndex].ToString(), false);
                    
                }
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

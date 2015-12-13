using ImageCharacteristics.ImageClasses;
using Microsoft.Win32;
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

namespace ImageCharacteristics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Images to work.
        private BitmapSource original1;
        private OpenFileDialog selectImage;
        private SaveFileDialog saveFile;

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = screenWidth / 2 - windowWidth / 2;
            this.Top = screenHeight / 2 - windowHeight / 2;
        }
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
            bttnCompareImgs.IsEnabled = false;

        }

        #region Similarity

        ImageSimilarity image_s1, image_s2;

        /// <summary>
        /// Load image for left panel.
        /// </summary>
        private void LoadImage1(object sender, RoutedEventArgs e)
        {

            selectImage = new OpenFileDialog();
            selectImage.Title = "Select a image";
            selectImage.Filter = "BMP|*.bmp";
            if (selectImage.ShowDialog() == true)
            {
                original1 = new BitmapImage(new Uri(selectImage.FileName));
                imgLeft.Source = new BitmapImage(new Uri(selectImage.FileName));
                image_s1 = new ImageSimilarity(original1, selectImage.SafeFileName);
                image_s1.CalculateMassCenter();
                bttnCompareImgs.IsEnabled = CanCompare();                
            }
        }

        /// <summary>
        /// Load image for right panel.
        /// </summary>
        private void LoadImage2(object sender, RoutedEventArgs e)
        {
            selectImage = new OpenFileDialog();
            selectImage.Title = "Select a image";
            selectImage.Filter = "BMP|*.bmp";
            if(selectImage.ShowDialog() == true)
            {
                original1 = new BitmapImage(new Uri(selectImage.FileName));
                imgRight.Source = new BitmapImage(new Uri(selectImage.FileName));
                image_s2 = new ImageSimilarity(original1, selectImage.SafeFileName);
                image_s2.CalculateMassCenter();
                bttnCompareImgs.IsEnabled = CanCompare();
            }
        }

        /// <summary>
        /// Returns Tanimoto distance.
        /// </summary>
        private void CompareImages(object sender, RoutedEventArgs e)
        {
            imgOverlay.Source =  (image_s1.height * image_s1.width >= image_s2.height * image_s2.width 
                                    ? image_s1.TrasposeImages(image_s2) : image_s2.TrasposeImages(image_s1));
            overlayInfo.Text = "//////////////////// Tanimoto distance ////////////////////";
            overlayInfo.Text += Environment.NewLine + "Comparing " + image_s1.name + " and " + image_s2.name + ":" + Environment.NewLine + Environment.NewLine;
            overlayInfo.Text += "Similarity: " + image_s1.CompareWith(image_s2).ToString();
            overlayInfo.Text += Environment.NewLine + Environment.NewLine + "////////////////////////// Details ////////////////////////" + Environment.NewLine;
            overlayInfo.Text += "Common 1-pixels:   " + image_s1.common_pixels;
            overlayInfo.Text += Environment.NewLine + image_s1.name + "\'s 1-pixels: " + image_s1.onePixels;
            overlayInfo.Text += Environment.NewLine + image_s2.name + "\'s 1-pixels: " + image_s2.onePixels;
        }

        /// <summary>
        /// Allows save tanimoto distance.
        /// </summary>
        private void SaveInfoSimilarity(object sender, RoutedEventArgs e)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Text file|*.txt";
            saveFile.Title = "Save information.";
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                System.IO.File.WriteAllText(saveFile.FileName, overlayInfo.Text.ToString());
                MessageBox.Show("File saved successfully", "File Saved!");
            }
        }

        /// <summary>
        /// Check if two images to compare are loaded.
        /// </summary>
        /// <returns>True if both images are selected. Otherwise returns false.</returns>
        private Boolean CanCompare()
        {
            if (image_s1 != null && image_s2 != null)
                return true;
            return false;
        }

        #endregion

        #region Topology

        private void LoadTopologyImage(object sender, RoutedEventArgs e)
        {
            selectImage = new OpenFileDialog();
            selectImage.Title = "Select a image";
            selectImage.Filter = "BMP|*.bmp";
            if (selectImage.ShowDialog() == true)
            {
                original1 = new BitmapImage(new Uri(selectImage.FileName));
                imgTopo.Source = new BitmapImage(new Uri(selectImage.FileName));
                EulerImage image = new EulerImage(original1, selectImage.SafeFileName);
                image.CalculateMassCenter();
                TopoInfo.Text = "/////////////// Topology ///////////////" + Environment.NewLine;
                TopoInfo.Text += "Name = " + image.name + Environment.NewLine;
                TopoInfo.Text += "Height = " + image.height + Environment.NewLine;
                TopoInfo.Text += "Width = " + image.width + Environment.NewLine;
                TopoInfo.Text += "1-pixels = " + image.onePixels + Environment.NewLine;
                TopoInfo.Text += "Components = " + image.components + Environment.NewLine;
                TopoInfo.Text += "Mass Center = " + "(" + image.xmc + "," + image.ymc + ")" + Environment.NewLine;
                TopoInfo.Text += "Wrap Perimeter = " + image.WrapPerimeter + Environment.NewLine;
                TopoInfo.Text += "Contact Perimeter = " + image.ContactPerimeter + Environment.NewLine;
                TopoInfo.Text += "Tetrapixels = " + image.TetrapixelsNumber + Environment.NewLine;
                TopoInfo.Text += "Vertex = " + image.VertexNumber + Environment.NewLine;
                TopoInfo.Text += "Holes = " + image.HolesNumber + Environment.NewLine;
                TopoInfo.Text += "Euler Characteristic = " + image.EulerCharacteristic + Environment.NewLine;
            }
        }

        private void SaveTopology(object sender, RoutedEventArgs e)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Text file|*.txt";
            saveFile.Title = "Save information.";
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                System.IO.File.WriteAllText(saveFile.FileName, TopoInfo.Text);
                MessageBox.Show("File saved successfully", "File Saved!");
            }
        }

        #endregion

        #region Chains

        private void LoadChainImage(object sender, RoutedEventArgs e)
        {
            selectImage = new OpenFileDialog();
            selectImage.Title = "Select a image";
            selectImage.Filter = "BMP|*.bmp";
            if (selectImage.ShowDialog() == true)
            {
                original1 = new BitmapImage(new Uri(selectImage.FileName));
                imgChain.Source = new BitmapImage(new Uri(selectImage.FileName));
                if(original1.PixelWidth* original1.PixelHeight < 700 * 700)
                {
                    ChainCodes image = new ChainCodes(original1, selectImage.SafeFileName);
                    image.CalculateMassCenter();
                    ChainInfo.Text = "/////////////// Topology ///////////////" + Environment.NewLine + "Name:" + image.name + Environment.NewLine + Environment.NewLine;
                    ChainInfo.Text += "<< F8 >>" + Environment.NewLine + Environment.NewLine;
                    
                    //F8
                    for(int i = 0; i < image.F8.Count; i++)
                    {
                        ChainInfo.Text += image.Coordenates.ElementAt(i) + Environment.NewLine;
                        ChainInfo.Text += image.F8.ElementAt(i) + Environment.NewLine + Environment.NewLine;
                    }

                    //AF8
                    ChainInfo.Text += "<<<<< AF8 >>>>>" + Environment.NewLine + Environment.NewLine;
                    for (int i = 0; i < image.F8.Count; i++)
                    {
                        ChainInfo.Text += image.Coordenates.ElementAt(i) + Environment.NewLine;
                        ChainInfo.Text += image.AF8.ElementAt(i) + Environment.NewLine + Environment.NewLine;
                    }

                    //F4
                    ChainInfo.Text += "<<<<< F4 >>>>>" + Environment.NewLine + Environment.NewLine;
                    for (int i = 0; i < image.F8.Count; i++)
                    {
                        ChainInfo.Text += image.Coordenates.ElementAt(i) + Environment.NewLine;
                        ChainInfo.Text += image.F4.ElementAt(i) + Environment.NewLine + Environment.NewLine;
                    }

                    //VCC
                    ChainInfo.Text += "<<<<< VCC >>>>>" + Environment.NewLine + Environment.NewLine;
                    for (int i = 0; i < image.F8.Count; i++)
                    {
                        ChainInfo.Text += image.Coordenates.ElementAt(i) + Environment.NewLine;
                        ChainInfo.Text += image.VCC.ElementAt(i) + Environment.NewLine + Environment.NewLine;
                    }

                    //3OT
                    ChainInfo.Text += "<<<<< 3OT >>>>>" + Environment.NewLine + Environment.NewLine;
                    for (int i = 0; i < image._3OT.Count; i++)
                    {
                        ChainInfo.Text += image.Coordenates.ElementAt(i) + Environment.NewLine;
                        ChainInfo.Text += image._3OT.ElementAt(i) + Environment.NewLine + Environment.NewLine;
                    }
                }
                else
                {
                    MessageBox.Show("Image is too big to show Chain Code. Please select a smaller image.");
                    ChainInfo.Text = "";
                }
            }
        }

        private void SaveChain(object sender, RoutedEventArgs e)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Text file|*.txt";
            saveFile.Title = "Save information.";
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                System.IO.File.WriteAllText(saveFile.FileName, ChainInfo.Text);
                MessageBox.Show("File saved successfully", "File Saved!");
            }
        }

        #endregion

        #region Moments

        private void LoadMomentsImage(object sender, RoutedEventArgs e)
        {
            selectImage = new OpenFileDialog();
            selectImage.Title = "Select a image";
            selectImage.Filter = "BMP|*.bmp";
            if (selectImage.ShowDialog() == true)
            {
                original1 = new BitmapImage(new Uri(selectImage.FileName));
                imgMoments.Source = new BitmapImage(new Uri(selectImage.FileName));
                ImageMoments image = new ImageMoments(original1, selectImage.SafeFileName);
                MomentsInfo.Text = "//////////////////// Moments ////////////////////" + Environment.NewLine + Environment.NewLine;

                MomentsInfo.Text += "<<<<<Central Moments>>>>>" + Environment.NewLine + Environment.NewLine;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        MomentsInfo.Text += "M" + i.ToString() + j.ToString() + " = " + image.getCentralMoment(i, j).ToString() + Environment.NewLine;

                MomentsInfo.Text += Environment.NewLine + "<<<<<Scaling Moments>>>>>" + Environment.NewLine + Environment.NewLine;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        MomentsInfo.Text += "N" + i.ToString() + j.ToString() + " = " + image.getScalingMoment(i, j).ToString() + Environment.NewLine;

                MomentsInfo.Text += Environment.NewLine + "<<<<<Rotation Moments>>>>>" + Environment.NewLine + Environment.NewLine;
                MomentsInfo.Text += "R1 = " + image.getFirstRotationMoment().ToString() + Environment.NewLine;
                MomentsInfo.Text += "R2 = " + image.getSecondRotationMoment().ToString() + Environment.NewLine;
                MomentsInfo.Text += "R3 = " + image.getThirdRotationMoment().ToString() + Environment.NewLine;
            }
        }

        private void SaveMoment(object sender, RoutedEventArgs e)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Text file|*.txt";
            saveFile.Title = "Save information.";
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                System.IO.File.WriteAllText(saveFile.FileName, MomentsInfo.Text);
                MessageBox.Show("File saved successfully", "File Saved!");
            }
        }

        #endregion
    }
}

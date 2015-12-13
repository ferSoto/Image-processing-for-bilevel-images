using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageCharacteristics.ImageClasses
{
    class ImageSimilarity : Images
    {

        public int common_pixels { get; protected set; }

        #region Constructors

        public ImageSimilarity(BitmapSource image, string name) 
        : base(image, name)
        {

        }

        #endregion

        /// <summary>
        /// Translates the image alpha pixels in x and beta in y.
        /// </summary>
        /// <param name="alpha">Integer value greater or equal to 0.</param>
        /// <param name="beta">Integer value greater or equal to 0.</param>
        protected void TranslateImage(uint alpha, uint beta)
        {
            try
            {
                //Creating new image matrix.
                int[,] new_image;
                new_image = new int[this.height + beta, this.width + alpha];
                //Copying traslated image to new image matrix.
                for (int i = 0; i < this.height; i++)
                    for (int j = 0; j < this.width; j++)
                        new_image[i + beta, j + alpha] = this.image[i, j];
                //Making new image matrix our actual image matrix.
                this.image = new_image;
                //Updating image properties.
                this.height += (int)beta;
                this.width += (int)alpha;
                CalculateMassCenter();
            }
            catch (IndexOutOfRangeException)
            {

            }
            
        }

        /// <summary>
        /// Receives an Images object and returns its Tanimoto distance
        /// comparated to this image.
        /// </summary>
        /// <param name="second_image"></param>
        /// <returns>Value between 0 and 1.
        /// Tend to 1 means they're differents.
        /// Tend to 0 means they're similar.</returns>
        public double CompareWith(Images second_image)
        {
            second_image.CalculateMassCenter();
            
            //Setting up new mass center for both images.
            int new_xmc, new_ymc;
            new_xmc = Math.Max(this.xmc, second_image.xmc);
            new_ymc = Math.Max(this.ymc, second_image.ymc);
            
            //Creating matrix to overlay images
            int[,] overlay_matrix;
            overlay_matrix = new int[new_ymc + Math.Max(this.height - this.ymc, second_image.height - second_image.ymc),
                                     new_xmc + Math.Max(this.width - this.xmc, second_image.width - second_image.xmc)];
            
            
            //Setting up alpha & beta for both images.
            int alpha1, alpha2, beta1, beta2;
            alpha1 = new_xmc - this.xmc;
            beta1 = new_ymc - this.ymc;
            alpha2 = new_xmc - second_image.xmc;
            beta2 = new_ymc - second_image.ymc;


            //Copying image1 to overlay matrix.
            for (int i = 0; i < this.height; i++)
                for (int j = 0; j < this.width; j++)
                    overlay_matrix[i + beta1, j + alpha1] = this.image[i, j];

            //Copying image2 to overlay matrix.
            for (int i = 0; i < second_image.height; i++)
                for (int j = 0; j < second_image.width; j++)
                    //We're using the sum to mark  the pixels where they are common 1-pixels.
                    overlay_matrix[i + beta2, j + alpha2] += second_image.image[i, j];

            //Getting common pixels.
            this.common_pixels = 0;
            foreach (int i in overlay_matrix)
                this.common_pixels += (i == 2 ? 1 : 0);

            return ((double)this.onePixels + second_image.onePixels - 2 * this.common_pixels) / 
                (this.onePixels + second_image.onePixels - (double)this.common_pixels);
        }

        public BitmapImage TrasposeImages(Images second_image)
        {
            second_image.CalculateMassCenter();

            //Setting up new mass center for both images.
            int new_xmc, new_ymc;
            new_xmc = Math.Max(this.xmc, second_image.xmc);
            new_ymc = Math.Max(this.ymc, second_image.ymc);

            //Creating matrix to overlay images
            int[,] overlay_matrix;
            int over_height = new_ymc + Math.Max(this.height - this.ymc, second_image.height - second_image.ymc);
            int over_width = new_xmc + Math.Max(this.width - this.xmc, second_image.width - second_image.xmc);
            overlay_matrix = new int[over_height, over_width];


            //Setting up alpha & beta for both images.
            int alpha1, alpha2, beta1, beta2;
            alpha1 = new_xmc - this.xmc;
            beta1 = new_ymc - this.ymc;
            alpha2 = new_xmc - second_image.xmc;
            beta2 = new_ymc - second_image.ymc;


            //Copying image1 to overlay matrix.
            for (int i = 0; i < this.height; i++)
                for (int j = 0; j < this.width; j++)
                    overlay_matrix[i + beta1, j + alpha1] = this.image[i, j];

            //Copying image2 to overlay matrix.
            for (int i = 0; i < second_image.height; i++)
                for (int j = 0; j < second_image.width; j++)
                    //We're using the sum to mark  the pixels where they are common 1-pixels.
                    overlay_matrix[i + beta2, j + alpha2] += second_image.image[i, j];

            Byte[] image_src;
            image_src = new Byte[over_height * over_width];

            //Transforming overlay image to byte array.
            for (int i = 0; i < over_height; i++)
                for(int j = 0; j < over_width; j++)
                {
                    //255 -> White
                    //0   -> Black
                    image_src[i * this.width + j] = (byte)(
                        overlay_matrix[i, j] == 0 ? 255 : (overlay_matrix[i, j] == 1 ? 128 : 0)
                        );
                }

            //Creating bitmap image from byte array.
            var dpi = 96d;
            var new_image = BitmapSource.Create(over_width, over_height, dpi, dpi, PixelFormats.Gray8, null, image_src, over_width);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream ms = new MemoryStream();
            BitmapImage really_new_image = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(new_image));
            encoder.Save(ms);

            really_new_image.BeginInit();
            really_new_image.StreamSource = new MemoryStream(ms.ToArray());
            really_new_image.EndInit();

            ms.Close();
            return really_new_image;
        }
    }
}

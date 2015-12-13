using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageCharacteristics.ImageClasses
{
    class Images
    {
        #region Atributtes

        public string name { get; protected set; }
        public int[,] image { get; protected set; }
        public int height { get; protected set; }
        public int width { get; protected set; }
        public int xmc { get; protected set; }
        public int ymc { get; protected set; }
        public int onePixels { get; protected set; }
        public int components { get; protected set; }
        private int[,] mirror_image;
        
        #endregion

        #region Constructors
        public Images()
        {

        }

        public Images(BitmapSource image, string name)
        {
            byte[] pixelsByte;
            FormatConvertedBitmap converted_image;
            converted_image = new FormatConvertedBitmap(image, PixelFormats.Gray8, BitmapPalettes.BlackAndWhite, 1);
            this.name = name;
            this.width = image.PixelWidth;
            this.height = image.PixelHeight;
            
            //Moving image pixels to byte array
            pixelsByte = new byte[this.height * this.width];
            converted_image.CopyPixels(pixelsByte, this.width, 0);
            
            //Transforming byte array to int matrix.
            this.image = new int[this.height, this.width];
            this.mirror_image = new int[this.height, this.width];
            for(int i = 0; i < this.height; i++)
            {
                for(int j = 0; j < this.width; j++)
                {
                    this.image[i, j] = (pixelsByte[i * this.width + j] == 255 ? 0 : 1);
                    this.mirror_image[i , j] = (pixelsByte[i * this.width + j] == 255 ? 0 : 1);
                }
            }

            //Getting number of 1-pixels
            this.onePixels = 0;
            for (int i = 0; i < this.height; i++)
                for (int j = 0; j < this.width; j++)
                    this.onePixels += this.image[i, j];

            //Getting connected components
            CalculateComponents();
        }

        #endregion

        public void CalculateMassCenter()
        {
            long xs = 0,
                 ys = 0,
                 ones = 0;
            for(int i = 0; i < this.height; i++)
                for(int j = 0; j < this.width; j++)
                    if(this.image[i, j] != 0)
                    {
                        xs += j;
                        ys += i;
                        ones += 1;
                    }
            if(ones != 0)
            {
                this.xmc = (int)Math.Round((double)xs / ones);
                this.ymc = (int)Math.Round((double)ys / ones);
            }
            else
            {
                //If there isn't 1 pixels.
                this.xmc = this.ymc = 0;
            }
            
        }

        /// <summary>
        /// Calculates the number of connected components.
        /// </summary>
        private void CalculateComponents()
        {
            this.components = 0;
            for(int i  = 0; i < this.height; i++)
                for(int j = 0; j < this.width; j++)
                    if(this.mirror_image[i, j] == 1)
                    {
                        this.components += 1;
                        BreadthSearch(i, j); 
                    }   
        }

        /// <summary>
        /// Deep search in 8-neighborhood.
        /// </summary>
        /// <param name="x">Current pixel coordinate.</param>
        /// <param name="y">Current pixel coordinate.</param>
        private void BreadthSearch(int y, int x)
        {
            Queue<KeyValuePair<int, int>> queue;
            queue = new Queue<KeyValuePair<int, int>>();
            //Adding search root.
            queue.Enqueue(new KeyValuePair<int, int>(y, x));
            while(queue.Count > 0)
            {
                KeyValuePair<int, int> current_position = queue.Dequeue();
                //Marking current pixel as visited.
                this.mirror_image[current_position.Key, current_position.Value] = 0;
                //Visiting 8-neighborhood.
                for (int i = (current_position.Key - 1 < 0 ? 0 : -1); i <= (current_position.Key + 1 == this.height ? 0 : 1); i++)
                {
                    for (int j = (current_position.Value - 1 < 0 ? 0 : -1); j <= (current_position.Value + 1 == this.width ? 0 : 1); j++)
                    {
                        //If one pixel in the 8-neighborhood is 1-pixel, add to the queue.
                        if (this.mirror_image[current_position.Key + i, current_position.Value + j] == 1 )
                        {
                            queue.Enqueue(new KeyValuePair<int, int>(current_position.Key + i, current_position.Value + j));
                            this.mirror_image[current_position.Key + i, current_position.Value + j] = 2;
                        }   
                    }
                }           
            }
        }

    }
}

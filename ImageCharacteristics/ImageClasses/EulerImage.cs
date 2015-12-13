using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageCharacteristics.ImageClasses
{
    class EulerImage : Images
    {
        #region Attributes

        public int WrapPerimeter { get; private set; }
        public ulong ContactPerimeter { get; private set; }
        public int HolesNumber { get; private set; }
        public long TetrapixelsNumber { get; private set; }
        public long VertexNumber { get; private set; }
        public int EulerCharacteristic { get; private set; }

        #endregion

        #region Constructors

        public EulerImage(BitmapSource image, string name)
        : base(image, name)
        {
            setWrapPerimeter();
            setTetraPixelNumber();
            setContactPerimeter();
            setVertexNumber();
            setHoleNumber();
            setEulerCharacteristic();
        }
        #endregion

        /// <summary>
        /// Sets wrap perimeter.
        /// </summary>
        private void setWrapPerimeter()
        {
            this.WrapPerimeter = 0;
            for(int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                    //Iw we're in 1-pixel.
                    if (this.image[i, j] == 1)
                        for (int h = 0; h < 2; h++)
                            for (int k = 0; k < 2; k++)
                            {
                                if (k * h == 0 && (k != 0 || h != 0) && this.image[i + h, j + k] == 0)
                                    this.WrapPerimeter++;
                            }
                    else
                        //If we're in 0-pixel.
                        for (int h = i; h < Math.Min(this.height - 2, i + 2); h++)
                            for (int k = j; k < Math.Min(this.width - 2, j + 2); k++)
                            {
                                if ((k != j || h != i) && (h == i || k == j) && this.image[h, k] == 1)
                                    this.WrapPerimeter++;
                            }
            }
        }

        /// <summary>
        /// Count the number of tetrapixels in the objects.
        /// </summary>
        private void setTetraPixelNumber()
        {
            this.TetrapixelsNumber = 0;
            for (int i = 0; i < this.height; i++)
                for (int j = 0; j < this.width; j++)
                    if (this.image[i, j] == 1 && this.image[i, j + 1] == 1
                        && this.image[i + 1, j] == 1 && this.image[i + 1, j + 1] == 1)
                        this.TetrapixelsNumber++;
        }

        /// <summary>
        /// Sets Contact perimeter using  (4 * 1-pixels - wrap perimeter)/2
        /// </summary>
        private void setContactPerimeter()
        {
            this.ContactPerimeter = (4 * (ulong)this.onePixels * (ulong)this.WrapPerimeter) / 2;
        }

        /// <summary>
        /// Sets number of vertex.
        /// </summary>
        private void setVertexNumber()
        {
            this.VertexNumber = this.TetrapixelsNumber + this.WrapPerimeter;
        }

        /// <summary>
        /// Sets number of holes of the objects in the image.
        /// </summary>
        private void setHoleNumber()
        {
            this.HolesNumber = this.components - (int)this.TetrapixelsNumber + (2 * this.onePixels - this.WrapPerimeter) / 2;
        }

        private void setEulerCharacteristic()
        {
            this.EulerCharacteristic = this.components - this.HolesNumber;
        }
    }
}

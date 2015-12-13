using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageCharacteristics.ImageClasses
{
    class ImageMoments : ImageSimilarity
    {
        #region Constructors

        public ImageMoments(BitmapSource image, string name) 
        : base(image, name)
        {
            CalculateMassCenter();
        }

        #endregion

        /// <summary>
        /// Calculates the pq-th order moment of the image and returns its value.
        /// </summary>
        /// <param name="p">Integer value greater or equal to 0.</param>
        /// <param name="q">Integer value greater or equal to 0.</param>
        public double getCentralMoment(int p, int q)
        {
            double Mpq = 0;
            for (int i = 0; i < this.height; i++)
                for (int j = 0; j < this.width; j++)
                    if (this.image[i, j] == 1)
                        Mpq += Math.Pow(i - this.xmc, p) * Math.Pow(j - this.ymc, q);
            return Mpq;
        }

        /// <summary>
        /// Returns M00^((p+q)/2 + 1).
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private double getM00Some(int p, int q)
        {
            return Math.Pow(this.onePixels, (p + q) / 2 + 1);
        }

        /// <summary>
        /// Returns the pq-th scaling moment.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public double getScalingMoment(int p, int q)
        {
            return getCentralMoment(p, q) / getM00Some(p, q);
        }

        /// <summary>
        /// First moment for rotations transformation.
        /// </summary>
        /// <returns></returns>
        public double getFirstRotationMoment()
        {
            return getCentralMoment(0, 2) + getCentralMoment(2, 0);
        }

        /// <summary>
        /// Second moment for rotation transformation.
        /// </summary>
        /// <returns></returns>
        public double getSecondRotationMoment()
        {
            return Math.Pow(getCentralMoment(2, 0) - getCentralMoment(0, 2), 2)
                        + 4 * Math.Pow(getCentralMoment(1, 1), 2);
        }

        /// <summary>
        /// Third moment for rotation transformation.
        /// </summary>
        /// <returns></returns>
        public double getThirdRotationMoment()
        {
            return Math.Pow(getCentralMoment(3, 0) - 3 * getCentralMoment(1, 2), 2)
                       + Math.Pow(3 * getCentralMoment(2, 1) - getCentralMoment(0, 3), 2);
        }
    }
}


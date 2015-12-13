using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageCharacteristics.ImageClasses
{
    class ChainCodes : Images
    {

        #region Attributes

        public List<string> F8 { get; private set; }
        public List<string> F4 { get; private set; }
        public List<string> _3OT { get; private set; }
        public List<string> VCC { get; private set; }
        public List<string> AF8 { get; private set; }
        public List<string> Coordenates { get; private set; }

        private static Dictionary<int, KeyValuePair<int, int>> F8_dictionary;
        private static string[,] AF8_matrix = {
            { "0", "1", "2", "3", "4", "5", "6", "7" }, 
            { "7", "0", "1", "2", "3", "4", "5", "6" },
            { "6", "7", "0", "1", "2", "3", "4", "5" },
            { "5", "6", "7", "0", "1", "2", "3", "4" },
            { "4", "5", "6", "7", "0", "1", "2", "3" },
            { "3", "4", "5", "6", "7", "0", "1", "2" },
            { "2", "3", "4", "5", "6", "7", "0", "1" },
            { "1", "2", "3", "4", "5", "6", "7", "0" },
        };
        private static string[,] F4_matrix = { 
            { "0" , "010" , "01", "0121", ""  , ""    , ""  , "03"   },
            { ""  , "10"  , "1" , "121" , "12", ""    , ""  , "3"    },
            { ""  , "10"  , "1" , "121" , "12", "1232", ""  , ""     },
            { ""  , "0"   , ""  , "21"  , "2" , "232" , "23", ""     },
            { ""  , ""    , ""  , "21"  , "2" , "232" , "23", "2303" },
            { "30", ""    , ""  , "1"   , ""  , "32"  , "3" , "303"  },
            { "30", "2010", ""  , ""    , ""  , "32"  , "3" , "303"  },
            { "0" , "010" , "01", ""    , ""  , "2"   , ""  ,"03"    }
        };

        private static string[,] _3OT_matrix = {
            { "1", "" , "2", ""  },
            { "" , "1", "" , "2" },
            { "2", "" , "1", ""  },
            { "" , "2", "" , "1" }
        };
        private static string[,] VCC_matrix = {
            { "0", "1", "" , "2"},
            { "2", "1", "1", "" },
            { "" , "2", "0", "1"},
            { "1", "" , "2", "0"}
        };

        #endregion

        #region Constructors

        public ChainCodes (BitmapSource image, string name)
        : base(image, name)
        {
            //Marking perimeter.
            MarkPerimeter();

            //Initializating code chains.
            this.F8 = new List<string>();
            this.F4 = new List<string>();
            this.VCC = new List<string>();
            this._3OT = new List<string>();
            this.AF8 = new List<string>();
            this.Coordenates = new List<string>();
            
            //Creating dictionary for F8 chain code.
            F8_dictionary = new Dictionary<int, KeyValuePair<int, int>>();
            F8_dictionary.Add(0, new KeyValuePair<int, int>(0, 1));
            F8_dictionary.Add(1, new KeyValuePair<int, int>(1, 1)); 
            F8_dictionary.Add(2, new KeyValuePair<int, int>(1, 0));
            F8_dictionary.Add(3, new KeyValuePair<int, int>(1, -1));
            F8_dictionary.Add(4, new KeyValuePair<int, int>(0, -1));
            F8_dictionary.Add(5, new KeyValuePair<int, int>(-1, -1));
            F8_dictionary.Add(6, new KeyValuePair<int, int>(-1, 0));
            F8_dictionary.Add(7, new KeyValuePair<int, int>(-1, 1));

            //Filling af8 matrix 
            

            setF8Code();
            setAF8Code();
            setF4Code();
            setVCCCode();
            set3OTCode();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private Boolean isBorder(int row, int column)
        {
            for (int i = Math.Max(0, row - 1); i < Math.Min(this.height, row + 2); i++)
                for (int j = Math.Max(0, column - 1); j < Math.Min(this.width, column + 2); j++)
                    if (this.image[i, j] == 0)
                        return true;
            return false;
        }

        /// <summary>
        /// Mark perimeter tranforming 1 to 2.
        /// </summary>
        private void MarkPerimeter()
        {
            for(int i = 0; i < this.height; i++)
            {
                for(int j = 0; j < this.width; j++)
                {
                    if(this.image[i, j] == 1 && isBorder(i, j))
                    {
                        this.image[i, j] = 2;
                    }
                }
            }
        }


        private void DeepSearch(int row, int column, int listIndex)
        {
            for(int i = 0; i < 8; i++)
            {
                KeyValuePair<int, int> new_point;
                F8_dictionary.TryGetValue(i, out new_point);
                if (this.image[row + new_point.Key, column + new_point.Value] == 2)
                    DeepSearch(row + new_point.Key, column + new_point.Value, listIndex, i);
            }
        }


        private void DeepSearch(int row, int column, int listIndex, int direction)
        {
            this.F8[listIndex] += direction.ToString();
            this.image[row, column] = 1;

            for (int i = 0; i < 8; i++)
            {
                KeyValuePair<int, int> new_point;
                F8_dictionary.TryGetValue(i, out new_point);
                if (this.image[row + new_point.Key, column + new_point.Value] == 2)
                    DeepSearch(row + new_point.Key, column + new_point.Value, listIndex, i);
            }
        }

        private void setF8Code()
        {
            for (int i = 0; i < this.height; i++)
            {
                for(int j = 0; j < this.width; j++)
                {
                    if(this.image[i, j] == 2)
                    {
                        F8.Add("");
                        //Saving coordenates
                        Coordenates.Add("(" + j + ", " + i + ")");
                        //Finding chain code
                        DeepSearch(i, j, F8.Count - 1);
                    }
                }
            }
        }

        private void setAF8Code()
        {
            foreach(string s in this.F8)
            {
                string new_chain = "";
                for(int i = 0; i < s.Length; i++)
                {
                    new_chain += AF8_matrix[Int32.Parse(s.ElementAt(i).ToString()), Int32.Parse(s.ElementAt((i + 1) % s.Length).ToString())];
                }
                this.AF8.Add(new_chain);
            }
        }

        private void setF4Code()
        {
            foreach(string s in this.F8)
            {
                string new_chain = "";
                for(int i = 0; i < s.Length; i++)
                {
                    new_chain += F4_matrix[Int32.Parse(s.ElementAt(i).ToString()), Int32.Parse(s.ElementAt((i + 1) % s.Length).ToString())];
                }
                this.F4.Add(new_chain);
            }
        }

        private void setVCCCode()
        {
            foreach(string s in this.F4)
            {
                string new_chain = "";
                for(int i = 0; i < s.Length; i++)
                {
                    new_chain += VCC_matrix[Int32.Parse(s.ElementAt(i).ToString()), Int32.Parse(s.ElementAt((i + 1) % s.Length).ToString())];
                }
                this.VCC.Add(new_chain);
            }
        }

        private void set3OTCode()
        {
            foreach(string s in this.F4)
            {
                string new_chain = "";
                int reference = 0, support = 1, current = 2;
                for(int i = 0; i < s.Length + 2; i++)
                {
                    if(s.ElementAt(support % s.Length) == s.ElementAt(current % s.Length))
                    {
                        new_chain += "0";
                        current++;
                        support++;
                    }
                    else
                    {
                        new_chain += _3OT_matrix[Int32.Parse(s.ElementAt(reference % s.Length).ToString()), Int32.Parse(s.ElementAt(current % s.Length).ToString())];
                        reference = support;
                        support = current;
                        current++;
                    }
                }
                this._3OT.Add(new_chain);
            }
        }
    }
}

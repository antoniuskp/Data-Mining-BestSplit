using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data_Mining_BestSplit
{
    public class ClassBestSplit
    {
        private DataGridView data;

        public ClassBestSplit(DataGridView data)
        {
            Data = data;
        }

        public DataGridView Data { get => data; set => data = value; }

        public void countGINI(ListBox listBox)
        {
            int rowCount = Data.Rows.Count;
            int columnCount = Data.Columns.Count;
            List<object> listClassType = new List<object>();

            //mencari GINI parent 
            List<int> listClassTypeCount = new List<int>();
            for (int indexRow = 0; indexRow < rowCount; indexRow++) //mencari classification dari column class
            {
                if (!listClassType.Contains(Data[columnCount - 1, indexRow]))
                {
                    listClassType.Add(Data[columnCount - 1, indexRow]);
                    listClassTypeCount.Add(1);
                }
                else
                {
                    foreach (object classtype in listClassType)
                    {
                        if (classtype == Data[columnCount - 1, indexRow])
                        {
                            listClassTypeCount[listClassType.IndexOf(classtype)]++;
                        }
                    }
                }
            }
            int classCount = listClassTypeCount.Sum();
            double giniParent = 0;
            for (int index = 0; index < classCount; index++)
            {
                giniParent = Math.Pow(listClassTypeCount[index] / classCount, 2.0);
            }
            giniParent = Math.Round(1 - giniParent, 4);
        }
    }
}

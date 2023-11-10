using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
                if (!(listClassType.Contains(Data[columnCount - 1, indexRow].Value)))
                {
                    if (!(Data[columnCount - 1, indexRow].Value is null))
                    {
                        listClassType.Add(Data[columnCount - 1, indexRow].Value);
                        listClassTypeCount.Add(1);
                    }
                }
                else
                {
                    for (int index = 0; index < listClassTypeCount.Count; index++)
                    {
                        if (listClassType[index].Equals(Data[columnCount - 1, indexRow].Value))
                        {
                            listClassTypeCount[index]++;
                        }
                    }
                }
            }
            for(int index = 0; index < listClassTypeCount.Count; index++)
            {
                listBox.Items.Add(listClassTypeCount[index]);
            }
            int classCount = listClassTypeCount.Sum();
            
            double giniParent = 0;
            for (int index = 0; index < listClassType.Count; index++)
            {
                double freq = listClassTypeCount[index] / (double)classCount;
                giniParent += Math.Pow(freq,2);
            }
            giniParent = Math.Round( 1 - giniParent, 4);
            listBox.Items.Add(giniParent);
            //GINI Parent Selesei 
            //Cari GINI Feature
            for(int indexColumn = 0; indexColumn < columnCount - 1; indexColumn ++)
            {
                List<object> listFeatureType = new List<object>();
                for(int indexRow =0; indexRow < rowCount - 1; indexRow++)
                {
                    if (!(listFeatureType.Contains(Data[indexColumn, indexRow].Value)))
                    {
                        listFeatureType.Add(Data[indexColumn, indexRow].Value);
                    }
                }
                foreach(object feature in listFeatureType)
                {
                    listBox.Items.Add(feature);
                }
                listBox.Items.Add("");
            }
        }
    }
}

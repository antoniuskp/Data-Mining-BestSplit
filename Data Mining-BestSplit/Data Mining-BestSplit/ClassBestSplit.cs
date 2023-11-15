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
        int rowCount;
        int columnCount;
        List<object> listClassType;

        public ClassBestSplit(DataGridView data)
        {
            Data = data;
            rowCount = Data.Rows.Count;
            columnCount = Data.Columns.Count;
        }

        public DataGridView Data { get => data; set => data = value; }

        public void countGINI(ListBox listBox)
        {
            listClassType = new List<object>();

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
            int classCount = listClassTypeCount.Sum();

            double giniParent = 0;
            for (int index = 0; index < listClassType.Count; index++)
            {
                double freq = listClassTypeCount[index] / (double)classCount;
                giniParent += Math.Pow(freq, 2);
            }
            giniParent = Math.Round(1 - giniParent, 4);
            listBox.Items.Add("Gini Parent");
            listBox.Items.Add(giniParent);
            //GINI Parent Selesei 
            //Cari GINI Feature
            List<double> listWeightedGiniFeature = new List<double>();
            for (int indexColumn = 0; indexColumn < columnCount - 1; indexColumn++)
            {
                int cekNilai;
                if (int.TryParse(Data[indexColumn, 0].Value.ToString(), out cekNilai) == true)
                {
                    listBox.Items.Add("Continuous Gini Mulai");
                    List<double> listGini = ContinuousGini();
                    foreach(double g in listGini)
                    {
                        listBox.Items.Add(g);
                    }
                    listBox.Items.Add("Continuous Gini Selesai");
                }
                else
                {
                    List<object> listFeatureType = new List<object>();
                    double weightedGiniFeature = 0;
                    for (int indexRow = 0; indexRow < rowCount - 1; indexRow++)
                    {
                        if (!(listFeatureType.Contains(Data[indexColumn, indexRow].Value)))
                        {
                            listFeatureType.Add(Data[indexColumn, indexRow].Value);
                        }
                    }
                    foreach (object type in listFeatureType)
                    {
                        listClassTypeCount.Clear();
                        foreach (object classType in listClassType)
                        {
                            listClassTypeCount.Add(0);
                            for (int indexRow = 0; indexRow < rowCount - 1; indexRow++)
                            {
                                if (classType.Equals(Data[Data.ColumnCount - 1, indexRow].Value) && Data[indexColumn, indexRow].Value.Equals(type))
                                {
                                    listClassTypeCount[listClassType.IndexOf(classType)]++;
                                }
                            }
                        }
                        classCount = listClassTypeCount.Sum();
                        double giniFeature = 0;
                        int jumlahClass = Data.Rows.Count - 1;
                        for (int index = 0; index < listClassType.Count; index++)
                        {
                            double freq = listClassTypeCount[index] / (double)classCount;
                            giniFeature += Math.Pow(freq, 2);
                        }
                        giniFeature = Math.Round(1 - giniFeature, 4);
                        weightedGiniFeature += giniFeature * classCount / jumlahClass;
                    }
                    weightedGiniFeature = Math.Round(weightedGiniFeature, 4);
                    listBox.Items.Add("Weighted Gini");
                    listBox.Items.Add(weightedGiniFeature);
                    listWeightedGiniFeature.Add(weightedGiniFeature);
                }
                double minGiniFeature = listWeightedGiniFeature.Min();
                int indexMinGini = listWeightedGiniFeature.IndexOf(minGiniFeature);
                string bestSplit = Data.Columns[indexMinGini].Name;
                listBox.Items.Add(bestSplit);
            }
        }
        public List<double> ContinuousGini()
        {
            List<double> listData = new List<double>(); //list angka2 di feat
            List<double> listDataMiddle = new List<double>(); //list angka2 setelah dibagi 2
            int columnIndex = 1;
            for (int i = 0; i < rowCount - 1; i++) //memasukkan ke list data
            {
                listData.Add((double)Data[columnIndex, i].Value);
            }

            listData.Sort();

            for (int i = 0; i < listData.Count + 1; i++) //memasukkan ke listDataMiddle
            {
                double nilaiTengah = (listData[i] + listData[i + 1]) / 2;
                listDataMiddle.Add(nilaiTengah);
            }

            List<double> listGini = new List<double>(); //list Gini yang didapat
            foreach (double number in listDataMiddle)
            {
                List<int> itemLebihDari = new List<int>();
                List<int> itemKurangDari = new List<int>();
                int kurangDari = 0;
                int lebihDari = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    if ((double)Data[columnIndex, i].Value <= number)
                    {
                        kurangDari++;
                    }
                    else if ((double)Data[columnIndex, i].Value > number)
                    {
                        lebihDari++;
                    }
                }
                foreach (object type in listClassType)
                {
                    int inClassLebih = 0;
                    int inClassKurang = 0;
                    for (int i = 0; i < rowCount; i++)
                    {
                        if ((double)Data[columnIndex, i].Value > number && Data[columnCount - 1, i].Value == type)
                        {
                            inClassLebih++;
                        }
                        else if ((double)Data[columnIndex, i].Value <= number && Data[columnCount - 1, i].Value == type)
                        {
                            inClassKurang++;
                        }
                    }
                    itemKurangDari.Add(inClassKurang);
                    itemLebihDari.Add(inClassLebih);
                }
                double giniKecilKurang = 1;
                foreach (int angka in itemKurangDari)
                {
                    giniKecilKurang -= Math.Pow((angka / kurangDari), 2);
                }
                double giniKecilLebih = 1;
                foreach (int angka in itemLebihDari)
                {
                    giniKecilLebih -= Math.Pow((angka / lebihDari), 2);
                }
                double gini = (giniKecilKurang * kurangDari / (kurangDari + lebihDari)) + (giniKecilLebih * lebihDari / (kurangDari + lebihDari));
                listGini.Add(gini);
            }
            return listGini;
        }
    }
}

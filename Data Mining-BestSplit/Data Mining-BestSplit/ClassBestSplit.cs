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
            for (int indexColumn = 0; indexColumn < columnCount-1; indexColumn++)
            {
                string isi = Data[indexColumn, 0].Value.ToString().Substring(0,Data[indexColumn, 0].Value.ToString().Length - 1);
                if (int.TryParse(isi, out int cekNilai))
                {
                    listBox.Items.Add("Continuous Gini Mulai");
                    List<double> listGini = ContinuousGini(indexColumn);
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
        public List<double> ContinuousGini(int indexColumn)
        {
            List<double> listGini = new List<double>(); //list Gini yang didapat
            List<double> listData = new List<double>(); //list angka2 di feat
            List<double> listDataMiddle = new List<double>(); //list angka2 setelah dibagi 2
            for (int i = 0; i < rowCount - 1; i++) //memasukkan ke list data
            {
                if (Data[indexColumn, i].Value.ToString().EndsWith("K"))
                {
                    string newString = Data[indexColumn, i].Value.ToString().Substring(0, Data[indexColumn, i].Value.ToString().Length - 1);
                    listData.Add(double.Parse(newString));
                }
                else
                {
/*                    listData.Add(double.Parse(Data[indexColumn, i].Value.ToString()));
*/                }   
            }

            listData.Sort();

            for (int i = 0; i < listData.Count - 1; i++) //memasukkan ke listDataMiddle
            {
                double nilaiTengah = (listData[i] + listData[i + 1]) / 2;
                listDataMiddle.Add(nilaiTengah);
            }

            foreach (double number in listDataMiddle)
            {
                List<double> itemKurangDari = new List<double>();
                List<double> itemLebihDari = new List<double>();
                int kurangDari = 0; //ada berapa angka yang kurang dari
                int lebihDari = 0; // ada berapa angka yang lebih dari
                foreach(double data in listData)
                {
                    if(data<=number)
                    {
                        kurangDari++;
                    }
                    else if(data>number)
                    {
                        lebihDari++;
                    }
                }

                foreach (object type in listClassType)
                {
                    double inClassLebih = 0;
                    double inClassKurang = 0;
                    for (int i = 0; i < rowCount-1; i++)
                    {
                        double angka = double.Parse(Data[indexColumn, i].Value.ToString().Substring(0, Data[indexColumn, i].Value.ToString().Length - 1));
                        object tipe = Data[columnCount - 1, i].Value;
                        if ((angka<= number) && 
                            (tipe.Equals(type)))
                        {
                            inClassKurang++;
                        }
                        else if ((angka > number) && 
                            (tipe.Equals(type)))
                        {
                            inClassLebih++;
                        }
                    }
                    itemKurangDari.Add(inClassKurang);
                    itemLebihDari.Add(inClassLebih);
                }
                double giniKecilKurang = 0;
                foreach (int angka in itemKurangDari)
                {
                    giniKecilKurang += Math.Pow((angka / (double)kurangDari), 2);
                }
                giniKecilKurang = Math.Round(1 - giniKecilKurang, 4);
                double giniKecilLebih = 0;
                foreach (int angka in itemLebihDari)
                {
                    giniKecilLebih += Math.Pow((angka / (double)lebihDari), 2);
                }
                giniKecilLebih = Math.Round(1 - giniKecilLebih, 4);
                double gini = (giniKecilKurang * kurangDari / (double)(kurangDari + lebihDari)) + (giniKecilLebih * lebihDari / (double)(kurangDari + lebihDari));
                listGini.Add(gini);
            }
            return listGini;
        }
    }
}

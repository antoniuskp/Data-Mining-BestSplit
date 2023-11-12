﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data_Mining_BestSplit
{
    public class ClassBestSplitContinuous
    {
        private DataGridView data;

        public ClassBestSplitContinuous(DataGridView data)
        {
            Data = data;
        }

        public DataGridView Data { get => data; set => data = value; }

        public void countGini(ListBox listBox)
        {
            int rowCount = Data.Rows.Count;
            int columnCount = Data.Columns.Count;
            List<object> listClassType = new List<object>();
            //
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
            for (int index = 0; index < listClassTypeCount.Count; index++)
            {
                listBox.Items.Add(listClassTypeCount[index]);
            }
            int classCount = listClassTypeCount.Sum();

            double giniParent = 0;
            for (int index = 0; index < listClassType.Count; index++)
            {
                double freq = listClassTypeCount[index] / (double)classCount;
                giniParent += Math.Pow(freq, 2);
            }
            giniParent = Math.Round(1 - giniParent, 4);
            listBox.Items.Add(giniParent);
            //GINI Parent Selesei

            //Continuous Gini
            List<double> listData = new List<double>(); //list angka2 di feat
            List<double> listDataMiddle = new List<double>(); //list angka2 setelah dibagi 2
            int columnIndex = 1;
            for(int i=0; i < rowCount-1; i++) //memasukkan ke list data
            {
                listData.Add((double)Data[columnIndex,i].Value);
            }

            listData.Sort();

            for(int i=0; i<listData.Count+1; i++) //memasukkan ke listDataMiddle
            {
                double nilaiTengah = (listData[i] + listData[i + 1]) / 2;
                listDataMiddle.Add(nilaiTengah);
            }

            List<double> listGini = new List<double>(); //list Gini yang didapat
            foreach(double number in listDataMiddle)
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
        }
    }
}

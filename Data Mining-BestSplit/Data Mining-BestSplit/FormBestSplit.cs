using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data_Mining_BestSplit
{
    public partial class FormBestSplit : Form
    {
        public FormBestSplit()
        {
            InitializeComponent();
        }

        private void FormBestSplit_Load(object sender, EventArgs e)
        {

        }

        private void buttonBuka_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files|*.csv|All Files|*.*";
            openFileDialog.Title = "Select a CSV File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog.FileName;

                // mengecek type file harus .csv
                if (IsCsvFile(selectedFileName))
                {
                    MessageBox.Show("Valid CSV file selected: " + selectedFileName, "Success");
                }
                else
                {
                    MessageBox.Show("Please select a valid CSV file.", "Error");
                }
                string csvFilePath = openFileDialog.FileName;

                // masukkan inputan dalam bentuk tabel
                DataTable dataTable = LoadCsvFile(csvFilePath);
                dataGridViewBest.DataSource = dataTable;
            }
            ClassBestSplit classBestSplit = new ClassBestSplit(dataGridViewBest);
            classBestSplit.countGINI(listBoxBest);
        }
        private DataTable LoadCsvFile(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    //baris pertama adalah header tabel
                    string[] headers = reader.ReadLine().Split(',');

                    //Buat kolom di dataTable berdasarkan header
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header);
                    }

                    //Baca dan tambah data
                    while (!reader.EndOfStream)
                    {
                        string[] rows = reader.ReadLine().Split(',');
                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dataRow[i] = rows[i];
                        }
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CSV file: " + ex.Message, "Error");
            }

            return dataTable;
        }
        private bool IsCsvFile(string fileName)
        {
            // mengecek type file harus .csv
            return fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
        }

        private void comboBoxDistance_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBoxBest_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

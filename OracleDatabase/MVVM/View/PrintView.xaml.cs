
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Document = iTextSharp.text.Document;
using Path = System.IO.Path;
using PdfDocument = IronPdf.PdfDocument;

namespace OracleDatabase.MVVM.View
{
    /// <summary>
    /// Interaction logic for PrintView.xaml
    /// </summary>
    public partial class PrintView : Window
    {
        public DataSet data = new DataSet();
        public PrintView(DataSet dataset)
        {
            InitializeComponent();
            data = dataset;
            PrintGrid.ItemsSource = dataset.Tables[0].DefaultView;

            var printViewModel = new PrintViewModel(PrintGrid);
            DataContext = printViewModel;
        }

        private void SaveAsPDFButton_Click(object sender, RoutedEventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePathOnDesktop = Path.Combine(desktopPath, "Report.pdf");
            SaveAsPDF(filePathOnDesktop);
            MessageBox.Show("Report Created Successfully\nFile Is Saved On Desktop");
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

            SaveAsPDF(@"c:\output.pdf");

            PrintDialog printDialog = new PrintDialog();

            PdfDocument PDF = PdfDocument.FromFile(@"C:\output.pdf");

            if (printDialog.ShowDialog() == true)
            {
                PDF.Print(printDialog.PrintQueue.Name);
            }

            if (File.Exists(@"c:\output.pdf"))
            {
                File.Delete(@"c:\output.pdf");
            }
        }

        private void MyDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader);
            e.Column.Width = new DataGridLength(e.Column.ActualWidth + 150, DataGridLengthUnitType.Pixel);
        }

        private void SaveAsPDF(string path)
        {
            DataTable dt = new DataTable();
            dt = data.Tables[0];

            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            document.Open();

            Font font5 = FontFactory.GetFont(FontFactory.HELVETICA, 5);

            PdfPTable table = new PdfPTable(dt.Columns.Count);

            Array floatArray = Array.CreateInstance(typeof(float), dt.Columns.Count);

            for (int i = 0; i < dt.Columns.Count; i++)
                floatArray.SetValue(4f, i);

            table.SetWidths((float[])floatArray);

            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = dt.Columns.Count;

            foreach (DataColumn c in dt.Columns)
            {
                table.AddCell(new Phrase(c.ColumnName, font5));
            }

            foreach (DataRow r in dt.Rows)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        table.AddCell(new Phrase(r[i].ToString(), font5));
                    }
                }
            }
            document.Add(table);
            document.Close();
        }
    }
}

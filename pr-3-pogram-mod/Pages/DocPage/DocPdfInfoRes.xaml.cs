using pr_3_pogram_mod.bd;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pr_3_pogram_mod.Pages.DocPage
{
    /// <summary>
    /// Логика взаимодействия для DocPdfInfoRes.xaml
    /// </summary>
    public partial class DocPdfInfoRes : Window
    {
        public users User { get; set; }
        public residents Resident { get; set; }


        public DocPdfInfoRes(users user, residents resident)
        {
            InitializeComponent();
            this.DataContext = this;

            User = user;
            Resident = resident;

            //List<service_requests> list = new List<service_requests>().ToList();
            var list = bdMod.GetContext(true).service_requests.ToList();
            FillTable(list);
        }

        private void FillTable(List<service_requests> list)
        {
            int rowIndex = 0;
            var rowGroup = RequestsTable.RowGroups[0];

            foreach (var request in list)
            {
                if (request.resident_id != Resident.id)
                    continue;

                var row = new TableRow();

                row.Cells.Add(new TableCell(new Paragraph(new Run(request.description ?? ""))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black });
                row.Cells.Add(new TableCell(new Paragraph(new Run($"{ request.employees.surname ?? ""} {request.employees.name ?? ""}"))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black });
                row.Cells.Add(new TableCell(new Paragraph(new Run(request.service_statuses.status_name ?? ""))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black });
                row.Cells.Add(new TableCell(new Paragraph(new Run(request.created_at?.ToString("dd.MM.yyyy") ?? ""))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black });
                row.Cells.Add(new TableCell(new Paragraph(new Run(request.assigned_at?.ToString("dd.MM.yyyy") ?? ""))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black });
                row.Cells.Add(new TableCell(new Paragraph(new Run(request.completed_at?.ToString("dd.MM.yyyy") ?? "В процессе"))) { BorderThickness = new Thickness(1), BorderBrush = Brushes.Black });

                if (rowIndex % 2 == 0)
                    row.Background = Brushes.White;
                else
                    row.Background = Brushes.LightGray;


                rowIndex++;
                rowGroup.Rows.Add(row);
            }
        }

        private void btInPdf_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = flowDoc.Document;

            if (doc == null)
            {
                MessageBox.Show("Документ не найден.");
                return;
            }

            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource idpSource = doc;
                printDialog.PrintDocument(idpSource.DocumentPaginator, $"Заявки {Resident.surname} {Resident.name}");

                this.Close();   
            }
        }
    }
}

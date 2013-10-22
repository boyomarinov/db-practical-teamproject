using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Windows;

namespace EntityFramework.Client
{
    class GetPdfTable
    {

        public static void GetTable(TelerikAcademyEntities db)
        {
            Document myDoc = new Document(PageSize.A4.Rotate());
            PdfWriter.GetInstance(myDoc, new FileStream("table.pdf", FileMode.Create));
            myDoc.Open(); 
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            Font font = new Font(Font.FontFamily.TIMES_ROMAN, 20f);
            font.SetStyle("bold");
            PdfPCell cell = new PdfPCell(new Phrase("Departments",font));
            cell.Colspan = 3;

           // cell.Indent = 50;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5;
            cell.BackgroundColor = new BaseColor(229, 228, 226);
            table.AddCell(cell);
            using (db)
            {
                foreach (Department dep in db.Departments.Include("Employee"))
                {
                    Console.WriteLine("Id={0} DepartmentName={1} ManagerName={2}", dep.DepartmentID, dep.Name, dep.Employee.FirstName + " " +
                        dep.Employee.LastName);
                    string depId = dep.DepartmentID.ToString();
                    string depName = dep.Name;
                    string depManagerName = dep.Employee.FirstName + " " + dep.Employee.LastName;
                    table.AddCell(depId);
                    table.AddCell(depName);
                    table.AddCell(depManagerName);
                }
              
                myDoc.Add(table);
            }
            myDoc.Close();
        }
    }
}

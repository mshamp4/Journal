using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace Journal
{
    public class JournalLayout 
    {
        private string name;
        
        private string company;

        private string dateSubmitted;
        
        private string journalPeriod;
        
        private int totalhoursWorked;

        private string[] journalDates = {"FROM: 5/10/2021 TO: 5/21/2021", "FROM: 5/24/2021 TO: 6/4/2021", "FROM: 6/7/2021 TO: 6/18/2021",
        "FROM: 6/21/2021 TO: 7/2/2021", "FROM: 7/5/2021 TO: 7/16/2021", "FROM: 7/19/2021 TO: 7/30/2021", "FROM: 8/2/2021 TO: 8/6/2021"};

        private const string path = @"Journal1.txt";

        public JournalLayout(int period)
        {
            name = "Matthew Shampine";
            company = "Rock Central LLC";
            dateSubmitted = "../../....";
            journalPeriod = journalDates[period];
            totalhoursWorked = 0;
        }

        public void CreateTask(string description)
        {
            if (!File.Exists(path))
            {
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine(ToString());
                tw.WriteLine(System.DateTime.Today.ToString("D"));
                tw.WriteLine("---------------------------------------------------------------------------");
                tw.WriteLine(description);
                tw.Close();
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(description + "\r\n");
                    sw.Close();
                }
            }
            // need to figure out what to return from this method
        }

        public string GetCurrentPeriod()
        {
            if (!File.Exists(path))
            {
                return "Error, no current information on file.";
            }
            using (StreamReader sr = File.OpenText(path))
            {
                string period = sr.ReadToEnd();
                sr.Close();
                return period;
            }
        }

        public void CreatePDF()
        {
            var doc = new PdfDocument();
            PdfPage page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            var font = new XFont("Times New Roman", 12, XFontStyle.Regular);
            gfx.DrawString("To:do// Work in progess", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.TopLeft);
            doc.Save("journal1.pdf");
        }

        public override string ToString()
        {
            return name + "\r" + company + "\r" + "Journal date: " + dateSubmitted + "\r" + "Journal period - " + 
            journalPeriod + "\r" + "Total hours worked this period: " + totalhoursWorked + "\r";
        }
    }
}

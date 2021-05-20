using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using Newtonsoft.Json;

namespace Journal
{
    public class JournalLayout 
    {
        private string name;
        
        private string company;

        private string dateSubmitted;
        
        private string journalPeriod;
        
        private int totalhoursWorked;

        private string[] journalDates;
        
        private const string path = @"Journal1.txt";

        private const string jsonPath = "user.json";

        public JournalLayout(int period)
        {
            using (System.IO.StreamReader sr = File.OpenText(jsonPath))
            {
                string jsonTxt = sr.ReadToEnd();
                JournalFormat item = JsonConvert.DeserializeObject<JournalFormat>(jsonTxt);

                name = item.Name;
                company = item.Company;
                dateSubmitted = item.DateSubmitted;
                journalPeriod = item.JournalPeriod;
                totalhoursWorked = item.TotalhoursWorked;
                journalDates = item.JournalDates;

                sr.Close();
            }
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
            doc.Info.Title = "Journal 6";
            var gfx = XGraphics.FromPdfPage(page);
            //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            var font = new XFont("Times New Roman", 12, XFontStyle.Regular);
            // loop to handle
            using (StringReader sr = new StringReader(GetCurrentPeriod()))
            {
                int linePos = 25;
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    gfx.DrawString(line, font, XBrushes.Black, new XRect(50, linePos, page.Width, page.Height), XStringFormats.TopLeft);
                    linePos += 12;
                }
                sr.Close();
            }
            doc.Save("journal1.pdf");
            //Process.Start(doc);
        }

        public override string ToString()
        {
            return name + "\r\n" + company + "\r\n" + "Journal date: " + dateSubmitted + "\r\n" + "Journal period - " + 
            journalPeriod + "\r\n" + "Total hours worked this period: " + totalhoursWorked + "\r\n";
        }
    }
}

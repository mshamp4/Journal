using System;
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

        public string lastLoginTime;

        private string date;

        private string[] journalDates;

        public bool isToday;

        private const string JOURNAL_PATH = @"journal3.txt";

        private const string USER_INFO_PATH = @"user1.json";

        public JournalLayout(int period = 0)
        {
            bool verify = CheckUserData();
            isToday = false;

            if (verify)
            {
                using (System.IO.StreamReader sr = File.OpenText(USER_INFO_PATH))
                {
                    string jsonTxt = sr.ReadToEnd();
                    JournalFormat item = JsonConvert.DeserializeObject<JournalFormat>(jsonTxt);

                    name = item.Name;
                    company = item.Company;
                    dateSubmitted = item.DateSubmitted;
                    journalPeriod = item.JournalPeriod;
                    totalhoursWorked = item.TotalhoursWorked;
                    lastLoginTime = item.LoginTime;
                    item.LoginTime = DateTime.Now.ToString("hh:mm:ss tt");
                    date = item.Date;
                    isToday = CheckDate(date);
                    item.Date = System.DateTime.Today.ToString("D");
                    journalDates = item.JournalDates;

                    sr.Close();

                    string defaultData = JsonConvert.SerializeObject(item, Formatting.Indented);
                    TextWriter tw = new StreamWriter(USER_INFO_PATH);
                    tw.Write(defaultData);
                    tw.Close();
                }
            }
        }
        public bool CheckUserData()
        {
            if (!File.Exists(USER_INFO_PATH))
            {
                name = "Your name";
                company = "Company";
                dateSubmitted = "../../....";
                journalPeriod = "FROM: 5/10/2021 TO: 5/21/2021";
                totalhoursWorked = 0;
                lastLoginTime = DateTime.Now.ToString("hh:mm:ss tt");
                date = System.DateTime.Today.ToString("D");
                journalDates = new String[] {"FROM: 5/10/2021 TO: 5/21/2021", "FROM: 5/24/2021 TO: 6/4/2021", "FROM: 6/7/2021 TO: 6/18/2021",
                        "FROM: 6/21/2021 TO: 7/2/2021", "FROM: 7/5/2021 TO: 7/16/2021", "FROM: 7/19/2021 TO: 7/30/2021", "FROM: 8/2/2021 TO: 8/6/2021"};

                WriteDefaultJsonFile();
                return false;
            }

            return true;
        }

        private void WriteDefaultJsonFile()
        {
            JournalFormat item = new JournalFormat()
            {
                Name = "Your name",
                Company = "Company",
                DateSubmitted = "../../....",
                JournalPeriod = "FROM: 5/10/2021 TO: 5/21/2021",
                TotalhoursWorked = 0,
                LoginTime = DateTime.Now.ToString("hh:mm:ss tt"),
                Date = System.DateTime.Today.ToString("D"),
                JournalDates = new String[] {"FROM: 5/10/2021 TO: 5/21/2021", "FROM: 5/24/2021 TO: 6/4/2021", "FROM: 6/7/2021 TO: 6/18/2021",
                        "FROM: 6/21/2021 TO: 7/2/2021", "FROM: 7/5/2021 TO: 7/16/2021", "FROM: 7/19/2021 TO: 7/30/2021", "FROM: 8/2/2021 TO: 8/6/2021"}
            };

            string defaultData = JsonConvert.SerializeObject(item, Formatting.Indented);
            TextWriter tw = new StreamWriter(USER_INFO_PATH);
            tw.Write(defaultData);
            tw.Close();
        }

        public bool CheckDate(string date)
        {
            if (date.Equals(System.DateTime.Today.ToString("D")))
            {
                return true;
            }

            return false;
        }

        public void CreateTask(string description)
        {
            if (!File.Exists(JOURNAL_PATH))
            {
                TextWriter tw = new StreamWriter(JOURNAL_PATH);
                tw.WriteLine(ToString());
                tw.WriteLine(System.DateTime.Today.ToString("D"));
                tw.WriteLine("---------------------------------------------------------------------------");
                tw.WriteLine(description);
                tw.Close();
            }
            else
            {
                using (StreamWriter sw = File.AppendText(JOURNAL_PATH))
                {
                    sw.WriteLine(description + "\n");
                    sw.Close();
                }
            }
            //TODO: need to figure out what to return from this method.
        }

        public string GetHeader()
        {
            string header = System.DateTime.Today.ToString("D") +
                        "\n---------------------------------------------------------------------------\n" +
                        DateTime.Now.ToString("hh:mm tt\n");

            if (!isToday)
            {
                isToday = true;
                return header;
            }
            
            return header = (string)DateTime.Now.ToString("hh:mm tt\n");
        }

        public string GetCurrentPeriod()
        {
            if (!File.Exists(JOURNAL_PATH))
            {
                return "Error, no current information on file.";
            }
            using (StreamReader sr = File.OpenText(JOURNAL_PATH))
            {
                string period = sr.ReadToEnd();
                sr.Close();
                return period;
            }
        }

        public PdfPage[] PDFSetupPages(PdfDocument document)
        {
            int numLines = GetNumLines();
            decimal spacing = (12 * numLines);
            int totalPages = (int)Math.Ceiling(spacing / 777);

            PdfPage[] pages = new PdfPage[totalPages];
            for (int i = 0; i < totalPages; i++)
            {
                pages[i] = document.AddPage();
            }

            return pages;
        }

        public XGraphics[] GFXSetup(PdfPage[] pages)
        {
            int pageLength = pages.Length;
            XGraphics[] graphics = new XGraphics[pageLength];
            for (int i = 0; i < pageLength; i++)
            {
                graphics[i] = XGraphics.FromPdfPage(pages[i]);
            }

            return graphics;
        }

        protected int GetNumLines()
        {
            if (!File.Exists(JOURNAL_PATH))
            {
                return 0;
            }
            return File.ReadAllLines(JOURNAL_PATH).Length;
        }

        public void CreatePDF()
        {
            var doc = new PdfDocument();

            PdfPage[] pages = PDFSetupPages(doc);
            XGraphics[] graphics = GFXSetup(pages);
            //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            var font = new XFont("Courier New", 12, XFontStyle.Regular);

            using (StringReader sr = new StringReader(GetCurrentPeriod()))
            {
                int linePos = 45;
                string line = null; //Better to always init variables before using
                int count = 0;
                PdfPage currentPage = pages[count];
                XGraphics currentGFX = graphics[count];
                while ((line = sr.ReadLine()) != null)
                {
                    if (linePos >= 777)
                    {
                        count += 1;
                        currentPage = pages[count];
                        currentGFX = graphics[count];
                        linePos = 45;
                    }

                    currentGFX.DrawString(
                        line,
                        font,
                        XBrushes.Black,
                        new XRect(35, linePos, currentPage.Width, currentPage.Height),
                        XStringFormats.TopLeft
                    );

                    linePos += 12;
                }

                sr.Close();
            }

            doc.Save("journal3.pdf");
            
            string execPath = @"/c " + JOURNAL_PATH;
            Console.WriteLine(execPath);
            System.Diagnostics.Process.Start(@"cmd.exe", @"/c journal3.pdf");
            //TODO: Fix bug where after closing pdf focus doesnt shift back to TUI.
            //Environment.Exit(0);
        }

        public override string ToString()
        {
            return name + "\r\n" + company + "\r\n" + "Journal date: " + dateSubmitted + "\r\n" + "Journal period - " +
            journalPeriod + "\r\n" + "Total hours worked this period: " + totalhoursWorked + "\r\n";
        }
    }
}

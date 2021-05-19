﻿using System;
using System.IO;
using Terminal.Gui;

namespace Journal
{
    public class JournalDriver 
    {
        private string configPath = @"config.txt";
        public string loginTime = "";
        public string defaultHeader;
        public bool isDate;

        public JournalDriver()
        {
            defaultHeader = GetHeader();
            isDate = VerifyDate();
        }

        public bool VerifyDate()
        {
            loginTime = DateTime.Now.ToString("hh:mm:ss tt");
            bool date = false;
            if (!File.Exists(configPath))
            {
                TextWriter tw = new StreamWriter(configPath);
                tw.WriteLine(DateTime.Now.ToString("hh:mm:ss tt"));
                tw.WriteLine(System.DateTime.Today.ToString("D"));
                tw.Close();
                return date;
            }
            else
            {
                using (StreamReader sr = File.OpenText(configPath))
                {
                    loginTime = sr.ReadLine();
                    string pastDate = sr.ReadLine();
                    if (pastDate.Equals(System.DateTime.Today.ToString("D")))
                    {
                        date = true;
                    }
                    sr.Close();
                }

                TextWriter newTxt = new StreamWriter(configPath);
                newTxt.WriteLine(DateTime.Now.ToString("hh:mm:ss tt"));
                newTxt.WriteLine(System.DateTime.Today.ToString("D"));
                newTxt.Close();
            }
            return date;
        }
        public string GetHeader() 
        {
            string header = System.DateTime.Today.ToString("D") + 
                        "\n---------------------------------------------------------------------------\n" +
                        DateTime.Now.ToString("hh:mm tt\n");
            if (!File.Exists(configPath))
            {
                return header;
            }
            else 
            {
                using (StreamReader sr = File.OpenText(configPath))
                {
                    sr.ReadLine();
                    string pastDate = sr.ReadLine();
                    if (pastDate.Equals(System.DateTime.Today.ToString("D")))
                    {
                        header = DateTime.Now.ToString("hh:mm tt\n");
                    }
                    sr.Close();
                }
            }
            return header;
        }
    }
    static class Program
    {
        static void Main(string[] args)
        {
            bool taskView = false;
            bool journalView = false;

            JournalLayout journal = new JournalLayout(0);
            JournalDriver jInfo = new JournalDriver();
            
            Application.Init();
            var top = Application.Top;
            
            var mainWin = new FrameView("Journal") 
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = Colors.Dialog
            };
            
            var loginLbl = new Label(jInfo.loginTime) 
            {
                X = Pos.Percent(38),
                Y = 0,
            };
            mainWin.Add(loginLbl);

            var dateLbl = new Label(System.DateTime.Today.ToString("D")) 
            {
                X = Pos.Percent(75),
                Y = 0,
            };
            mainWin.Add(dateLbl);

            var menuWin = new FrameView()
            {
                X = 1,
                Y = 1,
                Width = Dim.Percent(97),
                Height = Dim.Percent(15),
                ColorScheme = Colors.Menu
            };
  
            var taskWin = new FrameView("Add task")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(menuWin) + 1,
                Width = 80,
                Height = Dim.Percent(60),
                ColorScheme = Colors.Menu
            };

            var jWin = new FrameView("Journal")
            {
                X = Pos.Center(),
                Y = Pos.Bottom(menuWin) + 1,
                Width = 80,
                Height = Dim.Percent(74),
                ColorScheme = Colors.Menu
            };

            var journalBar = new StatusBar (new StatusItem []
            {
                new StatusItem(Key.CtrlMask | Key.O, "~^O~ Open", () => Application.RequestStop ()),
				new StatusItem(Key.CtrlMask | Key.P, "~^P~ Export PDF", () => Application.RequestStop ())
			});

            var journalField = new TextView() 
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Percent(100),
            };
 
            //journalField.LoadFile(@"C:\Users\msham\Desktop\490\Journal\Journal1.txt");
            //journalField.CloseFile();
            journalField.Text = journal.GetCurrentPeriod();
            journalField.ReadOnly = true;
            jWin.Add(journalField);

            var taskField = new TextView() 
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = 10
            };

            taskField.Text = jInfo.defaultHeader;
            taskField.WordWrap = true;
            taskWin.Add(taskField);
            
            var submitBtn = new Button("Submit")
            {
                X = Pos.Center(),
                Y = Pos.Percent(99)
            };

            var taskBtn = new Button("New task entry") 
            {
                X = Pos.Percent(5),
                Y = Pos.Top(mainWin)
            };

            Point cursorPos = new Point(0, 3);
            taskBtn.Clicked += () => 
            {
                if (journalView)
                {
                    mainWin.Remove(jWin);
                    mainWin.Remove(journalBar);
                    journalView = false;
                }
                if (taskView)
                {
                    mainWin.Remove(taskWin);
                    taskView = false;
                }
                mainWin.Add(taskWin);
                taskWin.SetFocus();
                taskField.SetFocus();
                taskField.CursorPosition = cursorPos;
                taskField.ScrollTo(0, true);
                taskView = true;
            };

            var journalBtn = new Button("View current journal") 
            {
                X = Pos.Percent(30),
                Y = Pos.Top(mainWin)
            };

            journalBtn.Clicked += () =>
            { 
                if (taskView)
                {
                    mainWin.Remove(taskWin);
                    taskView = false;
                }
                if (journalView)
                {
                    mainWin.Remove(jWin);
                    journalView = false;
                }
                mainWin.Add(jWin);
                jWin.SetFocus();
                journalField.ScrollTo(0, true);
                journalView = true;
                mainWin.Add(journalBar);
            };

            var newBtn = new Button("New")
            {
                X = Pos.Percent(55),
                Y = Pos.Top(mainWin)
            };

            var hoursBtn = new Button("Add hours")
            { 
                X = Pos.Left(journalBtn) + 6,
                Y = Pos.Bottom(journalBtn)
            };
            
            var exitBtn = new Button("Exit") 
            {
                X = Pos.Percent(80),
                Y = Pos.Top(mainWin)
            };
            exitBtn.Clicked += () => { Application.RequestStop (); };

            submitBtn.Clicked += () => 
            {
                var n = MessageBox.Query("Task Complete", "Are you sure you want to submit task?", "Yes", "No"); 
                if (n == 0) 
                {
                    mainWin.Remove(taskWin);
                    menuWin.SetFocus();
                    journalBtn.SetFocus();
                    journal.CreateTask(taskField.Text.ToString());
                    taskField.Text = jInfo.GetHeader();
                    journalField.Text = journal.GetCurrentPeriod();
                }
                else 
                {
                    taskField.SetFocus();
                }
            };

            menuWin.Add(taskBtn);
            menuWin.Add(journalBtn);
            menuWin.Add(newBtn);
            menuWin.Add(hoursBtn);
            menuWin.Add(exitBtn);

            taskWin.Add(submitBtn);

            mainWin.Add(menuWin);
            top.Add(mainWin);

            Application.Run();
        }
    }
}

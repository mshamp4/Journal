# Journal
This is a simple terminal interface (TUI) used for journal logging for my CIS 490 internship class.
This basic TUI can add new journal tasks, view the current journal, and export an already formatted journal
in pdf form that fits the requirements for CIS 490.

### Creating a new journal task:
![Journal add task view](https://github.com/mshamp4/Journal/blob/master/imgs/add_task.PNG?raw=true)

### Viewing current journal:
![View journal](https://github.com/mshamp4/Journal/blob/master/imgs/view_journal.PNG?raw=true)

### To run
After cloning this repo head into the folder and type in command prompt or powershell:
```
dotnet run
```
Currently, this TUI has only been tested on the Windows platform and has not been verified 
on Mac or Linux. However, this application is intended to be cross-platform and should
work after further developmentation.

### Resources
[Terminal.Gui API](https://migueldeicaza.github.io/gui.cs/articles/overview.html)

[PdfSharpCore API](https://github.com/ststeiger/PdfSharpCore)

[JSON Reference](https://www.newtonsoft.com/json)

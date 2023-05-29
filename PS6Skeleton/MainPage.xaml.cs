using Microsoft.UI.Xaml.Controls;
using SS;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.Media.AppBroadcasting;

namespace SpreadsheetGUI;

/// <summary>
/// Example of using a SpreadsheetGUI object
/// </summary>
public partial class MainPage : ContentPage
{
    private string recentFilePath;
    /// <summary>
    /// Constructor for the demo
    /// </summary>
	public MainPage()
    {
        InitializeComponent();

        // This an example of registering a method so that it is notified when
        // an event happens.  The SelectionChanged event is declared with a
        // delegate that specifies that all methods that register with it must
        // take a SpreadsheetGrid as its parameter and return nothing.  So we
        // register the displaySelection method below.
        spreadsheetGrid.SelectionChanged += displaySelection;
        spreadsheetGrid.SetSelection(0, 0);
    }

    /// <summary>
    /// Updates labels which show cell value and cell contents
    /// </summary>
    /// <param name="grid"></param>
    private void displaySelection(SpreadsheetGrid grid)
    {
        spreadsheetGrid.GetSelection(out int col, out int row); //get spreadsheet coordinates

        //updates the label which displays current cell name
        string c = ((char) (col + 65)).ToString();
        selectedCell.Text = c + (row+1);

        //updates the label which displays current cell value
        spreadsheetGrid.GetValue(col, row, out string value);
        selectedCellValue.Text = value;
        spreadsheetGrid.GetContents(col, row, out string contents);
        entry.Text = contents;
        spreadsheetGrid.GetDependents(col, row, out IEnumerable<string> dependencies);
        string dependents = "";
        foreach(string dependent in dependencies) //creates the dependency list
        {
            dependents += dependent+", ";
        }
        DependencyList.Text = dependents;
      
    }

    /// <summary>
    /// If new is clicked, check for data loss, if accepted, create a new spreadsheet
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void NewClicked(Object sender, EventArgs e)
    {
     
        bool answer = true;
        if (spreadsheetGrid.isChanged())
             answer = await DisplayAlert("Potential Data Loss", "Would you like to proceed", "Yes", "No");

        if (answer)
        {
            //create new clear spreadsheet
            spreadsheetGrid.Clear();
            entry.Text = "";
            selectedCellValue.Text = "";
        }
    }

    /// <summary>
    /// If save is clicked, opens a window with a save location for a user to enter, if valid 
    /// location, saves the spreadsheet to given location
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveClicked(Object sender, EventArgs e)
    {
        try
        {
            bool answer = true; //answer should start true
            string filepath = await DisplayPromptAsync("Save as:", "Enter file save location", "Save"); //asking for file path
                if(File.Exists(filepath) && filepath != recentFilePath) //file safety protection
                answer = await DisplayAlert("Overwritting file", "Would you like to proceed", "Yes", "No");
            if (answer)
            {
                recentFilePath = filepath; 
                if (filepath != null)
                    spreadsheetGrid.saveFile(filepath); //if safe save and possibly override file
            }
        }
        catch(Exception exc)
        {
           await DisplayAlert("Fail Saved", exc.Message, "return"); //catch a filesave exception
        }
    }
    /// <summary>
    /// Opens any file as text and prints its contents.
    /// Note the use of async and await, concepts we will learn more about
    /// later this semester.
    /// </summary>
    private async void OpenClicked(Object sender, EventArgs e)
    {

        bool answer = true;
        if (spreadsheetGrid.isChanged())
            answer = await DisplayAlert("Potential Data Loss", "Would you like to proceed", "Yes", "No");
        if (answer)
        {
            try
            {
                FileResult fileResult = await FilePicker.Default.PickAsync(); //finds file
                if (fileResult != null) //if not null
                {
                    System.Diagnostics.Debug.WriteLine("Successfully chose file: " + fileResult.FileName);
                    string fileContents = File.ReadAllText(fileResult.FullPath); //writes to the debugger file
                    System.Diagnostics.Debug.WriteLine("First 100 file chars:\n" + fileContents.Substring(0, 100));
                    spreadsheetGrid.openFile(fileResult.FullPath);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No file selected."); //if no file is selected
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error opening file:"); //catches all errors
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }


    /// <summary>
    /// If entry text is changed, change the display value and contents of the cell and its dependents
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnEntryTextChanged(object sender, EventArgs e)
    {
        try
        {
            spreadsheetGrid.GetSelection(out int col, out int row); //get selection highlight
            spreadsheetGrid.SetValue(col, row, entry.Text); //sets the value
            spreadsheetGrid.GetValue(col, row, out string value); //gets the value to display to label
            selectedCellValue.Text = value;
        }
        catch(Exception exc)
        {
            entry.Text = ""; //entry text
            DisplayAlert("Exception thrown", exc.Message, "return");
        }
    }

    /// <summary>
    /// Displays instructions for how to operate spreadsheet
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InstructionsClicked(object sender, EventArgs e)
    {
        DisplayAlert("Instructions", "To enter a value into a cell, click on the desired cell, go to the editable text box above the spreadsheet, enter desired input, then press enter. \n \n To edit cell contents, select the cell you'd like to edit, " +
            "then go to the editable text box above the spreadsheet, edit text both as desired, press enter \n \n To save your spreadsheet, you must enter the full file path your file \n \n My " +
            "extra feature is a dependency list which shows up next to the entry box to the right when you click on the cell ", "Exit");
    }

}

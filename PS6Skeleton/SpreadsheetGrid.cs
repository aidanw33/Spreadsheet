// Written by Joe Zachary and Travis Martin for CS 3500, September 2011, 2021
using Font = Microsoft.Maui.Graphics.Font;
using SizeF = Microsoft.Maui.Graphics.SizeF;
using PointF = Microsoft.Maui.Graphics.PointF;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using Microsoft.UI.Composition.Interactions;
using System.Numerics;

namespace SS;

/// <summary>
/// The type of delegate used to register for SelectionChanged events
/// </summary>
/// <param name="sender"></param>
public delegate void SelectionChangedHandler(SpreadsheetGrid sender);

/// <summary>
/// A grid that displays a spreadsheet with 26 columns (labeled A-Z) and 99 rows
/// (labeled 1-99).  Each cell on the grid can display a non-editable string.  One 
/// of the cells is always selected (and highlighted).  When the selection changes, a 
/// SelectionChanged event is fired.  Clients can register to be notified of
/// such events.
/// 
/// None of the cells are editable.  They are for display purposes only.
/// </summary>
public class SpreadsheetGrid : ScrollView, IDrawable
{
    /// <summary>
    /// The event used to send notifications of a selection change
    /// </summary>
    public event SelectionChangedHandler SelectionChanged;

    // These constants control the layout of the spreadsheet grid.
    // The height and width measurements are in pixels.
    private const int DATA_COL_WIDTH = 80;
    private const int DATA_ROW_HEIGHT = 20; //20
    private const int LABEL_COL_WIDTH = 30;
    private const int LABEL_ROW_HEIGHT = 30; //30
    private const int PADDING = 4;
    private const int COL_COUNT = 26;
    private const int ROW_COUNT = 99;
    private const int FONT_SIZE = 12;

    // Columns and rows are numbered beginning with 0.  This is the coordinate
    // of the selected cell.
    private int _selectedCol;
    private int _selectedRow;

    // Coordinate of cell in upper-left corner of display
    private int _firstColumn = 0;
    private int _firstRow = 0;

    // Scrollbar positions
    private double _scrollX = 0;
    private double _scrollY = 0;

    // The strings contained by this grid
    private Dictionary<Address, String> _values = new();

    //The contents contained by this grid
    private Dictionary<Address, String> _contents = new();

    // GraphicsView maintains the actual drawing of the grid and listens
    // for click events
    private GraphicsView graphicsView = new();

    //spreadsheet to track dependencies
    private Spreadsheet ss;

    public SpreadsheetGrid()
    {
        BackgroundColor = Colors.White;
        graphicsView.Drawable = this; //makes this class drawable
        graphicsView.HeightRequest = LABEL_ROW_HEIGHT + (ROW_COUNT + 1) * DATA_ROW_HEIGHT; //all set above, dictate height of draw
        graphicsView.WidthRequest = LABEL_COL_WIDTH + (COL_COUNT + 1) * DATA_COL_WIDTH; // also all set above, dictate width of draw
        graphicsView.BackgroundColor = Colors.MediumPurple;
        graphicsView.EndInteraction += OnEndInteraction; //signals to the graphics view, release
        this.Content = graphicsView;
        this.Scrolled += OnScrolled;
        this.Orientation = ScrollOrientation.Both; //allows both horizontal and vertical scroll directions
        ss = new Spreadsheet(isValidVariable, s => s.ToUpper(), "ps6");
    }

    /// <summary>
    /// Clears the display.
    /// </summary>
    public void Clear()
    {
        _values.Clear(); //clears values
        _contents.Clear(); //clears contents
        ss = new Spreadsheet(isValidVariable, s => s.ToUpper(), "ps6"); //reset spreadsheet
        Invalidate(); //informs canvas to redraw itself
    }

    /// <summary>
    /// If the zero-based column and row are in range, sets the value of that
    /// cell and returns true.  Otherwise, returns false.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool SetValue(int col, int row, string c)
    {
        if (InvalidAddress(col, row))
        {
            return false; //if given an invalid address, return false
        }
        string cellName = getCellName(col, row);
        Address a = new Address(col, row); // we know address is now valid, create one
        if (c == null || c == "")
        {
            _contents.Remove(a); //sets the contents map
            _values.Remove(a); //if given set input is null, or empty string, remove the address
            IList<string> dependents = ss.SetContentsOfCell(cellName, ""); // set contents of cell back to empty
            foreach (string dependent in dependents)
            {
                Address depA = findAddress(dependent);
                _values[depA] = cellValueSpreadsheet(dependent);
                _contents[depA] = cellContentsSpreadsheet(dependent);
            }
        }
       
        else
        {
            
            IList<string> dependents = ss.SetContentsOfCell(cellName, c);//set to spreadsheet
            foreach(string dependent in dependents)
            {
                Address depA = findAddress(dependent);
                _values[depA] = cellValueSpreadsheet(dependent);
                _contents[depA] = cellContentsSpreadsheet(dependent);
            }
            string value = cellValueSpreadsheet(cellName);
            _values[a] = value; //else add given input to our cells at the address
            _contents[a] = c; //sets contents of map
        }
        Invalidate(); //tell canvas to redraw itself
        return true;
    }

    /// <summary>
    /// If the zero-based column and row are in range, assigns the value
    /// of that cell to the out parameter and returns true.  Otherwise,
    /// returns false.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool GetValue(int col, int row, out string c)
    {
        if (InvalidAddress(col, row)) //if address doesn't exist, out null, return false
        {
            c = null;
            return false;
        }
        if (!_values.TryGetValue(new Address(col, row), out c)) //if we get the value, sets c, if we can't get value, it's set to ""
        {
            c = "";
        }
        return true;
    }

    /// <summary>
    /// If the zero-based column and row are in range, assigns the contents
    /// of that cell to the out parameter and returns true.  Otherwise,
    /// returns false.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public bool GetContents(int col, int row, out string c)
    {
        if (InvalidAddress(col, row)) //if address doesn't exist, out null, return false
        {
            c = null;
            return false;
        }
        if (!_contents.TryGetValue(new Address(col, row), out c)) //if we get the value, sets c, if we can't get value, it's set to ""
        {
            c = "";
        }
        return true;
    }

    public void openFile(string filePath)
    {
        Spreadsheet newSS = new Spreadsheet(filePath, isValidVariable, s => s.ToUpper(), "ps6");
        ss = newSS;
        fillValuesContents();
        Invalidate();
        
    }

    public void saveFile(string fileName)
    {
        ss.Save(fileName);
    }
    /// <summary>
    /// If the zero-based column and row are in range, uses them to set
    /// the current selection and returns true.  Otherwise, returns false.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public bool SetSelection(int col, int row)
    {
        if (InvalidAddress(col, row)) //if address is invalid, return false
        {
            return false;
        }
        _selectedCol = col; //reset selected col and row
        _selectedRow = row;
        Invalidate(); //refresh canvas
        return true; //return true
    }

    /// <summary>
    /// Assigns the column and row of the current selection to the
    /// out parameters.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    public void GetSelection(out int col, out int row)
    {
        col = _selectedCol;
        row = _selectedRow;
    }

    public void GetDependents(int col, int row, out IEnumerable<string> list)
    {
        if (InvalidAddress(col, row)) //if address doesn't exist, out null, return false
        {
            list = null;
        }
        string cellName = getCellName(col, row);

        ss.GetDependents(cellName, out list);
    }

    /// <summary>
    /// returns if the spreadsheet has been saved since changed
    /// </summary>
    /// <returns></returns>
    public bool isChanged()
    {
        return ss.Changed;
    }
    /// <summary>
    /// Returns true if given col/row are in an accepted address given row/col bounds defined by class constants
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private bool InvalidAddress(int col, int row)
    {
        return col < 0 || row < 0 || col >= COL_COUNT || row >= ROW_COUNT;
    }

    /// <summary>
    /// puts values from the spreadsheet into the values and contents maps
    /// </summary>
    private void fillValuesContents()
    {
        IEnumerable<string> cells = ss.GetNamesOfAllNonemptyCells();
        foreach(string cell in cells)
        {
            Address a = findAddress(cell);
            _values[a] = cellValueSpreadsheet(cell);
            _contents[a] = cellContentsSpreadsheet(cell);
        }
    }

    /// <summary>
    /// Listener for click events on the grid.
    /// </summary>
    private void OnEndInteraction(object sender, TouchEventArgs args)
    {
        PointF touch = args.Touches[0];
        OnMouseClick(touch.X, touch.Y); //calls OnMouseClicked, and sends x and y coordinates of mouse
    }

    /// <summary>
    /// Listener for scroll events. Redraws the panel, maintaining the
    /// row and column headers.
    /// </summary>
    private void OnScrolled(object sender, ScrolledEventArgs e)
    {
        _scrollX = e.ScrollX; //sets x position of finished scroll
        _firstColumn = (int)e.ScrollX / DATA_COL_WIDTH; //sets column of finished scroll
        _scrollY = e.ScrollY; //sets y position of finshed scroll
        _firstRow = (int)e.ScrollY / DATA_ROW_HEIGHT; //sets row of finished scroll
        Invalidate(); //refresh the canvas
    }

    /// <summary>
    /// Determines which cell, if any, was clicked.  Generates a SelectionChanged
    /// event.  All of the indexes are zero based.
    /// </summary>
    /// <param name="e"></param>
    private void OnMouseClick(float eventX, float eventY)
    {
        int x = (int)(eventX - _scrollX - LABEL_COL_WIDTH) / DATA_COL_WIDTH + _firstColumn; //calculates x cell 
        int y = (int)(eventY - _scrollY - LABEL_ROW_HEIGHT) / DATA_ROW_HEIGHT + _firstRow;  //calculates y cell
        if (eventX > LABEL_COL_WIDTH && eventY > LABEL_ROW_HEIGHT && (x < COL_COUNT) && (y < ROW_COUNT))
        {
            _selectedCol = x; //set x and y
            _selectedRow = y;
            if (SelectionChanged != null) //generates a SelectionChanged event
            {
                SelectionChanged(this);
            }
        }
        Invalidate(); //refreshes canvas
    }

    /// <summary>
    /// refreshes the view of the canvas
    /// </summary>
    private void Invalidate()
    {
        graphicsView.Invalidate();
    }

    /// <summary>
    /// Used internally to keep track of cell addresses
    /// </summary>
    private class Address
    {
        public int Col { get; set; } //col property
        public int Row { get; set; } //row property

        //trivial constructor
        public Address(int c, int r)
        {
            Col = c;
            Row = r;
        }

        //returns the hashchode for the given address
        public override int GetHashCode()
        {
            return Col.GetHashCode() ^ Row.GetHashCode();
        }

        /// <summary>
        /// Overrides Equals method, first makes sure given object is a non-null address, then checks coordinates for equality
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is Address))
            {
                return false;
            }
            Address a = (Address)obj;
            return Col == a.Col && Row == a.Row;
        }
    }

    /// <summary>
    /// Draws the spreadsheet
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="dirtyRect"></param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Move the canvas to the place that needs to be drawn.
        canvas.SaveState();
        canvas.Translate((float)_scrollX, (float)_scrollY);

        // Color the background of the data area white
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(
            LABEL_COL_WIDTH,
            LABEL_ROW_HEIGHT,
            (COL_COUNT - _firstColumn) * DATA_COL_WIDTH,
            (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT);

        // Draw the column lines
        int bottom = LABEL_ROW_HEIGHT + (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT;
        canvas.DrawLine(0, 0, 0, bottom);
        for (int x = 0; x <= (COL_COUNT - _firstColumn); x++)
        {
            canvas.DrawLine(
                LABEL_COL_WIDTH + x * DATA_COL_WIDTH, 0,
                LABEL_COL_WIDTH + x * DATA_COL_WIDTH, bottom);
        }

        // Draw the column labels
        for (int x = 0; x < COL_COUNT - _firstColumn; x++)
        {
            DrawColumnLabel(canvas, x,
                (_selectedCol - _firstColumn == x) ? Font.Default : Font.DefaultBold);
        }

        // Draw the row lines
        int right = LABEL_COL_WIDTH + (COL_COUNT - _firstColumn) * DATA_COL_WIDTH;
        canvas.DrawLine(0, 0, right, 0);
        for (int y = 0; y <= ROW_COUNT - _firstRow; y++)
        {
            canvas.DrawLine(
                0, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT,
                right, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT);
        }

        // Draw the row labels
        for (int y = 0; y < (ROW_COUNT - _firstRow); y++)
        {
            DrawRowLabel(canvas, y,
                (_selectedRow - _firstRow == y) ? Font.Default : Font.DefaultBold);
        }

        // Highlight the selection, if it is visible
        if ((_selectedCol - _firstColumn >= 0) && (_selectedRow - _firstRow >= 0))
        {
            canvas.DrawRectangle(
                LABEL_COL_WIDTH + (_selectedCol - _firstColumn) * DATA_COL_WIDTH + 1,
                              LABEL_ROW_HEIGHT + (_selectedRow - _firstRow) * DATA_ROW_HEIGHT + 1,
                              DATA_COL_WIDTH - 2,
                              DATA_ROW_HEIGHT - 2);
        }

        // Draw the text
        foreach (KeyValuePair<Address, String> address in _values)
        {
            String text = address.Value;
            int col = address.Key.Col - _firstColumn;
            int row = address.Key.Row - _firstRow;
            SizeF size = canvas.GetStringSize(text, Font.Default, FONT_SIZE + FONT_SIZE * 1.75f);
            canvas.Font = Font.Default;
            if (col >= 0 && row >= 0)
            {
                canvas.DrawString(text,
                    LABEL_COL_WIDTH + col * DATA_COL_WIDTH + PADDING,
                    LABEL_ROW_HEIGHT + row * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - size.Height) / 2,
                    size.Width, size.Height, HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }
        canvas.RestoreState();
    }

    /// <summary>
    /// Draws a column label.  The columns are indexed beginning with zero.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="x"></param>
    /// <param name="f"></param>
    private void DrawColumnLabel(ICanvas canvas, int x, Font f)
    {
        String label = ((char)('A' + x + _firstColumn)).ToString();
        SizeF size = canvas.GetStringSize(label, f, FONT_SIZE + FONT_SIZE * 1.75f);
        canvas.Font = f;
        canvas.FontSize = FONT_SIZE;
        canvas.DrawString(label,
              LABEL_COL_WIDTH + x * DATA_COL_WIDTH + (DATA_COL_WIDTH - size.Width) / 2,
              (LABEL_ROW_HEIGHT - size.Height) / 2, size.Width, size.Height,
              HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    /// <summary>
    /// Draws a row label.  The rows are indexed beginning with zero.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="y"></param>
    /// <param name="f"></param>
    private void DrawRowLabel(ICanvas canvas, int y, Font f)
    {
        String label = (y + 1 + _firstRow).ToString();
        SizeF size = canvas.GetStringSize(label, f, FONT_SIZE + FONT_SIZE * 1.75f);
        canvas.Font = f;
        canvas.FontSize = FONT_SIZE;
        canvas.DrawString(label,
            LABEL_COL_WIDTH - size.Width - PADDING,
            LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - size.Height) / 2,
            size.Width, size.Height,
              HorizontalAlignment.Right, VerticalAlignment.Center);
    }

    /// <summary>
    /// calculates if a given variable is valid given by ps6 rules
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private bool isValidVariable(string s)
    {
        string standard = @"^[A-Z][1-9][0-9]?$";
        if (Regex.IsMatch(s, standard))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Calculates the cell name given col and row
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private string getCellName(int col, int row)
    {
        string c = ((char)(col + 65)).ToString();
        return c + (row + 1);
    }

    /// <summary>
    /// returns the value of cell as a string
    /// </summary>
    /// <param name="cellName"></param>
    /// <returns></returns>
    private string cellValueSpreadsheet(string cellName)
    {
        if (ss.GetCellValue(cellName) is string)
            return (string) ss.GetCellValue(cellName);
        if (ss.GetCellValue(cellName) is double)
        {
            double d = (double)ss.GetCellValue(cellName);
            return d.ToString();
        }
        if(ss.GetCellValue(cellName) is FormulaError)
        {
            FormulaError error = (FormulaError)ss.GetCellValue(cellName);
            return error.ToString();
        }    
        else
        {
            return null;
        }
    }

    /// <summary>
    /// returns the value of cell as a string
    /// </summary>
    /// <param name="cellName"></param>
    /// <returns></returns>
    private string cellContentsSpreadsheet(string cellName)
    {
        if (ss.GetCellContents(cellName) is string)
            return (string)ss.GetCellContents(cellName);
        if (ss.GetCellContents(cellName) is double)
        {
            double d = (double)ss.GetCellContents(cellName);
            return d.ToString();
        }
        if (ss.GetCellContents(cellName) is Formula)
        {
            Formula formula = (Formula)ss.GetCellContents(cellName);
            return formula.ToString();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// finds the address given a cellName
    /// </summary>
    /// <param name="cellName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidNameException"></exception>
    private Address findAddress(string cellName)
    {
        if (cellName.Length > 3)
            throw new InvalidNameException();

        System.Diagnostics.Debug.WriteLine(cellName[0]);
        System.Diagnostics.Debug.WriteLine(cellName[1]);


        int col = (int) cellName[0] - 65;
        int row;
        if(cellName.Length == 2)
        {
            row = (int) cellName[1] - 49;
        }
        else
        {
            BigInteger rowB = BigInteger.Parse(cellName.Substring(1));
            row = (int) rowB - 1;
        }
        System.Diagnostics.Debug.WriteLine(col + " ," + row);
        return new Address(col, row);
    }


}

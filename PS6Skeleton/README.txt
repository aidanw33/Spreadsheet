@Author - Aidan Wilde
@uID - u1362761
@Date - 10/21/2022

This is a spreadsheet GUI created by Aidan Wilde as a problem set for CS 3500.

Spreadsheet use Instructions ------------------------

To use the spreadsheet, one must click on the cell they wish to modify, and then type the contents they wish the cell to be set to in the editable 
text box above the spreadsheet, then press enter to commit the changes.

Left to the editable text box is a label which tells the current selected cell

Right of the editable text box is a label which displays the contents of the currently selected cell, if cell has no contents, label will be invisible

(Extra Feature) To the far right of the editable text box is a label which shows the dependents of the currently selected cell, if cell has no dependencies,
label will be invisible

To save, user can go to File > Save, they will then be prompted where on their device they would like their spreadsheet saved to, user is expected to enter exact 
file locaiton and name.

To create a new spreadsheet, user presses File > New, a new spreadsheet will appear

To open an existing spreadsheet, user presses File > Open, they will then navigate to their spreadsheet file and may open it that way.

For all File operations, safety features are implemented to prevent the accidental loss of data. 

Spreadsheet Design and Implementation Decisions -------------------------------------

Throughout creation of the spreadsheset I used previous code compiled in the Spreadsheet solution. To access this code, I solely utilized the Spreadsheet class, I initialized
and instance of this class in Spreadsheetgrid.cs. Through spreadsheet access to a dependency graph, save file, open file, and many other methods were essential
to the implementation of the spreadsheet gui.

The GUI was created using Microsofts, MAUI. 

using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net.Mail;

namespace SS
{
    /// <summary>
    /// This spreadsheet is an instance of a AbstractSpreadsheet, it implements all the
    /// methods defined in AbstractSpreadsheet, and uses a few helper methods
    /// 
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected).
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// We are not concerned with values in PS4, but to give context for the future of the project,
    /// the value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid). 
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //fields
        //mapping from names to indiviudal cells
        [JsonProperty(PropertyName = "cells")]
        private Dictionary<string, Cell> cells;
        private DependencyGraph graph;

        public override bool Changed { get; protected set; }
     
        // ADDED FOR PS5
        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default") 
        {
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Three argument constructor
        /// </summary>
        /// <param name="IsValid"></param>
        /// <param name="Normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(Func<string, bool> IsValid, Func<string, string> Normalize, string version) : base(IsValid, Normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            Changed=false;
        }


        /// <summary>
        /// Four argument constructor
        /// </summary>
        /// <param name="IsValid"></param>
        /// <param name="Normalize"></param>
        /// <param name="version"></param>
        public Spreadsheet(string filePath, Func<string, bool> IsValid, Func<string, string> Normalize, string version) : base(IsValid, Normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
            Changed = false;

            try
            {
                //var fileText = File.ReadAllText(filePath);
                Spreadsheet? fromFile = JsonConvert.DeserializeObject<Spreadsheet>(File.ReadAllText(filePath));
                
                if(fromFile!.Version != this.Version)
                {
                    throw new SpreadsheetReadWriteException("Version files mismatch");
                }

                foreach (string s in fromFile.cells.Keys)
                {
                    Cell? cell;
                    if (fromFile.cells.TryGetValue(s, out cell))
                        this.SetContentsOfCell(s, cell.stringForm);
                }
            }
            catch(FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("File not found");
            }
            catch(Exception)
            {
                throw new SpreadsheetReadWriteException("Error constructing spreadsheet from file");
            }
            
            
        }



        // ADDED FOR PS5
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            string normName = Normalize(name);
            //check name for valid input
            if (isInvalidNameSpreadsheet(normName))
                throw new InvalidNameException();

            //if the cell hasn't been defined(aka empty) empty string is returned
            if (!cells.ContainsKey(normName))
                return "";

            //return the value of the cell!
            cells.TryGetValue(normName, out Cell? cell);
                 return cell!.value;
            
        }



        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            string normName = Normalize(name);
            //creates the return cell
            Cell? outCell;

            //check if name is invalid, if so throw exception
            if (isInvalidNameSpreadsheet(normName))
                throw new InvalidNameException();

            //find the cell object via the map, if it doesn't have a cell mapped, return the empty cell's value, empty string
            if (!cells.TryGetValue(normName, out outCell!))
                return "";

            return outCell.contents;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //return all the key in the dictionary which represent the nonEmptyCellNames
            return cells.Keys;
        }


        // ADDED FOR PS5
        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //if inputted content is an empty cell, or '""', don't add any contents and return an empty list.
           // if (content.Equals(""))
            //    return new List<string>();

            string normName = Normalize(name);
            //check for valid name
            if (isInvalidNameSpreadsheet(normName))
                throw new InvalidNameException();

            //checks if given name and content can be parsed into a cell with name and double
            if (Double.TryParse(content, out double result))
            {
                //recalculate cells value
                IList<string> list = SetCellContents(normName, result); //need to do the error checking for not changing graph if circular exception is thrown
                Changed = true;

                foreach (string cell in list)
                {
                    Recalculate(cell); //recalculates the cell

                }
                return list;
            }

       
            //checks if content is a formula
            if (content.Length > 0 && content.Substring(0, 1).Equals("="))
            {
                Formula f = new Formula(content.Substring(1), Normalize, IsValid);

                //recalculate cells value
                IList<string> list3 = SetCellContents(normName, f); //need to do the error checking for not changing graph if circular exception is thrown
                Changed = true;

                foreach(string cell in list3)
                {
                    Recalculate(cell); //recalculates the cell
                }

                return list3;
            }

            //recalculate cells value
            IList<string> list2 = SetCellContents(normName, content); //need to do the error checking for not changing graph if circular exception is thrown
            Changed = true;

            foreach (string cell in list2)
            {
                Recalculate(cell); //recalculates the cell
            }
            return SetCellContents(normName, content);
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return SetCellHelper(Normalize(name), "", number, false);
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            return SetCellHelper(Normalize(name), text, 0.0, true);
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            string normName = Normalize(name);
           
            Cell cell = new Cell();

            if(formula.Evaluate(lookup) is (double))
            {
                 cell = new Cell (formula, (double)formula.Evaluate(lookup));
            }

            if(formula.Evaluate(lookup) is (FormulaError))
            {
                cell = new Cell(formula, (FormulaError)formula.Evaluate(lookup));
            }


            //map name to cell
            //if dictionary already has a cell mapped to that name, remove it and replace
            Cell? removedCell;
            cells.TryGetValue(normName, out removedCell!);

            //capture a graph state
            DependencyGraph oldGraph = graph;

            if (cells.ContainsKey(normName))
            {
                cells.Remove(normName);
                if (removedCell.formula is not null)
                {
                    IEnumerable<string> removeVars = removedCell.formula.GetVariables();
                    foreach (string s in removeVars)
                        graph.RemoveDependency(normName, s);
                }
            }



            //now add the mapping
            cells.Add(normName, cell);

           
            //get variables from cell via formula
            if(cell.formula is not null)
            {
                IEnumerable<string> vars = cell.formula.GetVariables();
                foreach (string s in vars)
                    graph.AddDependency(s, normName);
            }

         
            
            List<string> returnList = new();

            //try catch is to make sure that if a circular dependency happns that we remove the added dependencys and cells
            try
            {
                returnList.AddRange(GetCellsToRecalculate(normName)); //getCellsToRecalculate could throw a circular exception?
            }
            catch
            {
                //if throws exception
                cells.Remove(normName);
                //only add the cell back if it was previously there
                if(removedCell is not null)
                   cells.Add(normName, removedCell);
                
                graph = oldGraph; //return graph to original state
                throw new CircularException();
            }

            return returnList;
        }


        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using a JSON format.
        /// The JSON object should have the following fields:
        /// "Version" - the version of the spreadsheet software (a string)
        /// "cells" - an object containing 0 or more cell objects
        ///           Each cell object has a field named after the cell itself 
        ///           The value of that field is another object representing the cell's contents
        ///               The contents object has a single field called "stringForm",
        ///               representing the string form of the cell's contents
        ///               - If the contents is a string, the value of stringForm is that string
        ///               - If the contents is a double d, the value of stringForm is d.ToString()
        ///               - If the contents is a Formula f, the value of stringForm is "=" + f.ToString()
        /// 
        /// For example, if this spreadsheet has a version of "default" 
        /// and contains a cell "A1" with contents being the double 5.0 
        /// and a cell "B3" with contents being the Formula("A1+2"), 
        /// a JSON string produced by this method would be:
        /// 
        /// {
        ///   "cells": {
        ///     "A1": {
        ///       "stringForm": "5"
        ///     },
        ///     "B3": {
        ///       "stringForm": "=A1+2"
        ///     }
        ///   },
        ///   "Version": "default"
        /// }
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(filename, json);
                Changed = false;
            }
            catch(Exception)
            {
                throw new SpreadsheetReadWriteException("file path DNE");
            }
         
        }

        public void GetDependents(string cellName, out IEnumerable<string> list)
        {
            string normName = Normalize(cellName);
            list = graph.GetDependents(normName);
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return graph.GetDependents(name);
        }


        /// <summary>
        /// Checks if given name is invalid as defined by Spreadsheet
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool isInvalidNameSpreadsheet(string name)
        {
            string validVar = @"^[a-zA-Z]+[0-9]+$";
            if (Regex.IsMatch(Normalize(name), validVar) && IsValid(Normalize(name)))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Helper method which provides the functionallity of SetCell for a double and a string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <param name="number"></param>
        /// <param name="isText"></param>
        /// <returns></returns>
        /// <exception cref="InvalidNameException"></exception>
        private IList<string> SetCellHelper(string name, string text, double number, bool isText)
        {
            Cell cell;

            //create a cell with contents of a double
            if (isText)
                cell = new Cell(text, text);
            else
                cell = new Cell(number, number);

            //map name to cell
            //if dictionary already has a cell mapped to that name, remove it and replace
            if (cells.ContainsKey(name))
                cells.Remove(name);

            //now add the mapping
            cells.Add(name, cell);

            //if cell is just a double or string it can't be dependent on anyone
            IEnumerable<string> dependee = graph.GetDependees(name);
            IList<string> list = new List<string>();
            foreach(string dep in dependee)
            {
                list.Add(dep);
            }
            foreach(string dep in list)
            {
                graph.RemoveDependency(dep, name);
            }

            List<string> returnList = new();
            returnList.AddRange(GetCellsToRecalculate(name)); //no circular expection possible, not a formula

            return returnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private Double lookup(string str)
        {
            Cell? cell;
            cells.TryGetValue(str, out cell);
            return (Double)cell!.value;
        }

        /// <summary>
        /// recalcualtes the cells value, lookup should be defined 
        /// </summary>
        /// <param name="name"></param>
        private void Recalculate(string name)
        {
            if (cells.TryGetValue(name, out Cell? cell))
                if(cell.formula is not null)
                    cell!.value = cell.formula.Evaluate(lookup);
        }
    }

    /// <summary>
    /// Class represents a Cell, which is a representation in a SS of an individual cell
    /// </summary>
    /// 
    [JsonObject(MemberSerialization.OptIn)]
    internal class Cell
    {
        //fields
        [JsonProperty]
        public string stringForm
        {
            get;
            set;
        }

        public Object contents
        {
            get;
            private set;
        }

        public Object value
        {
            get;
            set;
        }

        public Formula? formula
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor for string content
        /// </summary>
        /// <param name="contents"></param>
        public Cell(string contents, string value)
        {
            this.contents = contents;
            this.value = value;
          
            this.stringForm = contents;
        }

        /// <summary>
        /// Constructor for double contents
        /// </summary>
        /// <param name="contents"></param>
        public Cell(double contents, double value)
        {
            this.contents = contents;
            this.formula = new Formula(contents.ToString());
            this.value = contents;
            this.stringForm = value.ToString();
        }

        /// <summary>
        /// empty constructor for reading Json
        /// </summary>
        public Cell()
        {
            stringForm = "";
            contents = "";
            value = "";
            formula = null!;
        }


        /// <summary>
        /// Constructor for formula contents
        /// </summary>
        /// <param name="contents"></param>
        public Cell(Formula contents, double value)
        {
            this.contents = contents;
            this.formula = contents;
            this.value = value;
            this.stringForm = "=" + contents.ToString();
        }

        /// <summary>
        /// Constructor for formula contents and value FormulaError
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="FormulaError"></param>
        public Cell(Formula contents, FormulaError value)
        {
            this.contents = contents;
            this.formula = contents;
            this.value = value;
            this.stringForm = "=" + contents.ToString();
        }
    }
}
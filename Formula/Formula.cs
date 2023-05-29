// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/8, updated for non-nullable types

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        //private fields
        private string _formula;
        private IEnumerable<string> tokenList = new List<string>();
        private Func<string, string> normalize;
        private Func<string, bool> isValid;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            this.normalize = normalize;
            this.isValid = isValid;

            //assign private var _formula
            this._formula = formula;

            //Get an enumrable list of tokens from string _formula
             tokenList = GetTokens(this._formula);

            //make sure tokenList has at least 1 element
            if (tokenList.Count() < 1)
                throw new FormulaFormatException("Your formula has nothing in it, incorrect format, add to your formula");

            //Create a count to check if the amount of right parenthesis is ever more then the amount of left parenthesis
            int leftParenthesis = 0;
            int rightParenthesis = 0;
            int loopPosition = 0;

            //booleans to notify that only certain tokens are valid if bool is true
           // bool numVarOpen = false;
            bool opClose = true;

            //alphabet full of all operators and right parenthesis for opClose bool to check
            string[] alphabet = new string[5] {")", "+", "-", "*", "/" }; 

            //Check to make sure each token is valid
            foreach (string token in tokenList)
            {

                //increment for each loop
                loopPosition++;

                //check to make sure we recieve an expected token
                if (opClose)
                {
                    if (!(isNumber(token) || isVar(token) || token.Equals("(")))
                        throw new FormulaFormatException("Expected number, variable, or '(', instead recieved " + token + ", incorrect syntax, place syntax in proper order");
                    //reset the bool
                    opClose = false; 
                }
                //check to make sure we will receive an expected token
                else
                {
                    if (!(alphabet.Contains(token)))
                        throw new FormulaFormatException("Expected an operator or closing parenthesis, instead recieved " + token + ", incorrect syntax, place syntax in proper order");

                    //reset the bool
                    opClose = true;
                }


                //statement checks for a number
                if (isNumber(token))
                    continue;
                

                //statements check if the given token is a valid mathematical operator
                switch (token)
                {
                    case ")":
                        if (rightParenthesis >= leftParenthesis)
                            throw new FormulaFormatException("More right parenthesis than left parenthesis, possibly due to leading right parenthesis, incorrect format, balance your parenthesis");
                        opClose = false;
                        rightParenthesis++;
                        continue;
                    case "(":
                        if (loopPosition == tokenList.Count())
                            throw new FormulaFormatException("Cannot end expression with left parenthesis, incorrect format, remove ending left parenthesis");
                        leftParenthesis++;
                        opClose = true;
                        continue;
                    case "+":
                        ifFirstOrLastCheck(loopPosition, tokenList.Count(), "addition sign");
                        continue;
                    case "-":
                        ifFirstOrLastCheck(loopPosition, tokenList.Count(), "subtraction sign");
                        continue;
                    case "*":
                        ifFirstOrLastCheck(loopPosition, tokenList.Count(), "multiplication sign");
                        continue;
                    case "/":
                        ifFirstOrLastCheck(loopPosition, tokenList.Count(), "division sign");
                        continue;
                }
                //check if token fits description of a variable token
                bool isVariable =  isVar(token);
                
                //if it is a variable token, normalize and make sure it is accepted by validator
                if(isVariable)
                {
                    if (isValid(normalize(token)))
                         continue;
                    else
                        throw new FormulaFormatException("Variable token not accepted by validator after being normalized, incorrect format, input correct variable");

                }
            }
            if (rightParenthesis != leftParenthesis)
                throw new FormulaFormatException("Not equal amount of left and right parenthesis, incorrect format, fix parenthesis");

        }


        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            //create two stacks to use in algorithm
            Stack<string> value  = new Stack<string>();
            Stack<string> operat = new Stack<string>();

            //random double to consume the exports of Double.TryParse
            double dub = 0.0;

            //iterate through the tokens to evaluate expression
            foreach(string token in tokenList)
            {

                //runs if token is a double
                if(Double.TryParse(token, out dub))
                {
                    if (getTopOperand(operat).Equals("*"))
                        multAndPush(value, operat, dub);
                    else if (getTopOperand(operat).Equals("/"))
                    {
                        if (divideAndPush(value, operat, dub))
                            return new FormulaError("Attempted to divide by Zero");
                    }
                    else
                    {
                        //otherwise push double onto value stack, then move onto next token
                        value.Push(token);
                        continue;
                    }
                }
              

                //runs if token is a variable, don't need to validate, as it's already been done
                if(isVar(token))
                {
                    double varValue = 0;
                    //lookup variable value, try catch to find a possible argument exception
                    try
                    {
                        varValue = lookup(normalize(token));
                    }
                    catch(Exception)
                    {
                        return new FormulaError("Variable had no lookup value");
                    }
                    if (getTopOperand(operat).Equals("*"))
                        multAndPush(value, operat, varValue);
                    else if (getTopOperand(operat).Equals("/"))
                    {
                        if (divideAndPush(value, operat, varValue))
                            return new FormulaError("Attempted to divide by Zero");
                    }
                    else
                    {
                        //otherwise push integer onto value stack, then move onto next token
                        value.Push(varValue + "");
                        continue;
                    }
                }

                //runs if token is an operator, assumed at this point token is an operator
                if(token.Equals("*") || token.Equals("/") || token.Equals("("))
                    operat.Push(token);

                if(token.Equals("+") || token.Equals("-"))
                {
                    string topOperand = getTopOperand(operat);
                    if (topOperand == "+")
                    {
                        addSubMultandPush(value, operat, topOperand);

                        //push operator
                        operat.Push(token);
                    }
                    else if (topOperand == "-")
                    {
                        addSubMultandPush(value, operat, topOperand);

                        //push operator
                        operat.Push(token);
                    }
                    else
                        operat.Push(token);
                }

                if(token.Equals(")"))
                {
                    string topOperand = getTopOperand(operat);
                    if (topOperand == "+")
                        addSubMultandPush(value, operat, topOperand);

                    else if (topOperand == "-")
                        addSubMultandPush(value, operat, topOperand);

                    //pop the "(" on the operator stack
                    elementsLeftInStackPop(operat, 1);
                    string poppedString = operat.Pop();

                    //look at the top of operat stack if not empty
                    topOperand = getTopOperand(operat);

                    if (topOperand == "*")
                        addSubMultandPush(value, operat, topOperand);

                    else if (topOperand == "/")
                    {
                        //popping value stack twice
                        elementsLeftInStackPop(value, 2);
                        double pop1 = Double.Parse(value.Pop());
                        double pop2 = Double.Parse(value.Pop());

                        //popping operand stsack once
                        operat.Pop();

                        Double pushVal = 0;
                   
                        if (pop1 == 0)
                          return new FormulaError("Attempted to dividy by Zero");
                        //perform calculation
                        pushVal = pop2 / pop1;
                        
                       
                        //push outcome
                        value.Push(pushVal + "");

                    }
                }
            
            }
            //check to see if operator stack is empty
            if (operat.Count == 0)
            {
                //then operand stack should contain a single number, pop and report
                if (value.Count == 1)
                    dub = Double.Parse(value.Pop());
            }
            else if (operat.Count == 1 && value.Count == 2)
            {
                String poppedOp = operat.Pop();
                if (poppedOp.Equals("+"))
                {
                    double pop1 = Double.Parse(value.Pop());
                    double pop2 = Double.Parse(value.Pop());
                    dub = pop2 + pop1;
                }
                else if (poppedOp.Equals("-"))
                {
                    double pop1 = Double.Parse(value.Pop());
                    double pop2 = Double.Parse(value.Pop());
                    dub = pop2 - pop1;
                }
            }
           
            return dub;

        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            List<string> list = new List<string>();
            //enumerate throught the tokens of the formula checking for variables
            foreach (string token in tokenList)
            {
               //check if token is a string
                if (isVar(token))
                {
                    //if so normalize the token and see if it's in the list yet
                    string normalToken = normalize(token);
                    if (!list.Contains(normalToken))
                        list.Add(normalToken);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            //string which will be added to
            string answer = "";

            //add each token to answer, if its a variable normalize first then add
            foreach(string token in tokenList)
            {
                if (isVar(token))
                    answer += normalize(token);
                else
                    answer += token;
            }
            //return string 
            return answer;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            
            // checks if given object is null, returns false if it is
            if (obj == null)
                    return false;
             

            //checks to make sure obj is of type formula
            Type t = obj.GetType();
            if (!t.Equals(typeof(Formula)))
                return false;

            
            //get objects string, garunteed to not be null cause already checked 
            string objStr = obj.ToString()!;

            //with string create a list 
            IEnumerable<string> tokenList2 = new List<string>();
            tokenList2 = GetTokens(objStr);

            //create a tokenList for this object with normalized tokens via toString
            string thisString = this.ToString()!;

            IEnumerable<string> thisTokenList = new List<string>();
            thisTokenList = GetTokens(thisString);


            if (thisTokenList.Count() != tokenList2.Count())
                return false;

            //enumerate through the tokens in both formulas checking for equality
            for(int x = 0; x < thisTokenList.Count(); x++)
            {

                //if token is varaible, normalize and check for equality, don't need to normalize cause normalized via toString
                if (isVar(thisTokenList.ElementAt(x)) && isVar(tokenList2.ElementAt(x)))
                    if (thisTokenList.ElementAt(x).Equals(tokenList2.ElementAt(x)))
                        continue;
                    else
                        return false;

               //if token is number, turn it to double, then back to string, then return
               if(Double.TryParse(thisTokenList.ElementAt(x), out double result) && Double.TryParse(tokenList2.ElementAt(x), out double result2))
                {
                    //convert back to strings and check for equality
                    string resStr = result.ToString();
                    string res2Str = result2.ToString();

                    if (resStr.Equals(res2Str))
                        continue;
                    else
                        return false;
                }

                //if not number or variable check string value for equality
                if (thisTokenList.ElementAt(x).Equals(tokenList2.ElementAt(x)))
                     continue;
                else
                    return false;

            }
           
            //if made to this point formulas are equal
            return true;

        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1.Equals(f2))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {

            //hashcode final answer
            int hashCode = 0;

            //create a string via toString, then use the String class hashcode method
            string objStr = this.ToString();

            IEnumerable<string> thisTokenList = new List<string>();
            thisTokenList = GetTokens(objStr);

            //for each tokne in thisTokenList we want to get its hashcode then add it
            foreach(string token in thisTokenList)
            {
                //we have to parse for doubles because 1e1 can produce a different hashcode then 10
                if(Double.TryParse(token, out double result))
                {
                    string str = result.ToString();
                    hashCode += Int32.Parse(str).GetHashCode();
                }
                else
                {
                    hashCode += token.GetHashCode();
                }



            }

            //return the strings hashcode
            return hashCode;
        }

        /// <summary>
        /// Checks if the given stack has at least depth elements, if not, throws an exception
        /// </summary>
        /// <param name="stck"></param>
        /// <param name="depth"></param>
        /// <returns>Throws exception if the stack doesn't have 'depth' length</returns>
        private static void elementsLeftInStackPop(Stack<String> stck, int depth)
        {
            if (stck.Count >= depth)
                return;
           
        }

        /// <summary>
        /// Checks if the given stack has at least depth elements, if not returns false
        /// </summary>
        /// <param name="stck"></param>
        /// <param name="depth"></param>
        /// <returns>returns true or false based if the stack has, depth elements</returns>
        private static bool elementLeftInStackCheck(Stack<string> stck, int depth)
        {
            if (stck.Count >= depth)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Executes of common sequence of operations on value and operator stacks using multiplication
        /// </summary>
        /// <param name="value"></param>
        /// <param name="operat"></param>
        /// <param name="variableValue"></param>
        private static void multAndPush(Stack<string> value, Stack<string> operat, double variableValue)
        {
            //pop value stack
            elementsLeftInStackPop(value, 1);
            double poppedVal = Double.Parse(value.Pop());

            //pop operator stack
            string operand = operat.Pop();

            //calculate new number then push it onto value as a string
            Double newPush = poppedVal * variableValue;
            value.Push(newPush + "");
        }

        /// <summary>
        /// Executes of common sequence of operations on value and operator stacks using division, returns true if attempted to divide by zero, otherwise false
        /// </summary>
        /// <param name="value"></param>
        /// <param name="operat"></param>
        /// <param name="variableValue"></param>
        /// <exception cref="ArgumentException"></exception>
        private static bool divideAndPush(Stack<string> value, Stack<String> operat, double variableValue)
        {
            //pop value stack
            elementsLeftInStackPop(value, 1);
            double poppedVal = Double.Parse(value.Pop());

            //pop operator stack
            string operand = operat.Pop();

            //calculate new number then push it onto value as a string, catch divide by zero and throw argument exception error instead per insturction
            
            if (variableValue == 0)
                 return true;
            Double newPush = poppedVal / variableValue;
            value.Push(newPush + "");
            return false;
            
           

        }

        /// <summary>
        /// Executes of common sequence of operations on value and operator stacks using addition, subtraction, or multiplication
        /// </summary>
        /// <param name="value"></param>
        /// <param name="operat"></param>
        /// <param name="op"></param>
        private static void addSubMultandPush(Stack<string> value, Stack<string> operat, string op)
        {
            //popping value stack twice
            elementsLeftInStackPop(value, 2);
            double pop1 = Double.Parse(value.Pop());
            double pop2 = Double.Parse(value.Pop());

            //popping operand stsack once
            operat.Pop();

            double pushVal = 0;

            //perform calc
            if (op.Equals("+"))
                pushVal = pop2 + pop1;
            else if (op.Equals("*"))
                pushVal = pop2 * pop1;
            else
                pushVal = pop2 - pop1;
            //push outcome
            value.Push(pushVal + "");
        }

        /// <summary>
        /// Common operation to get top operator, if the top operator exists in the stack
        /// </summary>
        /// <param name="operat"></param>
        /// <returns>the topOperand on the stack if it exists</returns>
        private static string getTopOperand(Stack<string> operat)
        {
            string topOperand = "";
            //check to see if operat has any elements in it
            if (elementLeftInStackCheck(operat, 1))
                topOperand = operat.Peek();
            return topOperand;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        /// <summary>
        /// Method checks if the given sign is in an incorrect postition, if so throws a FormulaFormatException
        /// </summary>
        /// <param name="loopPosition"></param>
        /// <param name="listSize"></param>
        /// <param name="sign"></param>
        /// <exception cref="FormulaFormatException"></exception>
        private void ifFirstOrLastCheck(int loopPosition, int listSize, string sign)
        {
            if (loopPosition == listSize)
                throw new FormulaFormatException("Cannot end expression with " + sign + ", incorrect format, remove ending " + sign);

        }

        /// <summary>
        /// Checks if given token is a number, if so returns true, else false
        /// </summary>
        /// <param name="potentialNumber"></param>
        /// <returns></returns>
        private bool isNumber(string token)
        {
            double number = -1;
            if (Double.TryParse(token, out number))
                return true;
            else
                return false;
        }

        /// <summary>
        /// checks if given string is a variable defined by PS3
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool isVar(string token)
        {
            string validVar = @"^[a-zA-Z_][a-zA-Z_0-9]*$";
            if (Regex.IsMatch(token, validVar))
                return true;
            else
                return false;
        }

      
        

    }


    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
            
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}



using System.Data.Common;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FormulaEvaluator
{
    /// <summary>
    /// Class reads an expression and calculates the infix calculation of the given expression
    /// @author Aidan Wilde
    /// @date
    /// </summary>
    public static class Evaluator
    {
        //delegate function used to lookup variables values
        public delegate int Lookup(String v);

        /// <summary>
        /// Fucntion evaluates the expression given from the inputted string via infix calculation
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>the evaluated expression as an integer</returns>
        public static int Evaluate(string exp, Lookup variableEvaluator)
        {
            //try catch block to catch any possible exceptions thrown
                // turns given string into an array of parsed substrings
                string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");

                //trims the substrings array into tokens used in the algorithm, checks for invalid vairables and invalid inputs
                string[] tokenstrings = tokenAnalyzer(substrings);

                //runs the algorithm
               int answer = algorithm(tokenstrings, variableEvaluator);

                //making sure answer was replaced by a real value before returning
                if (answer != -100000000)
                    return answer;
                else
                {
                    throw new Exception("no answer returned");
                }   
        }
        
        /// <summary>
        /// Checks if a given string is a variable
        /// </summary>
        /// <param name="s"></param>
        /// <returns>returns true or false depending on whether or not inputted string is a variable</returns>
        public static bool isVariable(string s)
        {
            //check to make sure the first char of string is a letter
            string s1 = s.Substring(0, 1);

            //check if s1 is a string, if not return false
            if (!Char.IsLetter(char.Parse(s1)))
                return false;

            //create a substring containing the rest of s
            string s2 = s.Substring(1);

            //boolean which signifies if the variable has moved to the number side of it's structure
            bool flip = false;

            //enumerate through all the char in s2 to check for proper structure of a variable
            foreach(char c in s2)
            {
                //check if we have reached the number portion of the variable
                if (!flip)
                {
                    //if letter do nothing
                    if (Char.IsLetter(c))
                        continue;

                    //if digit change flip to true
                    else if (Char.IsDigit(c))
                        flip = true;

                    //if neither digit or string return false
                    else
                        return false;
                }

                //if char is not a digit from this point on, it is not a variable
                else if (!Char.IsDigit(c))
                    return false;
            }
            //checks to make sure it has been flipped
            if (flip)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Reduces the given string into an array with tokens(strings elements) that contain no white space, also if any token is found that is not allowed expection is thrown
        /// </summary>
        /// <param name="substrings"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string[] tokenAnalyzer(string[] substrings)
        {
            //due to such small alphabet, run time vs data structure should be neglegent
            // alphabet for checking if the tokens contain on of the values
            string[] alphabet = new string[6] { "(", ")", "+", "-", "*", "/" }; ;

            //removes white space from tokens
            for (int x = 0; x < substrings.Length; x++)
                substrings[x] = substrings[x].Trim();
      
            //removes blank strings
            substrings = substrings.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            //make sure substrings has at least one substring
            if (substrings.Count() == 0)
                throw new ArgumentException("Null input");

            //check to see if first token is unary negative
            if (substrings[0].Equals("-"))
                throw new ArgumentException("unary negative found");

            //generally check each token is an allowed token
            foreach (string s2 in substrings)
            {
                //checks that given token is an allowed integer
                int number = -1;
               
                if (Int32.TryParse(s2, out number))
                    continue;
               
                //checks that given token is a variable if not a number
                if (isVariable(s2))
                    continue;

               
                if (alphabet.Contains(s2))
                    continue;

                throw new ArgumentException("Given input is invalid");

            }

            return substrings;
        }

        /// <summary>
        /// Method takes an array of strings or tokens and a delegate function, uses an infix algorithm to evaluate the the array
        /// </summary>
        /// <param name="tokenstrings"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>Returns the int calculated via the algorithm</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int algorithm(string[] tokenstrings, Lookup variableEvaluator)
        {
            //create answer variable
            int answer = -1;
            //Create stacks used in algorithm
            Stack<String> value = new Stack<String>();
            Stack<String> operat = new Stack<String>();

            foreach(string tokenstring in tokenstrings)
            {
                //runs if tokenstring is an integer
                int number = -1;
                bool isParsable = Int32.TryParse(tokenstring, out number);
                if (isParsable)
                { 
                    //peek the topOperand if it exists
                    string topOperand = getTopOperand(operat); 
                    if (topOperand.Equals("*"))
                    {
                        multAndPush(value, operat, number);
                    }
                    else if (topOperand.Equals("/"))
                    {
                        divideAndPush(value, operat, number);
                    }
                    else
                    {
                        //otherwise push integer onto value stack, then move onto next token
                        value.Push(tokenstring);
                        continue;
                    }
                }

                //runs if tokenstring is a variable
                if (isVariable(tokenstring))
                {
                    int variableValue = variableEvaluator(tokenstring);
                    {
                        string topOperand = getTopOperand(operat);
                        if (topOperand.Equals("*"))
                            multAndPush(value, operat, variableValue);
                        
                        else if (topOperand.Equals("/"))
                            divideAndPush(value, operat, variableValue);
                        else
                        {
                            //otherwise push integer onto value stack, then move onto next token
                            value.Push(variableValue + "");
                            continue;
                        }
                    }
                }
                    //Runs if tokenstring is an operand
                    switch (tokenstring)
                {
                    case "*":
                        {
                            operat.Push(tokenstring);
                            break;
                        }
                    case "/":
                        {
                            operat.Push(tokenstring); 
                             break;
                        }
                    case "+":
                        {
                            string topOperand = getTopOperand(operat);
                            if (topOperand == "+")
                            {
                                addSubMultandPush(value, operat, topOperand);
              
                                //push operator
                                operat.Push(tokenstring);
                            }
                            else if (topOperand == "-")
                            {
                                addSubMultandPush(value, operat, topOperand);
                             
                                //push operator
                                operat.Push(tokenstring);
                            }
                            else
                                operat.Push(tokenstring);

                            break;
                        }

                    case "-":
                        {
                            string topOperand = getTopOperand(operat);
                            if (topOperand == "+")
                            {
                                addSubMultandPush(value, operat, topOperand);
                  
                                //push operator
                                operat.Push(tokenstring);
                            }
                            else if (topOperand == "-")
                            {
                                addSubMultandPush(value, operat, topOperand);
                           
                                //push operator
                                operat.Push(tokenstring);
                            }
                            else
                                operat.Push(tokenstring);
                            
                            break;
                        }
                    case ("("):
                        {
                            operat.Push(tokenstring);
                            break;
                        }
                    case (")"):
                        {
                            string topOperand = getTopOperand(operat);
                            if (topOperand == "+")
                                addSubMultandPush(value, operat, topOperand);
                            
                            else if (topOperand == "-")
                                addSubMultandPush(value, operat, topOperand);
                         
                            //pop the "(" on the operator stack
                            elementsLeftInStackPop(operat, 1);
                            string poppedString = operat.Pop();

                            //make sure the popped string equals '(', per the algorithm
                            if (!poppedString.Equals("("))
                                throw new ArgumentException("improper syntax given");


                            //look at the top of operat stack if not empty
                            topOperand = getTopOperand(operat);

                            if (topOperand== "*")
                                addSubMultandPush(value, operat, topOperand);
                            
                            else if (topOperand == "/")
                            {
                                //popping value stack twice
                                elementsLeftInStackPop(value, 2);
                                int pop1 = Int32.Parse(value.Pop());
                                int pop2 = Int32.Parse(value.Pop());

                                //popping operand stsack once
                                operat.Pop();

                               int pushVal = 0;
                                try
                                {
                                    //perform calculation
                                    pushVal = pop2 / pop1;
                                }
                                catch (DivideByZeroException)
                                { 
                                    throw new ArgumentException("attempted to divide by zero");
                                }

                                //push outcome
                                value.Push(pushVal + "");

                            }
                            break;

                        }
                    }  
            }
            //check to see if operator stack is empty
            if(operat.Count == 0)
            {
                //then operand stack should contain a single number, pop and report
                if (value.Count == 1)
                    answer = Int32.Parse(value.Pop());
                else
                    throw new ArgumentException("algorithm is wrong, value stack doesn't have 1 element");
            }
            else if(operat.Count == 1 && value.Count == 2)
            {
                String poppedOp = operat.Pop(); 
                if(poppedOp.Equals("+"))
                {
                    int pop1 = Int32.Parse(value.Pop());
                    int pop2 = Int32.Parse(value.Pop());
                    answer = pop2 + pop1;
                }
                else if(poppedOp.Equals("-"))
                {
                    int pop1 = Int32.Parse(value.Pop());
                    int pop2 = Int32.Parse(value.Pop());
                    answer = pop2 - pop1;
                }
            }
            else
                throw new ArgumentException("Improper syntax Entered");
            
            return answer;
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
            else
                throw new ArgumentException("Stack doesn't have access to necessary elements, most likely a syntax error");
        }

        /// <summary>
        /// Checks if the given stack has at least depth elements, if not returns false
        /// </summary>
        /// <param name="stck"></param>
        /// <param name="depth"></param>
        /// <returns>returns true or false based if the stack has, depth elements</returns>
        private static bool elementLeftInStackCheck(Stack<string> stck, int depth)
        {
            if(stck.Count >= depth)
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
        private static void multAndPush(Stack<string> value, Stack<string> operat, int variableValue)
        {
            //pop value stack
            elementsLeftInStackPop(value, 1);
            int poppedVal = Int32.Parse(value.Pop());

            //pop operator stack
            string operand = operat.Pop();

            //calculate new number then push it onto value as a string
            int newPush = poppedVal * variableValue;
            value.Push(newPush + "");
        }

        /// <summary>
        /// Executes of common sequence of operations on value and operator stacks using division
        /// </summary>
        /// <param name="value"></param>
        /// <param name="operat"></param>
        /// <param name="variableValue"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void divideAndPush(Stack<string> value, Stack<String> operat, int variableValue)
        {
            //pop value stack
            elementsLeftInStackPop(value, 1);
            int poppedVal = Int32.Parse(value.Pop());

            //pop operator stack
            string operand = operat.Pop();

            //calculate new number then push it onto value as a string, catch divide by zero and throw argument exception error instead per insturction
            try
            {
                int newPush = poppedVal / variableValue;
                value.Push(newPush + "");
            }
            catch (DivideByZeroException)
            {
                throw new ArgumentException("attempted to divide by zero");
            }
            
        }

        /// <summary>
        /// Executes of common sequence of operations on value and operator stacks using addition, subtraction, or multiplication
        /// </summary>
        /// <param name="value"></param>
        /// <param name="operat"></param>
        /// <param name="op"></param>
        private static void addSubMultandPush(Stack<string> value, Stack<string> operat,  string op)
        {
            //popping value stack twice
            elementsLeftInStackPop(value, 2);
            int pop1 = Int32.Parse(value.Pop());
            int pop2 = Int32.Parse(value.Pop());

            //popping operand stsack once
            operat.Pop();

            int pushVal = 0;

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
    }
}
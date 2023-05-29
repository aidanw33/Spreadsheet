using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace FormulaEvaluator
{
    [TestClass]
    public class EvaluatorTests 
    {

        /// <summary>
        /// method serves as a delegate function that returns two
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int returnsTwo(String s)
        {
            return 2;
        }
        
        /// <summary>
        /// method serves as a delegate function that return zero
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int returnsZero(String s)
        {
            return 0;
        }

        ///----------------------------------------Begin isVariable Tests --------------------------------------///


        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableNoNumer()
        {
            Assert.IsFalse(Evaluator.isVariable("dsdf"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableLetterinNumber()
        {
            Assert.IsFalse(Evaluator.isVariable("asddf323d323"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableNumberInLetter()
        {
            Assert.IsFalse(Evaluator.isVariable("332s232fdsafda"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVairable6()
        {
            Assert.IsTrue(Evaluator.isVariable("ASDFJFKLSFDASVE323432432432"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableSymbolInLetter()
        {
            Assert.IsFalse(Evaluator.isVariable("adfad#dafda323432423"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableNumberInLetter2()
        {
            Assert.IsFalse(Evaluator.isVariable("Asdfee3dfdas34223"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableIsTrue1()
        {
            Assert.IsTrue(Evaluator.isVariable("aAdfiklnnnnnnnn324324329908908"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableIsTrue2()
        {
            Assert.IsTrue(Evaluator.isVariable("a3"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableIsTrue3()
        {
            Assert.IsTrue(Evaluator.isVariable("ADFJKL3234"));

        }

        /// <summary>
        /// Tests the isVariableMethod, specifically the test name
        /// </summary>
        [TestMethod]
        public void isVariableIsTrue4()
        {
            Assert.IsTrue(Evaluator.isVariable("ad3222"));

        }

        ///---------------------------------------End isVariable Tests --------------------------------------------///

        ///----------------------------------------Begin Expression Equality Tests --------------------------------///
        /// <summary>
        /// Tests the Evaluator on given in class expression
        /// </summary>
        [TestMethod]
        public void InClassExression()
        {
            int ans = Evaluator.Evaluate("5 + 3 * 7 - 8 / (4 + 3) - 2 / 2", returnsTwo);

            Assert.IsTrue(ans == 24);
        }

        /// <summary>
        /// Tests Evaluator on an empty expression
        /// </summary>
        [TestMethod]
        public void emptyStringTest()
        {
            try
            {
                Evaluator.Evaluate("", returnsTwo);
                Assert.Fail("no exception thrown");
            }
            catch(ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on a 1 integer
        /// </summary>
        [TestMethod]
        public void oneIntegerTest()
        {
            int ans = Evaluator.Evaluate("2", returnsTwo);

            Assert.IsTrue(ans == 2);
        }

        /// <summary>
        /// Tests the Evaluator on one given variable
        /// </summary>
        [TestMethod]
        public void oneVarTest()
        {
            int ans = Evaluator.Evaluate("a3", returnsTwo);

            Assert.IsTrue(ans == 2);
        }

        /// <summary>
        /// Tests the Evaluator on an example given in piazza
        /// </summary>
        [TestMethod]
        public void piazzaTest()
        {
            int ans = Evaluator.Evaluate("((3*6)+8)/2", returnsTwo);

            Assert.IsTrue(ans == 13);

        }

        /// <summary>
        /// Tests the Evaluator on an example given in piazza
        /// </summary>
        [TestMethod]
        public void piazzaTest2()
        {
            int ans = Evaluator.Evaluate("(1 + 3 + 4)", returnsTwo);

            Assert.IsTrue(ans == 8);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest7()
        {
            int ans = Evaluator.Evaluate("(4 * 8) / 3 + (((4 - 3) - (9 * 0)) - 3)", returnsTwo);
            // 10 + (( 1) - 3) = 8
            Assert.IsTrue(ans == 8);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest8()
        {
            int ans = Evaluator.Evaluate("3 * 3 * 3  - (9 * 0)", returnsTwo);
            
            Assert.IsTrue(ans == 27);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest9()
        {
            int ans = Evaluator.Evaluate("1 /1 /1 * 8 -90 ", returnsTwo);
            
            Assert.IsTrue(ans == -82);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest10()
        {
            int ans = Evaluator.Evaluate("1+1+1+1+1+1+1/80/9", returnsTwo);
           
            Assert.IsTrue(ans == 6);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest11()
        {
            int ans = Evaluator.Evaluate("((((( asdf33232 )))))", returnsTwo);

            Assert.IsTrue(ans == 2);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest12()
        {
            int ans = Evaluator.Evaluate("3 / asdfds3234", returnsTwo);

            Assert.IsTrue(ans == 1);

        }
        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest13()
        {
            int ans = Evaluator.Evaluate("((232 + 1 + asdf3233232))", returnsTwo);

            Assert.IsTrue(ans == 235);

        }
        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest14()
        {
            int ans = Evaluator.Evaluate("( as32 - 2 / 10)", returnsTwo);
       
            Assert.IsTrue(ans == 2);

        }
        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest15()
        {
            int ans = Evaluator.Evaluate("(10 * 0) + 8", returnsTwo);

            Assert.IsTrue(ans == 8);

        }

        /// <summary>
        /// Tests the Evaluator on a given expression
        /// </summary>
        [TestMethod]
        public void expressionTest16()
        {
            int ans = Evaluator.Evaluate("5/5*5*5", returnsTwo);

            Assert.IsTrue(ans == 25);

        }


        /// ----------------------------------End Expression Equality Tests ----------------------------///

        ///------------------------------------Being unproper syntax set -------------------------------///
        ///
        /// <summary>
        /// Tests the Evaluator on an invalid variable
        /// </summary>
        [TestMethod]
        public void oneBadVariable()
        { 
            try
            {
                Evaluator.Evaluate("ase32s2", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on a bad integer, leading negative
        /// </summary>
        [TestMethod]
        public void unaryNeg()
        {
            try
            {
                Evaluator.Evaluate("-3", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on dividing by zero
        /// </summary>
        [TestMethod]
        public void dividingByZero()
        {
            try
            {
                Evaluator.Evaluate("asedfd32222222 / 0", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyLeftParanthesis()
        {
            try
            {
                int ans = Evaluator.Evaluate("(((3)", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyRightParanthesis()
        {
            try
            {
                int ans = Evaluator.Evaluate("(((3))))))))", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyPluses()
        {
            try
            {
                int ans = Evaluator.Evaluate("3++a3", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyMinusus()
        {
            try
            {
                int ans = Evaluator.Evaluate("3--a3", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyMult()
        {
            try
            {
                int ans = Evaluator.Evaluate("3**a3", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyDiv()
        {
            try
            {
                int ans = Evaluator.Evaluate("3///a3", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyNum()
        {
            try
            {
                int ans = Evaluator.Evaluate("3+ 6 3", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void tooManyVariables()
        {
            try
            {
                int ans = Evaluator.Evaluate("a23 + ds32 dr43", returnsTwo);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// <summary>
        /// Tests the Evaluator on an improper syntax
        /// </summary>
        [TestMethod]
        public void divideByZeroVariable()
        {
            try
            {
                int ans = Evaluator.Evaluate("3 / adfa3233", returnsZero);

                Assert.Fail("no exception thrown");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }

        /// --------------------------------------------- End Unproper syntax tests -----------------------------------------------///




    }
}
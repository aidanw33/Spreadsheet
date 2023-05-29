using System.Text.RegularExpressions;

namespace SpreadsheetUtilities;

[TestClass]
public class FormulaTests
{

    /// <summary>
    /// lookup delegate method
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private double returnsTwo(string s)
    {
        return 2.0;
    }



    /// <summary>
    /// Function which serves as delegate function for testing, return false always
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static bool isNotValid(string str)
    {
        return false;
    }

    /// <summary>
    /// Function which serves as delegate function for testing, does nothing...
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static string normalizeNothing(string str)
    {
        return str;
    }

    //------------------------------------------------------- *** Begin Constructor Tests *** ---------------------------------------------------------//

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void EmptyConstructor()
    {
        Formula formula = new Formula("3");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void BasicAdditionConstructor()
    {
        Formula formula = new Formula("3 + 7");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void BasicSubtractionConstructor()
    {
        Formula formula = new Formula("3 - 8");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void BasicMultiplicationConstructor()
    {
        Formula formula = new Formula("3 * 8");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void BasicDivisionConstructor()
    {
        Formula formula = new Formula("3 / 9");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void BasicParenthesisConstructor()
    {
        Formula formula = new Formula("(3)");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void BasicVaraibleConstructor()
    {
        Formula formula = new Formula("s3");
        Assert.IsNotNull(formula);
    }

    /// <summary>
    /// Tests running the constructor by just initializing the object, Formula
    /// </summary>
    [TestMethod]
    public void complexVariableConstructor()
    {
        Formula formula = new Formula("_____s3423432432fdasfdafe_afda13");
        Assert.IsNotNull(formula);
    }


    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void badVariableConstructor()
    {
        Formula formula = new Formula(" __232-=<32 ");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void badVariableConstructor2()
    {
        Formula formula = new Formula(" 2_23 ");

    }


    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void emptyFormulaExcpetion()
    {
        Formula formula = new Formula("  ");

    }


    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void unaryNegativeNumberExcpetion()
    {
        Formula formula = new Formula("-3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void unaryPositiveNumberException()
    {
        Formula formula = new Formula("+3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void unaryMultiplicationNumberException()
    {
        Formula formula = new Formula("*3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void unaryDivisionNumberException()
    {
        Formula formula = new Formula("/3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void moreRightThanLeftParenthesis()
    {
        Formula formula = new Formula("(3 + 7))");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void moreRightThanLeftParenthesis2()
    {
        Formula formula = new Formula("(3 + 7) / ( 3))");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void moreRightThanLeftParenthesis3()
    {
        Formula formula = new Formula("())");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void rightParenthesisFirst()
    {
        Formula formula = new Formula(") + 3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void leftParenthesisLast()
    {
        Formula formula = new Formula("(3 + 7) - adf32342fdfsdafda (");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void plusSignLast()
    {
        Formula formula = new Formula("(3 + 7) +");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void minusSignLast()
    {
        Formula formula = new Formula("(3 + 7) -");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void multSignLast()
    {
        Formula formula = new Formula("(3 + 7) *");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void divisionSignLast()
    {
        Formula formula = new Formula("(3 + 7) /");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void varIsNotValid()
    {
        Formula formula = new Formula("2 + az3", normalizeNothing, isNotValid);

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void notEqualParenthesis()
    {
        Formula formula = new Formula("(3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void repeatOperators()
    {
        Formula formula = new Formula("(3 + +  7)");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void repeatOperators2()
    {
        Formula formula = new Formula("(3 - -   7)");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void repeatOperators3()
    {
        Formula formula = new Formula("3 * * (  7)");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void repeatOperators4()
    {
        Formula formula = new Formula("3 * / (  7)");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void repeatOperators5()
    {
        Formula formula = new Formula("3 - +9");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void numToVar()
    {
        Formula formula = new Formula("3 s3");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void varToNum()
    {
        Formula formula = new Formula("as34 3 + 8");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void numToClose()
    {
        Formula formula = new Formula("3 ( 9 + 0)");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void closeToNum()
    {
        Formula formula = new Formula(") 8");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void varToClose()
    {
        Formula formula = new Formula("a3 (");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void closeToVar()
    {
        Formula formula = new Formula(") asf32");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void openToOp()
    {
        Formula formula = new Formula("( * ( 9 -0 ))");

    }

    /// <summary>
    /// Tests running the constructor on a syntax that is expected to throw an exception
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void endWithOpen()
    {
        Formula formula = new Formula("3 - 9 + (");

    }

    [TestMethod]
    public void scientificNotation()
    {
        Formula formula = new Formula("3 + 1e2");
    }
    //------------------------------------------------------- *** End Constructor Tests *** ---------------------------------------------------------//


    //------------------------------------------------------- *** Begin Evaluate Tests *** -------------------------------------------------------------//

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void InClassExression()
    {
        Formula f = new Formula("5 + 3 * 7 - 8 / (4 + 3) - 2 / 2");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(24.0 - (1.0 / 7.0), fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression()
    {
        Formula f = new Formula("3.6-1.6");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(2, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression2()
    {
        Formula f = new Formula("7.8+.1");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(7.9, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression3()
    {
        Formula f = new Formula("7*8     -9");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(47, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression4()
    {
        Formula f = new Formula("3/2");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(1.5, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression5()
    {
        Formula f = new Formula("(4*0)");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(0, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression6()
    {
        Formula f = new Formula("4/4/4");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(.25, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression7()
    {
        Formula f = new Formula("7/7 * 8 -0+3");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(11, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression8()
    {
        Formula f = new Formula("1-1-1-1-1-1-1-1-1-1-1-1");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(-10, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression9()
    {
        Formula f = new Formula("9*9/9*9/9/9 + 3");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(4, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression10()
    {
        Formula f = new Formula("_fdaf2232fdsaf");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(2, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression11()
    {
        Formula f = new Formula("a_23 + a1");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(4, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression12()
    {
        Formula f = new Formula("3 * _var");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(6, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression13()
    {
        Formula f = new Formula("4 / _var");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(2, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression14()
    {
        Formula f = new Formula("(3 - 2 - 9)");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(-8, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression15()
    {
        Formula f = new Formula("(2 * 90)");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(180, fD, 1e-9);
    }

    /// <summary>
    /// Tests the Evaluate method on a given expression
    /// </summary>
    [TestMethod]
    public void evaluatorExpression16()
    {
        Formula f = new Formula(" 4 *(2 * 1) * 8");
        double fD = (double)f.Evaluate(returnsTwo);

        Assert.AreEqual(64, fD, 1e-9);
    }

    /// <summary>
    /// Tests the evaluate method if dividing by zero
    /// </summary>
    [TestMethod]
    public void divideByZeroDouble()
    {
        Formula formula = new Formula("3 / 0");
        FormulaError fE = (FormulaError)formula.Evaluate(s => 0);

    }

    /// <summary>
    /// Tests the evaluate method if dividing by zero
    /// </summary>
    [TestMethod]
    public void divideByZeroVar()
    {

        Formula formula = new Formula("3 / _a3");
        FormulaError fE = (FormulaError)formula.Evaluate(s => 0);

    }

    /// <summary>
    /// Tests the evaluate method if dividing by zero
    /// </summary>
    [TestMethod]
    public void divideByZeroExpression()
    {
        Formula formula = new Formula("3 / (2 -2 )");
        FormulaError fE = (FormulaError)formula.Evaluate(s => 0);
    }

    /// <summary>
    /// Tests the evaluate method on a bad variabel
    /// </summary>
    [TestMethod]
    public void badVarExpression()
    {
        Formula formula = new Formula("3 / (2 - _a1)");
        FormulaError fE = (FormulaError)formula.Evaluate(s => throw new ArgumentException());
    }

    /// <summary>
    /// Tests the evaluate method on a bad variabel
    /// </summary>
    [TestMethod]
    public void scientificNotationEvaluate()
    {
        Formula formula = new Formula("1e3 + 1");

        Assert.IsTrue((double)formula.Evaluate(returnsTwo) == 1001.0);

    }

    //------------------------------------------------------- *** End Evaluate Tests *** ---------------------------------------------------------------//

    //------------------------------------------------------- *** Begin GetVariables Tests *** ---------------------------------------------------------------//


    /// <summary>
    /// Tests the GetVariables method in formulaTests
    /// </summary>
    [TestMethod]
    public void getVariables()
    {
        Formula formula = new Formula("a3");
        IEnumerable<string> list = formula.GetVariables();
        Assert.IsTrue(list.Count() == 1);
        Assert.IsTrue(list.Contains("a3"));
    }

    /// <summary>
    /// Tests the GetVariables method in formulaTests
    /// </summary>
    [TestMethod]
    public void getVariables2()
    {
        Formula formula = new Formula("a3  + 54 - _2");
        IEnumerable<string> list = formula.GetVariables();
        Assert.IsTrue(list.Count() == 2);
        Assert.IsTrue(list.Contains("a3"));
        Assert.IsTrue(list.Contains("_2"));

    }

    /// <summary>
    /// Tests the GetVariables method in formulaTests
    /// </summary>
    [TestMethod]
    public void getVariables3()
    {
        Formula formula = new Formula("7/6");
        IEnumerable<string> list = formula.GetVariables();
        Assert.IsTrue(list.Count() == 0);
    }

    /// <summary>
    /// Tests the GetVariables method in formulaTests
    /// </summary>
    [TestMethod]
    public void getVariables4()
    {
        Formula formula = new Formula("a - z", s => s.ToUpper(), s => true);
        IEnumerable<string> list = formula.GetVariables();
        Assert.IsTrue(list.Count() == 2);
        Assert.IsTrue(list.Contains("A"));
        Assert.IsTrue(list.Contains("Z"));

    }

    //------------------------------------------------------- *** End GetVariables Tests *** ---------------------------------------------------------------//

    //------------------------------------------------------- *** Begin toString Tests *** ---------------------------------------------------------------//

    /// <summary>
    /// Tests the toString method from the Formula class
    /// </summary>
    [TestMethod]
    public void toStringTest()
    {
        Formula formula = new Formula("3 + 5");
        string ans = formula.ToString();
        Assert.AreEqual("3+5", ans);

    }

    /// <summary>
    /// Tests the toString method from the Formula class
    /// </summary>
    [TestMethod]
    public void toStringTest2()
    {
        Formula formula = new Formula("a3 + 5");
        string ans = formula.ToString();
        Assert.AreEqual("a3+5", ans);

    }

    /// <summary>
    /// Tests the toString method from the Formula class
    /// </summary>
    [TestMethod]
    public void toStringTest3()
    {
        Formula formula = new Formula("a3 + 5", s => s.ToUpper(), s => true);
        string ans = formula.ToString();
        Assert.AreEqual("A3+5", ans);

    }

    /// <summary>
    /// Tests the toString method from the Formula class
    /// </summary>
    [TestMethod]
    public void toStringTest4()
    {
        Formula formula = new Formula("3.0 + 5                   -0/3");
        string ans = formula.ToString();
        Assert.AreEqual("3.0+5-0/3", ans);

    }

    //------------------------------------------------------- *** End toString Tests *** ---------------------------------------------------------------//

    //------------------------------------------------------- *** Begin equals Tests *** ---------------------------------------------------------------//


    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSingleNumber()
    {
        Formula f = new Formula("3");
        Formula f2 = new Formula("3");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSingleNumberDiffFormat()
    {
        Formula f = new Formula("3.0");
        Formula f2 = new Formula("3");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSingleNumberDiffFormat2()
    {
        Formula f = new Formula("30");
        Formula f2 = new Formula("3e1");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSingleNumberDiffFormat3()
    {
        Formula f = new Formula("3.00000");
        Formula f2 = new Formula("3");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSimpleExpression()
    {
        Formula f = new Formula("3.00000 + 3");
        Formula f2 = new Formula("3 + 3");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSimpleExpression2()
    {
        Formula f = new Formula("4-0");
        Formula f2 = new Formula("4-0");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsSimpleExpression3()
    {
        Formula f = new Formula("2/8");
        Formula f2 = new Formula("2/8");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsOneVarNoNormalizer()
    {
        Formula f = new Formula("a3");
        Formula f2 = new Formula("a3");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsOneVarNormalizer()
    {
        Formula f = new Formula("x + y", s => s.ToUpper(), x => true);
        Formula f2 = new Formula("X + Y");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsOneVarNormalizerFalse()
    {
        Formula f = new Formula("x + y", s => s.ToUpper(), x => true);
        Formula f2 = new Formula("x + Y");
        Assert.IsFalse(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsOneVarNormalizerFalseReverse()
    {
        Formula f2 = new Formula("x + y", s => s.ToUpper(), x => true);
        Formula f = new Formula("x + Y");
        Assert.IsFalse(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsOneVarNormalizerReverse()
    {
        Formula f2 = new Formula("x + y", s => s.ToUpper(), x => true);
        Formula f = new Formula("X + Y");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsTwoVarNormalizer()
    {
        Formula f = new Formula("A+y-3+1e1", s => s.ToLower(), s => true);
        Formula f2 = new Formula("a + y     - 3+10");
        Assert.IsTrue(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void nullExpressiontestEquals()
    {
        Formula f = new Formula("3");
        Formula? f2 = null;

        Assert.IsFalse(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsNotFormula()
    {
        Formula f = new Formula("3.00000");
        string s = "notFormula";
        Assert.IsFalse(f.Equals(s));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void testEqualsTokenListDiffSize()
    {
        Formula f = new Formula("3 + 9 - 0");
        Formula f2 = new Formula("3 + 9");
        Assert.IsFalse(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void oneIsVarOneIsntEquals()
    {
        Formula f = new Formula("a3 + 9 - 0");
        Formula f2 = new Formula("3 + 9 - 0");
        Assert.IsFalse(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void bothVarDiffVarEquals()
    {
        Formula f = new Formula("a3 + 9 - 0", s => s.ToUpper(), s => true);
        Formula f2 = new Formula("a3 + 9 - 0");
        Assert.IsFalse(f.Equals(f2));
    }

    /// <summary>
    /// Tests the equals method from Formula class
    /// </summary>
    [TestMethod]
    public void bothDoubleNotEquals()
    {
        Formula f = new Formula("a3 + 9 - 0", s => s.ToUpper(), s => true);
        Formula f2 = new Formula("A3 + 8 - 0");
        Assert.IsFalse(f.Equals(f2));
    }

    //------------------------------------------------------- *** End equals Tests *** ---------------------------------------------------------------//

    //------------------------------------------------------- *** Begin == and != Tests *** ---------------------------------------------------------------//

    /// <summary>
    /// Tests the equals method using the overwritten operator == and !=
    /// </summary>
    [TestMethod]
    public void operatorEqualsTest()
    {
        Formula f = new Formula("a3 + 9 - 0", s => s.ToUpper(), s => true);
        Formula f2 = new Formula("A3 + 8 - 0");
        Assert.IsFalse(f == f2);

    }

    /// <summary>
    /// Tests the equals method using the overwritten operator == and !=
    /// </summary>
    [TestMethod]
    public void operatorEqualsTest2()
    {
        Formula f = new Formula("a3 + 9 - 0", s => s.ToUpper(), s => true);
        Formula f2 = new Formula("A3 + 8 - 0");
        Assert.IsTrue(f != f2);

    }

    /// <summary>
    /// Tests the equals method using the overwritten operator == and !=
    /// </summary>
    [TestMethod]
    public void operatorEqualsTest3()
    {
        Formula f = new Formula("9 -0");
        Formula f2 = new Formula("9-0");
        Assert.IsTrue(f == f2);

    }

    /// <summary>
    /// Tests the equals method using the overwritten operator == and !=
    /// </summary>
    [TestMethod]
    public void operatorEqualsTest4()
    {
        Formula f = new Formula("9 -0");
        Formula f2 = new Formula("9-0");
        Assert.IsFalse(f != f2);

    }

    /// <summary>
    /// Tests the equals method using the overwritten operator == and !=
    /// </summary>
    [TestMethod]
    public void operatorEqualsTest5()
    {
        Formula f = new Formula("9 -0");
        Formula f2 = new Formula("9-1");
        Assert.IsFalse(f == f2);


    }
    [TestMethod]
    public void operatorEqualsTest6()
    {
        Formula f = new Formula("9 -0");
        Formula f2 = new Formula("9 * 7-0");
        Assert.IsFalse(f == f2);

    }

    //------------------------------------------------------- *** Begin == and != Tests *** ---------------------------------------------------------------//

    //------------------------------------------------------- *** Begin GetHashCode Tests *** ---------------------------------------------------------------//

    /// <summary>
    /// test method tests the functionallity of getHashCode form class formula
    /// </summary>
    [TestMethod]
    public void GetHashCodeTests()
    {
        Formula f = new Formula("9 -0");
        Formula f2 = new Formula("9  -0");
        Assert.AreEqual(f.GetHashCode(), f2.GetHashCode());
    }

    /// <summary>
    /// test method tests the functionallity of getHashCode form class formula
    /// </summary>
    [TestMethod]
    public void GetHashCodeTests2()
    {
        Formula f = new Formula("9 * 8 -0");
        Formula f2 = new Formula("9 *8 -0");
        Assert.AreEqual(f.GetHashCode(), f2.GetHashCode());
    }

    /// <summary>
    /// test method tests the functionallity of getHashCode form class formula
    /// </summary>
    [TestMethod]
    public void GetHashCodeTests3()
    {
        Formula f = new Formula("A+y-3+1e1", s => s.ToLower(), s => true);
        Formula f2 = new Formula("a + y     - 3+10");
        Assert.AreEqual(f.GetHashCode(), f2.GetHashCode());
    }


    //------------------------------------------------------- *** End GetHashCode Tests *** ---------------------------------------------------------------//

    //------------------------------------------------------- *** Autograder Tests *** ---------------------------------------------------------------//
    [TestClass]
    public class GradingTests
    {

        // Normalizer tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        public void TestNormalizerGetVars()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            HashSet<string> vars = new HashSet<string>(f.GetVariables());

            Assert.IsTrue(vars.SetEquals(new HashSet<string> { "X1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        public void TestNormalizerEquals()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2+X1", s => s.ToUpper(), s => true);

            Assert.IsTrue(f.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestNormalizerToString()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f.ToString());

            Assert.IsTrue(f.Equals(f2));
        }

        // Validator tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorFalse()
        {
            Formula f = new Formula("2+x1", s => s, s => false);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        public void TestValidatorX1()
        {
            Formula f = new Formula("2+x", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX2()
        {
            Formula f = new Formula("2+y1", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX3()
        {
            Formula f = new Formula("2+x1", s => s, s => (s == "x"));
        }


        // Simple tests that return FormulaErrors
        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        public void TestUnknownVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestDivideByZeroVars()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraCloseParen()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOpenParen()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator2()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator3()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator4()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestComplex1()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestRightParens()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestLeftParens()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("53")]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestEqualsBasic()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestEqualsWhitespace()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestEqualsDouble()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestEqualsComplex()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestEqualsNullAndString()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestEqFalse()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }


        // Tests of != operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestNotEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestNotEqTrue()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestString()
        {
            Formula f = new Formula("2*5");
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestHashCode()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Technically the hashcodes could not be equal and still be valid,
        // extremely unlikely though. Check their implementation if this fails.
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestHashCodeFalse()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("2 * 5 + 4.00 - _x");
            Formula f2 = new Formula("2*5+4-_x");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestVarsNone()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestVarsSimple()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestVarsTwo()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestVarsDuplicate()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestVarsComplex()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestMultipleFormulae()
        {
            Formula f1 = new Formula("2 + a1");
            Formula f2 = new Formula("3");
            Assert.AreEqual(2.0, f1.Evaluate(x => 0));
            Assert.AreEqual(3.0, f2.Evaluate(x => 0));
            Assert.IsFalse(new Formula(f1.ToString()) == new Formula(f2.ToString()));
            IEnumerator<string> f1Vars = f1.GetVariables().GetEnumerator();
            IEnumerator<string> f2Vars = f2.GetVariables().GetEnumerator();
            Assert.IsFalse(f2Vars.MoveNext());
            Assert.IsTrue(f1Vars.MoveNext());
        }

        // Repeat this test to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestMultipleFormulaeB()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestMultipleFormulaeC()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestMultipleFormulaeD()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestMultipleFormulaeE()
        {
            TestMultipleFormulae();
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestConstructor()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // This test is repeated to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestConstructorB()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestConstructorC()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("51")]
        public void TestConstructorD()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("52")]
        public void TestConstructorE()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }


        //------------------------------------------------------- *** Autograder Tests *** ---------------------------------------------------------------//



    }
}
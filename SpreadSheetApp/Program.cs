using FormulaEvaluator;
using System;
using System.Text.RegularExpressions;

namespace SpreadSheetApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            int ans = Evaluator.Evaluate("5 + 3 * 7 - 8 / (4 + 3) - 2 / 2", falseLookup);

           Console.WriteLine(ans);
        }



        public static int falseLookup(String s)
        {
            return 2;
        }
    }



}
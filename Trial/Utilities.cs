using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankingUI
{
    static class Utilities
    {
        public static int GetPositiveInt(int max = 10)
        {
            try
            {
                int choice = int.Parse(Console.ReadLine());
                if (choice > max || choice <= 0)
                    throw new Exception();
                return choice;
            }
            catch
            {
                Console.WriteLine("You must enter an integer between 1 and {0} (inclusive). Try again.", max);
                return GetPositiveInt(max);
            }
        }
    }
}

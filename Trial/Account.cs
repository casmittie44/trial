using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trial
{
    public class Account
    {
        public string name;
        public AccountType type;
        private User owner;
        private int balance; // The balance is stored internally as an integer, to avoid floating-point errors
        private List<int> transactionRecord;

        public Account(string name, AccountType type, User owner)
        {
            this.name = name;
            this.type = type;
            this.balance = 0;
            this.transactionRecord = new List<int>();
            this.owner = owner;
            owner.AddAccount(this);
        }

        
        /// <summary>
        /// Deposit an amount into the account, in dollars
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Deposit(double amount)
        {
            if (amount < 0)
                throw new Exception("Cannot deposit negative amount of money. Try withdrawing, silly.");

            int amountInCents = Convert.ToInt32(amount * 100.0);
            balance += amountInCents;
            transactionRecord.Add(amountInCents);
            return amountInCents;
        }


        /// <summary>
        /// Withdraw a dollar amount from the account.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Withdraw(double amount)
        {
            if (amount < 0)
                throw new Exception("Cannot withdraw a negative amount of money. Try depositing, silly.");

            int amountInCents = Convert.ToInt32(amount * 100.0);
            balance -= (amountInCents);
            transactionRecord.Add(-1 * amountInCents);
            return -1 * amountInCents;
        }

        public double GetBalance(User requestingUser)
        {
            if (owner == requestingUser)
                return (balance / 100.0);
            else
                throw new Exception("The current user does not have access to that account.");
        }

        public int[] GetTransactionRecord()
        {
            return transactionRecord.ToArray();
        }

        public string PrintTransactionRecord()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0, -10} {1, 10}","Deposits", "Withdrawals\n");
            foreach (var transaction in transactionRecord)
            {
                if (transaction > 0)
                    sb.AppendFormat("{0, -10}---\n", (ToDollars(transaction)).ToString());
                
                if (transaction <= 0)
                    sb.AppendFormat("----{0, 10}\n", (ToDollars(transaction).ToString()));
            }

            sb.AppendFormat("Current Balance: {0}", ToDollars(balance)); 
            return sb.ToString();
        }

        private double ToDollars(int cents)
        {
            return cents / 100.0;
        }
    }

    public enum AccountType
    {
        Savings,
        Checkings,
    };
}

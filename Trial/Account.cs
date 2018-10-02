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
        private byte[] passwordHash;
        private int balance; // The balance is stored internally as an integer, to avoid floating-point errors
        private List<Transaction> transactionRecord;

        public Account(string name, AccountType type, byte[] passwordHash)
        {
            this.name = name;
            this.type = type;
            this.balance = 0;
            this.transactionRecord = new List<Transaction>();
            this.passwordHash = passwordHash;
        }

        private Account();

        /// <summary>
        /// Deposit a dollar amount into account
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Deposit(double amount, byte[] passwordHash)
        {
            if(!Authenticate(passwordHash))
                throw new Exception("Access denied.");

            if (amount < 0)
                throw new Exception("Cannot deposit negative amount of money. Try withdrawing, silly.");

            int amountInCents = Convert.ToInt32(amount * 100.0);
            balance += amountInCents;
            var transaction = new Transaction(-1 * amountInCents, DateTime.Now);
            transactionRecord.Add(transaction);
            return amountInCents;
        }

        /// <summary>
        /// Withdraw a dollar amount from the account.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Withdraw(double amount, byte[] passwordHash)
        {
            if (!Authenticate(passwordHash))
                throw new Exception("Access denied.");

            if (amount < 0)
                throw new Exception("Cannot withdraw a negative amount of money. Try depositing, silly.");

            int amountInCents = Convert.ToInt32(amount * 100.0);
            balance -= (amountInCents);
            var transaction = new Transaction(-1 * amountInCents, DateTime.Now);
            transactionRecord.Add(transaction);
            return -1 * amountInCents;
        }

        public double GetBalance(byte[] passwordHash)
        {
            if (!Authenticate(passwordHash))
                throw new Exception("Access denied.");

            return (balance / 100.0);                
        }

        public Transaction[] GetTransactionRecord(byte[] passwordHash)
        {
            if (Authenticate(passwordHash))
                return transactionRecord.ToArray();
            else
                return null;
        }

        public string PrintTransactionRecord(byte[] hash)
        {
            var sb = new StringBuilder();

            if (Authenticate(hash))
            {
                sb.AppendFormat("{0, -10} {1, -10} {2, -20}", "Deposits", "Withdrawals", "Date\n");
                foreach (var transaction in transactionRecord)
                {
                    if (transaction.amount > 0)
                        sb.AppendFormat("{0, -10}---{1, -20}\n", (ToDollars(transaction.amount)).ToString(), transaction.time.ToString());

                    if (transaction.amount <= 0)
                        sb.AppendFormat("----{0, 10}----{1, -20}\n", (ToDollars(transaction.amount).ToString()), transaction.time.ToString());
                }

                sb.AppendFormat("Current Balance: {0}", ToDollars(balance));

            }

            else
                sb.AppendLine("Access denied.");
                return sb.ToString();
        }

        /// <summary>
        /// Compares the argument password to the stored field passwordHash.
        /// If they do not have the same contents, returns false;
        /// if they do have the same contents, returns true.
        /// This is used to ensure that only the proper user has access to the
        /// account.
        /// </summary>
        private bool Authenticate(byte[] password)
        {
            if (password.Length != passwordHash.Length)
                return false;
            for (int i = 0; i < passwordHash.Length; i++)
                if (password[i] != this.passwordHash[i])
                    return false;

            return true;
        }

        private double ToDollars(int cents)
        {
            return cents / 100.0;
        }
    }

    public struct Transaction
    {        
        public int amount;
        public DateTime time;
        
        public Transaction(int transactionAmount, DateTime timeOfTransaction)
        {
            this.amount = transactionAmount;
            this.time = timeOfTransaction;
        }
    }

    /// <summary>
    /// There is currently no qualitative difference in how different types of accounts
    /// are handled, but in a real world application there would be
    /// </summary>
    public enum AccountType
    {
        Savings,
        Checkings,
    };
}

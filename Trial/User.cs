using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace Trial
{
	public class User
    {
        #region Fields
        public string userName;   // Username for accessing the account
		private List<Account> accounts;

        // Salt value to be used in hashing password
        private byte[] salt; 

        // Hash value of the salt prepended to the password;
        // this is the actual value used in login validations
        private byte[] passwordHash;

        // Field denoting whether to allow access to sensitive
        // internals. This field is only accessed through the 
        // Login and Logout functions
        private bool allowAccess = false;
        #endregion

        #region Methods
        /// <summary>
        /// Adds a bank account to the list of accounts owned by the User instance.
        /// </summary>
        /// <param name="newAccount"></param>
        public void CreateAccount(string name, AccountType type)
        {
            accounts.Add(new Account(name, type, this.passwordHash));
        }

        public int Deposit(double amount, int accountIndex)
        {
            if (!allowAccess)
                throw new Exception("Error: Access denied.");
            if (accountIndex > accounts.Count)
                throw new Exception("Error: attempted to access an account which does not exist.");
            else
                return accounts[accountIndex].Deposit(amount, this.passwordHash);
        }

        /// <summary>
        /// Returns an array containing all the accounts owned by the user.
        /// </summary>
		public Account[] GetAccounts() 
		{
            if (allowAccess == false || accounts == null)
                return null;
            else
			    return this.accounts.ToArray();
		}

        /// <summary>
        /// Checks the string enteredPassword against the stored passwordHash of the instance.
        /// Returns true if the passwords match, false if they do not.
        /// </summary>
        public bool Login(string enteredPassword)
        {
            var hash = new SHA256Managed();

            // Convert the entered password to a byte array and prepend the salt value to the array
            byte[] bytePassword = Encoding.ASCII.GetBytes(enteredPassword);
            byte[] arrayToHash = new byte[salt.Length + bytePassword.Length];
            for (int i = 0; i < salt.Length; i++)
                arrayToHash[i] = salt[i];
            for (int i = 0; i < bytePassword.Length; i++)
                arrayToHash[i + salt.Length] = bytePassword[i];

            // Hash the prepended array and compare to the stored hash value.
            // If they do not match return false
            byte[] enteredHash = hash.ComputeHash(arrayToHash);
            for (int i = 0; i < enteredHash.Length; i++)
                if (enteredHash[i] != passwordHash[i])
                    return false;

            allowAccess = true;
            return true;
        }

        public void Logout()
        {
            allowAccess = false;
        }

        /// <summary>
        /// Returns a formatted string containing the names and types of the accounts owned by the instance.
        /// The list is indexed from 1 to the number of accounts owned by the user.
        /// </summary>
        public string PrintAccounts()
        {          
            var sb = new StringBuilder();
            if (allowAccess == true)
            {
                sb.Append("Accounts\n");
                for (int i = 0; i < accounts.Count; i++)
                {

                    sb.Append((i + 1).ToString());
                    sb.Append(" - ");
                    sb.Append(accounts[i].name);
                    sb.Append(" --- ");
                    sb.Append("Type: ");
                    sb.Append(accounts[i].type.ToString());
                    sb.Append("\n");
                }
            }

            else
                sb.AppendLine("Access denied.");

            return sb.ToString();
        }

        public int GetNumberOfAccounts()
        {
            if (allowAccess)
                return accounts.Count;
            else
                throw new Exception("Error: Access denied.");
        }

		/// <summary>
		/// Withdraw the specified non-negative amount from the account given by accountIndex. Throws an exception
        /// if the amount entered is 
		/// </summary>
		/// <param name="amount">Amount.</param>
		/// <param name="accountIndex">Account index.</param>
		public int Withdraw(double amount, int accountIndex)
		{
            if(!allowAccess)
                throw new Exception("Error: Access denied.");
			if (accountIndex > accounts.Count)
				throw new Exception("Error: attempted to access an account which does not exist.");
			else
				return accounts[accountIndex].Withdraw(amount, this.passwordHash);
		}

        /// <summary>
        /// Throws an exception if there accessAllowed is false, or if the passed parameter is larger than the 
        /// number of accounts.
        /// </summary>
        /// <param name="accountIndex"></param>
        /// <returns></returns>
        public double GetAccountBalance(int accountIndex)
        {
            if (ValidateAccountAccess(accountIndex))
                return accounts[accountIndex].GetBalance(this.passwordHash);
            else
                throw new Exception("Something went wrong while getting account balance.");
        }

        public string GetAccountName(int accountIndex)
        {
            if (ValidateAccountAccess(accountIndex))
                return accounts[accountIndex].name;
            else
                throw new Exception("Error occurred while trying to get account name. Operation failed");
        }

        public string PrintTransactionRecord(int accountIndex)
        {
            if(ValidateAccountAccess(accountIndex))
                return accounts[accountIndex].PrintTransactionRecord(this.passwordHash);
            else
                throw new Exception("Error occurred while trying to get account name. Operation failed");
        }

        private bool ValidateAccountAccess(int index)
        {
            if (!allowAccess)
                return false;
            else if (index > accounts.Count)
                return false;
            else
                return true;
        }
        #endregion

        #region Constructor
        public User(string name, string password)
        {
            userName = name;

            // Generate salt and use it to hash password
            var random = new RNGCryptoServiceProvider();
            salt = new byte[256];
            random.GetBytes(salt);
            var hash = new SHA256Managed();
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            byte[] arrayToHash = new byte[salt.Length + passwordBytes.Length];
            for (int i = 0; i < salt.Length; i++)
                arrayToHash[i] = salt[i];
            for (int i = 0; i < passwordBytes.Length; i++)
                arrayToHash[i + salt.Length] = passwordBytes[i];

            passwordHash = hash.ComputeHash(arrayToHash);

            // Initialize list of accounts
            accounts = new List<Account>();
        }

        private User() { }
        #endregion
    }	
}


using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace Trial
{
	public class User
	{
		public string userName;
		private byte[] passwordHash;
		private List<Account> accounts;
        private byte[] salt;

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

			return true;
		}

        public void AddAccount(Account newAccount)
        {
            accounts.Add(newAccount);
        }

		public Account[] GetAccounts() 
		{
            if (accounts == null)
                return null;
            else
			    return this.accounts.ToArray();
		}

        public string PrintAccounts()
        {
            var sb = new StringBuilder("Accounts\n");
            for (int i = 0; i < accounts.Count; i++)
            {
                sb.Append((i+1).ToString());
                sb.Append(" - ");
                sb.Append(accounts[i].name);
                sb.Append(" --- ");
                sb.Append("Type:");
                sb.Append(accounts[i].type.ToString());
                sb.Append("\n");
            }

            return sb.ToString();
        }

		/// <summary>
		/// Withdraw the specified non-negative amount from the account given by accountIndex.
		/// </summary>
		/// <param name="amount">Amount.</param>
		/// <param name="accountIndex">Account index.</param>
		public int Withdraw(int amount, int accountIndex)
		{
			if (accountIndex > accounts.Count)
				throw new Exception ("Error: attempted to access an account which does not exist.");
			else
				return accounts [accountIndex].Withdraw (amount);
		}

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
            accounts = new List<Account>();
		}

	}	
}


using System;
using System.Collections.Generic;
using System.Text;
using Trial;


namespace BankingUI
{
	public static class UI
	{
        private static User currentUser;
		private static List<User> users = new List<User>();

        public static void Main()
        {
            StartUp();
            ManageInput();
        }

        /// <summary>
        /// Creates a new user who can log in and act on accounts. Creating a new user
        /// has no effect on the currently logged in user.
        /// </summary>
		public static void CreateNewUser() 
		{
            string name = GetUsername();
			string password = GetPassword();
			users.Add (new User (name, password));
			Console.WriteLine ("User {0} has been created.", name);
		}

        /// <summary>
        /// Gets a password to be used in creating a new username.
        /// </summary>
        /// <returns></returns>
		private static string GetPassword()
		{
            bool match;
            string firstPassword;

            do
            {
                Console.WriteLine("Enter a password:");
                firstPassword = Console.ReadLine();
                Console.WriteLine("Enter it again:");
                string secondPassword = Console.ReadLine();

                match = firstPassword.Equals(secondPassword);

                if (!match)
                    Console.WriteLine("Passwords do not match. Try again.");
            } while (!match);

				
            return firstPassword;
		}

        /// <summary>
        /// Get a username from the user. Checks to ensure that username will be unique.
        /// Returns the entered username as a string. Used by the CreateNewUser function
        /// </summary>
        /// <returns></returns>
        private static string GetUsername()
        {
            bool usernameFree;
            string name;
            do
            {
                Console.WriteLine("Enter a username:");
                name = Console.ReadLine();
                usernameFree = true;

                // Compare the entered username to the usernames of saved users
                foreach (var user in users)
                {
                    if (name == user.userName)
                    {
                        Console.WriteLine("That username has already been taken. Try again.");
                        usernameFree = false;
                        break;
                    }
                }

            } while (!usernameFree);

            return name;            
        }

        /// <summary>
        /// Leads the user through an attempted login. It returns true on succesfull login and false on a failed attempt.
        /// Asks the user for a username and password combination. 
        /// </summary>
        private static bool AttemptLogin()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("No users. You must create a user before you can log in.");
                return false;
            }

            // In theory, a user must log out before being able to log in as a different user, but 
            // we check for it just in case
            if (currentUser != null)
            {
                Console.WriteLine("You must log out before you attempt to log in with a different username.");
                return false;
            }

            string enteredName, enteredPassword;
            Console.Write("Enter your username: ");
            enteredName = Console.ReadLine();
            Console.Write("Enter your password: ");
            enteredPassword = Console.ReadLine();

            User attemptedLogin = null;
            foreach (var user in users)
            {
                if (enteredName == user.userName)
                {
                    attemptedLogin = user;
                    break;
                }
            }

            if (attemptedLogin == null)
            {
                Console.WriteLine("Login attempt failed.");
                return false;
            }

            if (attemptedLogin.Login(enteredPassword))
            {
                currentUser = attemptedLogin;
                Console.WriteLine("Login succesful.");
                return true;
            }

            else
            {
                Console.WriteLine("Login attempt failed.");
                return false;
            }
        }

        /// <summary>
        /// Creates a new bank account for the currentUser and adds it to the list of accounts owned by currentUser.
        /// It asks the user for input specifying the type of account to create (checkings or savings).
        /// </summary>
        private static void CreateAccount(User currentUser)
        {
            if (currentUser == null)
            {
                Console.WriteLine("You must log in before you can create an account.");
                return;
            }

            // Get account type: checkings or savings
            Console.WriteLine("What type of account would you like to create?");
            Console.WriteLine("1 - Checking Account");
            Console.WriteLine("2 - Savings Account");
            int type = Utilities.GetPositiveInt(2);
            AccountType accountType;
            if (type == 1)
                accountType = AccountType.Checkings;
            else
                accountType = AccountType.Savings;

            // Get a name for the account
            Console.Write("Enter a name for the account: ");
            string name = Console.ReadLine();
           
            // Add any other users to be owners of the account
            // Extend this later
            //Console.Write("Would you like to add any other users as co-owners of this account? [y/n]: ");
            var newAccount = new Account(name, accountType, currentUser);
            //currentUser.AddAccount(ret);
            
            if (newAccount == null)
                Console.WriteLine("Account creation failed.");

            else
            {
                Console.WriteLine("Successfully created account.");
                Console.WriteLine("Account name: {0}\n Account Balance: {1}\n Primary owner: {2}\n",
                newAccount.name, newAccount.GetBalance(currentUser), currentUser.userName);
            }
        }

        private static bool Deposit(User currentUser)
        {
            Account choice = SelectAccount(currentUser);
            if (choice == null)
                return false; ;

            // Get amount to deposit and check it is properly formatted
            Console.Write("Enter dollar amount to deposit: ");
            double amount;
            if (!double.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("You must enter a numerical value.");
                return false;
            }

            try
            {
                choice.Deposit(Convert.ToDouble(amount));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            Console.WriteLine("Deposit successful.");
            Console.WriteLine("Current balance in account {0}: {1}", choice.name,
             (choice.GetBalance(currentUser)));

            return true;
        }

        /// <summary>
        /// This function logs out currentUser. This function's main effect is to modify the global field 'currentUser',
        /// setting it to null.
        /// </summary>
        private static void LogOut()
        {
            if (currentUser == null)
                Console.WriteLine("Not currently logged in.");

            else
            {
                currentUser = null;
                Console.WriteLine("Logout successful.");
            }
        }

        /// <summary>
        /// Print menu with options to the user. If currentUser has the value null, then there is no user logged in
        /// and a menu with actions that are actually possible for this situation is printed.
        /// </summary>
        private static void PrintMenu(User currentUser)
        {
            var sb = new StringBuilder("MENU\n");
            if (currentUser == null)
            {
                sb.AppendLine("1 - Create new user");
                sb.AppendLine("2 - Log in");
                sb.AppendLine("3 - Quit");
            }

            else
            {
                sb.AppendLine("1 - Create new user");
                sb.AppendLine("2 - Create account");
                sb.AppendLine("3 - Deposit into account");
                sb.AppendLine("4 - Withdraw from account");
                sb.AppendLine("5 - View transaction record");
                sb.AppendLine("6 - View balance");
                sb.AppendLine("7 - Log out");
                sb.AppendLine("8 - Quit");
            }

            Console.Write(sb.ToString());
        }

        /// <summary>
        /// Prints a menu of options to the user upon start up. When the program first starts,
        /// the only options for a user are 1) Create a new user 2) Quit. These are the options displayed
        /// by this function.
        /// </summary>
        private static void PrintStartupMenu()
        {
            var sb = new StringBuilder("MENU\n");
            if (users.Count == 0)
            {
                sb.AppendLine("1 - Create new user");
                sb.AppendLine("2 - Quit");
            }

            Console.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Asks the user to select an account from the accounts owned by currentUser.
        /// </summary>
        private static Account SelectAccount(User currentUser)
        {
            if (currentUser == null)
            {
                Console.WriteLine("You must be signed in to do that.");
                return null;
            }
            else if (currentUser.GetAccounts().Length == 0)
            {
                Console.WriteLine("You do not have any accounts. Create an account to continue.");
                return null;
            }
            else
            {
                Console.WriteLine("Select an account.");
                Console.Write(currentUser.PrintAccounts());
                Account[] accounts = currentUser.GetAccounts();
                int choice = Utilities.GetPositiveInt(accounts.Length);
                return accounts[choice - 1];
            }
        }

        private static void ShowTransactionRecord(User currentUser)
        {
            Account choice;
            choice = SelectAccount(currentUser);
            if (choice != null)
            {
                Console.Write("Balance of account {0}: ", choice.name);
                Console.WriteLine(choice.GetBalance(currentUser).ToString());
            }
        }

        private static void ViewBalance(User currentUser)
        {
            Account choice;
            choice = SelectAccount(currentUser);

            if (choice != null)
            {
                // Show transaction recordS
                Console.WriteLine("Transaction record from account {0}:", choice.name);
                Console.WriteLine(choice.PrintTransactionRecord());
            }
        }

        private static bool Withdraw(User currentUser)
        {
            Account choice = SelectAccount(currentUser);
            if (choice == null)
                return false;

            Console.Write("Enter dollar amount to withdraw: ");
            double amount;
            if (!double.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("You must enter a numerical value.");
                return false;
            }

            // Insert code here to check if we are going to overdraw the account
            try
            {
                choice.Withdraw(Convert.ToDouble(amount));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            Console.WriteLine("Withdrawal successful.");
            Console.WriteLine("Current balance in account {0}: {1}", choice.name,
                (choice.GetBalance(currentUser)));

            return true;
        }

        private static void StartUp()
        {
            while (true)
            {
                PrintStartupMenu();
                int option = Utilities.GetPositiveInt(2);
                switch (option)
                {
                    case 1:
                        CreateNewUser();
                        return;
                        break;

                    case 2:
                        System.Environment.Exit(0);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void NoCurrentUserManageInput()
        {
            while (true)
            {
                PrintMenu(currentUser);
                int option = Utilities.GetPositiveInt(3);
                switch (option)
                {
                    case 1:
                        CreateNewUser();
                        break;

                    case 2:
                        // On successful login, return control to normal input processing
                        if (AttemptLogin()) 
                            return; 
                        break;
                    
                    case 3:
                        System.Environment.Exit(0);
                        break;

                    default:
                        break;
                }
            }
        }

        private static void ManageInput()
        {
            while(true)
            {
                if (currentUser == null)
                    NoCurrentUserManageInput();

                PrintMenu(currentUser);
                int choice = Utilities.GetPositiveInt(8);

                switch (choice)
                {
                    case 1:
                        CreateNewUser();
                        break;

                    case 2:
                        CreateAccount(currentUser);                   
                        break;

                    case 3:
                        Deposit(currentUser);                        
                        break;

                    case 4:
                        Withdraw(currentUser);
                        break;

                    case 5:
                        ViewBalance(currentUser);
                       break;

                    case 6:
                        ShowTransactionRecord(currentUser);
                        break;

                    case 7:
                        LogOut();
                        break;

                    case 8:
                        System.Environment.Exit(0);
                        break;

                    default:
                        break;
                } 
            }
        }
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using Trial;


namespace BankingUI
{
	public static class BankingUI
    {
        public static void Main()
        {
            StartUp();
            ManageInput();
        }

        #region Fields
        // User currently logged in (may be null)
        private static User currentUser;  

        // List of users which have been created in this session
		private static List<User> users = new List<User>();

  
        #endregion

        #region Banking Methods
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
            int type = GetPositiveInt(2);
            AccountType accountType;
            if (type == 1)
                accountType = AccountType.Checkings;
            else
                accountType = AccountType.Savings;

            // Get a name for the account
            Console.Write("Enter a name for the account: ");
            string name = Console.ReadLine();

            currentUser.CreateAccount(name, accountType);
            Console.WriteLine("Successfully created account.");
        }

        private static bool Deposit(User currentUser)
        {
            int choice;
            int? selection = SelectAccount(currentUser);
            if (selection == null)
                return false;
            else
                choice = (int)selection;

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
                //choice.Deposit(Convert.ToDouble(amount));
                currentUser.Deposit(amount, (int)choice);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            Console.WriteLine("Deposit successful.");
            Console.WriteLine("Current balance in account {0}: {1}", currentUser.GetAccountName(choice),
             currentUser.GetAccountBalance(choice));

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
                currentUser.Logout();
                currentUser = null;
                Console.WriteLine("Logout successful.");
            }
        }     

        /// <summary>
        /// Asks the user to select an account from the accounts owned by currentUser.
        /// It returns a nullable integer, representing the index of the selected
        /// account.
        /// </summary>
        private static int? SelectAccount(User currentUser)
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
                int choice = GetPositiveInt(currentUser.GetNumberOfAccounts());
                return choice - 1;
            }
        }

        /// <summary>
        /// Asks the user to select one of currentUser's account and then prints its balance
        /// </summary>
        private static void ViewBalance(User currentUser)
        {
            int? selection = SelectAccount(currentUser);
            if (selection != null)
            {
                int choice = (int)selection;
                Console.Write("Balance of account {0}: ", currentUser.GetAccountName(choice));
                Console.WriteLine(currentUser.GetAccountBalance(choice).ToString());
            }
        }

        /// <summary>
        /// Asks the user to select one of currentUser's accounts and then prints its transaction record
        /// </summary>
        private static void ViewTransactionRecord(User currentUser)
        {
            int? selection = SelectAccount(currentUser);

            if (selection != null)
            {
                int choice = (int)selection;
                // Show transaction recordS
                Console.WriteLine("Transaction record from account {0}:", currentUser.GetAccountName(choice));
                Console.WriteLine(currentUser.PrintTransactionRecord(choice));
            }
        }

        /// <summary>
        /// Asks the user to select one of currentUser's accounts and then
        /// withdraws money from it.
        /// </summary>
        private static bool Withdraw(User currentUser)
        {
            int? selection = SelectAccount(currentUser);
            if (selection == null)
                return false;

            int choice = (int)selection;
            Console.Write("Enter dollar amount to withdraw: ");
            double amount;
            if (!double.TryParse(Console.ReadLine(), out amount))
            {
                Console.WriteLine("You must enter a numerical value.");
                return false;
            }

            // In production code, there would some sort of warning about overdrawing the 
            // account and what not.
            try
            {
                currentUser.Withdraw(amount, choice);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            Console.WriteLine("Withdrawal successful.");
            Console.WriteLine("Current balance in account {0}: {1}", currentUser.GetAccountName(choice),
                currentUser.GetAccountBalance(choice));

            return true;
        }
        #endregion

        #region I/O Management Methods
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
            var sb = new StringBuilder("Greetings! ");
            sb.AppendLine("Note that all transactions are rounded to the nearest cent.\n");
            sb.AppendLine("MENU");
            sb.AppendLine("1 - Create new user");
            sb.AppendLine("2 - Quit");
            Console.Write(sb.ToString());
        }

        private static void StartUp()
        {
            while (true)
            {
                PrintStartupMenu();
                int option = GetPositiveInt(2);
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

        /// <summary>
        /// Prints a menu of options to the user and hands off execution
        /// to the appropriate function depending on the choice of the user.
        /// This function is called when there is no logged in user; the
        /// options displayed to the user are appropriately different than when
        /// there is a logged in user (e.g. there is no option to create an account).
        /// </summary>
        private static void NoCurrentUserManageInput()
        {
            while (true)
            {
                PrintMenu(currentUser);
                int option = GetPositiveInt(3);
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

        /// <summary>
        /// Prints a menu of options to the user and hands off execution
        /// to the appropriate function depending on the choice of the user.
        /// </summary>
        private static void ManageInput()
        {
            while(true)
            {
                if (currentUser == null)
                    NoCurrentUserManageInput();

                try
                {
                    PrintMenu(currentUser);
                    int choice = GetPositiveInt(8);

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
                            ViewTransactionRecord(currentUser);
                            break;

                        case 6:
                            ViewBalance(currentUser);
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
                catch(Exception ex)
                {
                    // Write exception message to user, then just keep looping
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        public static int GetPositiveInt(int max = 10)
        {
            bool success;
            int choice;
            // Loop until we get an integer in the proper rang from the user
            do
            {
                success = int.TryParse(Console.ReadLine(), out choice);

                if (choice > max || choice <= 0)
                {
                    // If the entered value is outside the proper range
                    success = false;
                    Console.WriteLine("Enter an integer between 1 and {0}. Try again.", max);
                }
            }while(!success);

            return choice;            
        }
        #endregion
    }
}
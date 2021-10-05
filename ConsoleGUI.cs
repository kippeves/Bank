using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public enum Menu
    {
        MAIN,
        CREATE,
        EDIT,
        LOAD,
        ACCOUNT,
        ACCOUNT_ALL,
        DELETE
    }

    public class ConsoleGUI
    {
        static Menu Menu = Menu.MAIN;

        private static bool ConfirmKey()
        {
            return Console.ReadKey(intercept: true).Key switch
            {
                ConsoleKey.Y => true,
                ConsoleKey.N => false,
                _ => false,
            };
        }

        private static Menu GetCurrentMenu()
        {
            return Menu;
        }
        private static void SetCurrentMenu(Menu m)
        {
            Menu = m;
        }

        private static void AccountMenu(BankLogic b)
        {
            Console.Clear();
            Customer c = b.GetCurrentUser();
            StringBuilder sb = new();
            int count = c.GetListOfAccounts().Count;
            if (count > 0)
            {
                c.GetListOfAccounts().ForEach(account => sb.AppendLine(account.ShowAccount()));
            }
            else
            {
                sb.AppendLine("You do not seem to have any accounts open");
                sb.AppendLine("Do you wish to open an account? Select an option below:");
            }
            sb.AppendLine("1. Open Account");
            if (count > 0)
            {
                sb.AppendLine("2. Select Account");
                sb.AppendLine("3. Close Account");
                sb.AppendLine("4. Exit");
            }
            else { 
                sb.AppendLine("2. Exit");
            }
            Console.WriteLine(sb);
            switch (Console.ReadKey(intercept: true).Key)
            {
                case ConsoleKey.D1:
                    b.AddSavingsAccount(c.SSN);
                    break;
                case ConsoleKey.D2:

                    break;
                case ConsoleKey.D3:
                    break;
                case ConsoleKey.D4:
                    break;
                default:
                    break;
            }
            Console.Clear();
        }

        private static void SelectAccount(BankLogic b)
        {
            SavingsAccount sa;
            StringBuilder sb = new("What account do you want to select?");
            int account;
            if (int.TryParse(Console.ReadLine(), out account))
            {
                if (null != (sa = b.AccountHelper(b.GetCurrentUser(), account)))
                {
                    b.SetCurrentAccount(sa);
                }
                else Console.WriteLine("Could not find account. Try again.");
            }

        }

        private static void MainMenu(BankLogic b)
        {
            Console.Clear();
            StringBuilder sb = new("Choose an option:\n");
            if (b.GetCurrentUser() == null)
            {
                sb.AppendLine("1. Create User");
                sb.AppendLine("2. Load User");
                sb.AppendLine("3. Delete User");
                sb.AppendLine("4. Exit");
            }
            else
            {
                sb.AppendLine($"Logged in user: {b.GetCurrentUser().FullName}");
                sb.AppendLine("1. Accounts");
                sb.AppendLine("2. Edit User");
                sb.AppendLine("3. Log Out");
                sb.AppendLine("4. Exit");
            }
            sb.AppendLine();
            sb.AppendLine("Information for reference");
            b.getAllCustomers().ForEach(line => sb.AppendLine(line));
            sb.AppendLine();
            sb.Append("Select an option:");
            Console.Write(sb);

            switch (Console.ReadKey(intercept: true).Key)
            {
                case ConsoleKey.D1:
                    SetCurrentMenu((b.GetCurrentUser() == null) ? Menu.CREATE : Menu.ACCOUNT);
                    break;
                case ConsoleKey.D2:
                    if (b.GetCurrentUser() == null)
                    {
                        SetCurrentMenu(Menu.LOAD);
                    }
                    else SetCurrentMenu(Menu.EDIT);
                    break;
                case ConsoleKey.D3:
                    if (b.GetCurrentUser() == null)
                    {
                        SetCurrentMenu(Menu.DELETE);
                    }
                    else b.SetCurrentUser(null);
                    break;
                case ConsoleKey.D4:
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }
        private static void CreateUserMenu(BankLogic b)
        {
            StringBuilder sb = new("Do you want to create a user?\n");
            if (b.GetCurrentUser() != null)
            {
                sb.AppendLine("You appear to already have a user (" + b.GetCurrentUser().FullName + ") active");
                sb.AppendLine("You can only have one user loaded concurrently.");
                sb.AppendLine("If you create a new one your current one will be logged out.");
            }
            sb.AppendLine("Press Y to continue, N to go back to Main Menu.");
            Console.WriteLine(sb);
            sb.Length = 0;
            switch (Console.ReadKey(intercept: true).Key)
            {
                case ConsoleKey.Y:
                    bool CreatedUser = false;
                    string name;
                    long pNr;
                    bool correctSSN;

                    while (!CreatedUser)
                    {
                        do
                        {
                            Console.WriteLine("What is your name?");
                            name = Console.ReadLine();
                            Console.WriteLine("Your name is " + name + ". Correct? (Y/N)");
                        } while (!ConfirmKey());
                        do
                        {
                            Console.WriteLine("What is your social security number?");
                            correctSSN = long.TryParse(Console.ReadLine(), out pNr);
                            if (!correctSSN)
                            {
                                Console.WriteLine("You did not enter your number correctly. Please try again.");
                            }
                            else
                            {
                                Console.WriteLine("Your SSN is: " + pNr + ". Correct? (Y/N)");
                                correctSSN = ConfirmKey();
                            }
                        } while (!correctSSN);
                        sb.AppendLine("Information to be put in the system:");
                        sb.AppendLine($"Name:\t{name}");
                        sb.AppendLine($"SSN:\t{pNr}");
                        sb.AppendLine("Correct? (Y/N)?");
                        Console.WriteLine(sb);
                        if (ConfirmKey())
                        {
                            bool registeredUser = b.AddCustomer(name, pNr);
                            if (registeredUser)
                            {
                                Console.WriteLine("You successfully registered in our system.");
                                Console.WriteLine("You will now be returned to the Main Menu.");
                                CreatedUser = true;
                                b.SetCurrentUser(b.CustomerHelper(pNr));
                                SetCurrentMenu(Menu.MAIN);
                                System.Threading.Thread.Sleep(1000);
                            }
                            else
                            {
                                Console.WriteLine("You could not be registered into the system.");
                                Console.WriteLine("Do you already have a user with that Social Security Number registered?");
                            }
                        }
                    }
                    break;
                case ConsoleKey.N:
                    SetCurrentMenu(Menu.MAIN);
                    break;
                default:
                    break;
            }
        }
        private static void EditUserMenu(BankLogic b)
        {
            Console.Clear();
            StringBuilder sb = new("Edit User:\n");
            Customer c = b.GetCurrentUser();
            if (null != c)
            {
                sb.AppendLine($"Current user: {c.FullName}");
                sb.AppendLine("1. Change First Name.");
                sb.AppendLine("2. Change Surname.");
                sb.AppendLine("3. Exit");
            }
            else
            {
                sb.Length = 0;
                sb.AppendLine("You really shouldn't be here...");
            }
            Console.WriteLine(sb);
            String tempString = null;
            bool Confirm;
            switch (Console.ReadKey(intercept: true).Key)
            {
                case ConsoleKey.D1:
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("What is your new name?");
                        tempString = Console.ReadLine().Trim();
                        if (tempString.Length > 0)
                        {
                            Console.WriteLine($"\"{tempString}\".");
                            Console.WriteLine("Is that correct? (Y/N)");
                            Confirm = ConfirmKey();
                            if (Confirm)
                            {
                                c.SetFirstName(tempString);
                            }
                        }
                        else
                        {
                            Confirm = true;
                        }
                    } while (!Confirm);
                    break;
                case ConsoleKey.D2:
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("What is your new name?");
                        tempString = Console.ReadLine().Trim();
                        if (tempString.Length > 0)
                        {
                            Console.WriteLine($"\"{tempString}\".");
                            Console.WriteLine("Is that correct? (Y/N)");
                            Confirm = ConfirmKey();
                            if (Confirm)
                            {
                                c.SetLastName(tempString);
                            }
                        }
                        else
                        {
                            Confirm = true;
                        }
                    } while (!Confirm);
                    break;
                case ConsoleKey.D3:
                    SetCurrentMenu(Menu.MAIN);
                    break;
                default:
                    break;
            }
        }
        

        static void Main()
        {
            BankLogic b = new();
            long ssn = 2827328;
            b.AddCustomer("John Andersson", ssn);
            Customer c = b.CustomerHelper(ssn);
            b.SetCurrentUser(c);
            b.SetCurrentAccount(null);

            while (true)
            {
                if (GetCurrentMenu() == Menu.MAIN)
                    MainMenu(b);
                else if (GetCurrentMenu() == Menu.CREATE)
                    CreateUserMenu(b);
                else if (GetCurrentMenu() == Menu.EDIT)
                    EditUserMenu(b);
                else if (GetCurrentMenu() == Menu.LOAD)
                    LoadUserMenu(b);
                else if (GetCurrentMenu() == Menu.ACCOUNT)
                    AccountMenu(b);
                //                else if (GetCurrentMenu() == Menu.DELETE)
                //               else if (GetCurrentMenu() == Menu.ACCOUNT_ALL)
            }
        }

        private static void LoadUserMenu(BankLogic b)
        {
            throw new NotImplementedException();
        }
    }
}

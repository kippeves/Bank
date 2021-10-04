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

        private static void CreateUserMenu(BankLogic BankInstance)
        {
            StringBuilder sb = new("Do you want to create a user?\n");
            if (BankInstance.GetCurrentUser() != null)
            {
                sb.AppendLine("You appear to already have a user (" + BankInstance.GetCurrentUser().FullName + ") active");
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
                            bool registeredUser = BankInstance.AddCustomer(name, pNr);
                            if (registeredUser)
                            {
                                Console.WriteLine("You successfully registered in our system.");
                                Console.WriteLine("You will now be returned to the Main Menu.");
                                CreatedUser = true;
                                BankInstance.SetCurrentUser(BankInstance.CustomerHelper(pNr));
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

        static void Main()
        {
            BankLogic b = new();
            b.SetCurrentUser(null);
            b.SetCurrentAccount(null);
            while (true)
            {
                Console.Clear();
                if (GetCurrentMenu() == Menu.MAIN)
                {
                    StringBuilder sb = new("Choose an option:\n");
                    if (b.GetCurrentUser() == null)
                    {
                        sb.AppendLine("1. Create User");
                        sb.AppendLine("2. Load User");
                        sb.AppendLine("3. Delete User");
                        sb.AppendLine("4. Exit");

                    }
                    else {
                        sb.AppendLine($"Logged in user: {b.GetCurrentUser().FullName}");
                        sb.AppendLine("1. View Accounts");
                        sb.AppendLine("2. Log Out");
                        sb.AppendLine("3. Exit");

                    }
                    sb.AppendLine("Select an option:");
                    Console.WriteLine(sb);
                    switch (Console.ReadKey().KeyChar)
                    {
                        case '1':
                            SetCurrentMenu((b.GetCurrentUser() == null) ? Menu.CREATE : Menu.ACCOUNT);
                            break;
                        case '2':
                            if (b.GetCurrentUser() == null)
                            {
                                SetCurrentMenu(Menu.LOAD);
                            }
                            else b.SetCurrentUser(null);
                            break;
                        case '3':
                            if (b.GetCurrentUser() == null)
                            {
                                SetCurrentMenu(Menu.DELETE);
                            }
                            else Environment.Exit(0);
                            break;
                        case '4':
                            if (b.GetCurrentUser() == null)
                            { 
                                Environment.Exit(0);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (GetCurrentMenu() == Menu.CREATE)
                {
                    CreateUserMenu(b);
                }
            }
        }
    }
}

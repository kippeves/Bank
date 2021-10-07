using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bank
{

    class BankLogic
    {
        readonly private List<Customer> ListOfCustomers = new();

        /// <summary>
        /// Hämtar en lista med alla kunder som finns i banken.
        /// </summary>
        /// <returns>En lista med alla kunder i objektformat</returns>
        public List<Customer> GetListOfCustomers()
        {
            return ListOfCustomers;
        }

        /// <summary>
        /// Hämtar en kund i systemet. Om kunden inte finns så returneras null.
        /// </summary>
        /// <param name="ssn"></param>
        /// <returns>Ett Customer-objekt.</returns>
        public Customer CustomerHelper(long ssn)
        {
            IEnumerable<Customer> tempCustList = ListOfCustomers.Where(c => c.SSN == ssn);
            if (tempCustList.Count() == 1)
            {
                return tempCustList.First();
            }
            else return null;
        }


        /// <summary>
        /// Hämtar ett sparkonto-objekt. Om objektet inte finns så returneras null.
        /// </summary>
        /// <param name="c">Ett kundobjekt.</param>
        /// <param name="accountId">Ett kontonummer i heltalsform</param>
        /// <returns></returns>
        public SavingsAccount AccountHelper(Customer c, int accountId)
        {
            IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
            if (temp.Count() == 1)
            {
                return temp.First();
            }
            else return null;
        }

        /// <summary>
        /// Hämtar en lista med information om alla kunder.
        /// </summary>
        /// <returns>En lista med strängar.</returns>
        public List<String> GetCustomers()
        {
            List<string> returnList = new();
            ListOfCustomers.ForEach(cust => returnList.Add("Personnummer: " + cust.SSN + ", Namn: " + cust.FullName));
            return returnList;
        }

        /// <summary>
        /// Lägger till en kund i banksystemet.
        /// Namnet klipps på det sista mellanslaget.
        /// </summary>
        /// <param name="name">Namnet på kunden, både för och efternamn.</param>
        /// <param name="pNr">Personnummer för en kund</param>
        /// <returns></returns>
        public bool AddCustomer(string name, long pNr)
        {
            int SpaceIndex = name.LastIndexOf(" ");
            string FirstName;
            string LastName;
            if (SpaceIndex > 0)
            {
                FirstName = name[0..SpaceIndex].Trim();
                LastName = name[(SpaceIndex + 1)..name.Length].Trim();
            }
            else
            {
                FirstName = name;
                LastName = "";
            }
            Customer c = CustomerHelper(pNr);
            if (null == c)
            {
                c = new(FirstName, LastName, pNr);
                ListOfCustomers.Add(c);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Hämtar information om en kund i textformat.
        /// </summary>
        /// <param name="pNr">Ett personnummer.</param>
        /// <returns></returns>
        public List<string> GetCustomer(long pNr)
        {
            List<string> TempList = new();
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                TempList.Add($"Namn: {c.FullName}, Personnummer: {c.SSN}");
                TempList.Add("Konton:");
                TempList.AddRange(c.PrintAccounts());
                return TempList;
            }
            else return null;
        }


        /// <summary>
        /// Ändrar namnet på en kund i banksystemet.
        /// Namnet klipps på det sista mellanslaget.
        /// </summary>
        /// <param name="name">Namnet på kunden, både för och efternamn.</param>
        /// <param name="pNr">Personnummer för en kund</param>
        /// <returns></returns>
        public bool ChangeCustomerName(String name, long pNr)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                int SpaceIndex = name.LastIndexOf(" ");
                string FirstName;
                string LastName;
                if (SpaceIndex > 0)
                {
                    FirstName = name[0..SpaceIndex].Trim();
                    LastName = name[(SpaceIndex + 1)..name.Length].Trim();
                }
                else
                {
                    FirstName = name;
                    LastName = "";
                }
                c.SetFirstName(FirstName);
                c.SetLastName(LastName);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Tar bort en kund i banksystemet.
        /// </summary>
        /// <param name="pNr">Personnumret för kunden som skall tas bort.</param>
        /// <returns></returns>
        public List<string> RemoveCustomer(long pNr)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                List<string> tempList = new();
                tempList.AddRange(GetCustomer(pNr));
                List<SavingsAccount> accounts = c.GetListOfAccounts();
                if (accounts.Count > 0)
                {
                    decimal sum = 0;
                    decimal sumOfInterest = 0;

                    accounts.ForEach(a => sum += a.GetAmount());
                    tempList.Add($"Sum of accounts:\t" + sum);
                    accounts.ForEach(a => sumOfInterest += a.CalculateInterest());
                    tempList.Add($"Sum of interest:\t" + sumOfInterest);
                    accounts.Clear();

                }
                else tempList.Add("You have no accounts...");
                GetListOfCustomers().Remove(c);
                return tempList;
            }
            else return null;
        }


        /// <summary>
        /// Lägger till ett sparkonto för en kund.
        /// </summary>
        /// <param name="pNr">Personnumret för den kund som skall få ett konto.</param>
        /// <returns></returns>
        public int AddSavingsAccount(long pNr)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                if (c.GetListOfAccounts().Count > 0)
                {
                    int id = c.GetListOfAccounts().Last().GetAccountNo();
                    c.GetListOfAccounts().Add(new SavingsAccount(++id));
                    return id;
                }
                else
                {
                    c.GetListOfAccounts().Add(new SavingsAccount(1001));
                    return 1001;
                }
            }
            else return -1;
        }

        /// <summary>
        /// Hämtar information om ett konto
        /// </summary>
        /// <param name="pNr">Personnumret på en kund</param>
        /// <param name="accountId">Kontonumret för det konto som skall hämtas.</param>
        /// <returns></returns>
        public string GetAccount(long pNr, int accountId)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                SavingsAccount sa = AccountHelper(c, accountId);
                if (null == sa)
                {
                    return sa.ShowAccount();
                }
                else return "You had more than one account with that account-number.";
            }
            else return "No customer with that Social Security Number.";
        }
        /// <summary>
        /// Lägger till pengar på ett konto
        /// </summary>
        /// <param name="pNr">Personnumret för den kund som skall få pengar insatta</param>
        /// <param name="accountId">Kontonumret för det konto som skall få pengar insatta.</param>
        /// <param name="amount">Den mängd som skall sättas in.</param>
        /// <returns></returns>
        public Boolean Deposit(long pNr, int accountId, decimal amount)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                SavingsAccount sa = AccountHelper(c, accountId);
                if (null != sa)
                {
                    sa.DepositAmount(amount);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Tar ut pengar ifrån ett konto
        /// </summary>
        /// <param name="pNr">Personnumret för den kund som skall ta ut pengar</param>
        /// <param name="accountId">Kontonumret för det konto som skall få pengar uttaget.</param>
        /// <param name="amount">Den mängd som skall tas ut.</param>
        /// <returns></returns>
        public Boolean Withdraw(long pNr, int accountId, decimal amount)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                SavingsAccount sa = AccountHelper(c, accountId);
                if (null != sa)
                {
                    return sa.WithdrawAmount(amount);
                }
                else return false;
            }
            else return false;
        }


        /// <summary>
        /// Stänger ett sparkonto i banksystemet.
        /// </summary>
        /// <param name="pNr">Personnumret för den kund vars konto ska stängas</param>
        /// <param name="accountId">Kontonummer för det konto som skall stängas</param>
        /// <returns></returns>
        public string CloseAccount(long pNr, int accountId)
        {
            Customer c = CustomerHelper(pNr);
            if (null != c)
            {
                SavingsAccount sa = AccountHelper(c, accountId);
                if (null != sa)
                {
                    string returnString = sa.ShowAccount();
                    c.GetListOfAccounts().Remove(sa);
                    return returnString;
                }
                else return null;
            }
            else return null;
        }
        /// <summary>
        /// Hämtar en lista med alla kunder i textformat.
        /// </summary>
        /// <returns></returns>
        public List<string> getAllCustomers()
        {
            List<string> tempString = new();
            foreach (Customer c in GetListOfCustomers())
            {
                GetCustomer(c.SSN).ForEach(line => tempString.Add(line));
            }
            return tempString;
        }


        public static void Case1(BankLogic b)
        {
            StringBuilder sb = new();
            Console.Clear();
            Console.WriteLine("Skapar textfilen \"list.txt\" i mappen som .EXE-filen körs i.");
            foreach (string line in b.getAllCustomers())
            {
                sb.AppendLine(line);
            }
            Console.WriteLine(sb);
            string path = @"list.txt";
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(sb);
            }
            Console.WriteLine($"Lista skriven till {path}");

            Console.ReadLine();
            Console.Clear();
        }
        public static void Case2(BankLogic b)
        {
            string name;
            bool correctSSN;
            long pNr;
            StringBuilder sb = new();
            do
            {
                Console.WriteLine("Vad är ditt namn?");
                name = Console.ReadLine();
            } while (name.Trim().Length == 0);
            Console.WriteLine();
            do
            {
                Console.WriteLine("Vad är ditt personnummer?");
                correctSSN = long.TryParse(Console.ReadLine(), out pNr);
                if (!correctSSN)
                {
                    Console.WriteLine("Du skrev inte in ditt nummer korrekt. Vänligen försök igen");
                }
            } while (!correctSSN);
            sb.AppendLine("Denna information kommer att sparas:");
            sb.AppendLine($"Namn:\t{name}");
            sb.AppendLine($"Personnummer:\t{pNr}");
            Console.WriteLine(sb);
            Console.WriteLine();
            bool registeredUser = b.AddCustomer(name, pNr);
            if (registeredUser)
            {
                Console.WriteLine("Din användare lades till i listan.");
            }
            else
            {
                Console.WriteLine("Du kunde inte läggas till i systemet");
                Console.WriteLine("Do you already have a user with that Social Security Number registered?");
            }
            Console.WriteLine();
        }
        public static void Case3(BankLogic b)
        {
            bool correctSSN;
            string name;
            long pNr;

            Console.WriteLine("Vilken användare vill du ändra namn på?");
            foreach (Customer c in b.GetListOfCustomers())
            {
                Console.WriteLine($"{c.SSN}: {c.FullName}");
            }
            do
            {
                Console.WriteLine("What is your social security number?");
                correctSSN = long.TryParse(Console.ReadLine(), out pNr);
                if (!correctSSN)
                {
                    Console.WriteLine("You did not enter your number correctly. Please try again.");
                }
            } while (!correctSSN);
            do
            {
                Console.WriteLine("What name do you want to change to?");
                name = Console.ReadLine();
            }
            while (name.Trim().Length == 0);
            b.ChangeCustomerName(name, pNr);
        }
        public static void Case4(BankLogic b)
        {
            foreach (var item in b.getAllCustomers())
            {
                Console.WriteLine(item);
            }
            string answer = "";
            while (answer != "nej")
            {

                Console.Write("Mata in personnummret på kunden du vill ta bort: ");
                long pNr = long.Parse(Console.ReadLine());
                List<string> s;
                if ((s = b.RemoveCustomer(pNr)) != null)
                {
                    Console.WriteLine("Du tog bort denna kund:");
                    Console.WriteLine("");
                    foreach (var item in s)
                    {
                        Console.WriteLine(item);
                    }
                }
                else
                {
                    Console.WriteLine("Felaktigt personnummer.");
                }

                Console.WriteLine("");
                Console.WriteLine("");

                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("");
                Console.WriteLine("Vill du ta bort yttligare en kund? ja/nej");
                answer = Console.ReadLine().ToLower();
            }
        }
        public static void Case5(BankLogic b)
        {
            string answer = "";
            while (answer != "nej")
            {
                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("");
                Console.Write("Mata in personnummer på den kund som du vill skapa ett nytt konto åt: ");
                long svar3 = long.Parse(Console.ReadLine());

                int status = b.AddSavingsAccount(svar3);

                if (status == -1)
                {
                    Console.WriteLine("Felaktigt personnummer.");
                }
                else
                {
                    Console.WriteLine($"Ett sparkonto skapades. Kontonummer {status}");
                }

                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }

                Console.Write("Vill du skapa yttligare ett sparkonto åt en befintlig kund? ja/nej ");
                answer = Console.ReadLine().ToLower();
            }
        }
        public static void Case6(BankLogic b)
        {
            string answer = "";
            while (answer != "nej")
            {
                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("");
                Console.Write("Mata in det personnummer till det kontot du vill avsluta: ");
                long svar6 = long.Parse(Console.ReadLine());

                Console.WriteLine(" ");

                Console.Write("Mata in konto Id till kontot du vill avsluta: ");
                int svar7 = int.Parse(Console.ReadLine());
                string status = b.CloseAccount(svar6, svar7);

                if (status != null)
                {
                    Console.WriteLine("Ditt konto har tagits bort.");
                }
                else
                {
                    Console.WriteLine("Felaktig inmatning!");
                }

                Console.WriteLine("Vill du ta bort yttligare ett konto? ja/nej");
                answer = Console.ReadLine().ToLower();
            }
        }
        public static void Case7(BankLogic b)
        {
            string answer = "";
            while (answer != "nej")
            {
                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine(" ");
                Console.Write("Ange kundens personnummer som du vill se information om? ");
                long svar8 = long.Parse(Console.ReadLine());

                Customer s = b.CustomerHelper(svar8);
                if (s == null)
                {
                    Console.WriteLine("Kunden fanns inte.");
                }
                else
                {
                    Console.WriteLine("Förnamn: " + s.GetFirstName());
                    Console.WriteLine("Efternamn: " + s.GetLastName());
                    Console.WriteLine("SSN: " + svar8);
                }
                Console.WriteLine(" ");
                Console.WriteLine("Vill du se yttligare en kund? ");
                answer = Console.ReadLine().ToLower();
            }
        }
        public static void Case8(BankLogic b)
        {
            string answer = "";
            bool ParseSuccess;
            bool customerExists;
            long tempValue;
            Customer c;

            while (answer != "nej")
            {
                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }
                do
                {
                    Console.WriteLine("Skriv personnummer för kund:");
                    String parseString = Console.ReadLine().Trim();
                    ParseSuccess = long.TryParse(parseString, out tempValue);
                    c = b.CustomerHelper(tempValue);
                    if (c == null)
                    {
                        Console.WriteLine("Det finns ingen kund med det personnumret");
                    }
                    customerExists = !ParseSuccess & (c == null);
                } while (customerExists);

                SavingsAccount sa;
                int temp;

                do
                {
                    Console.WriteLine("Skriv kontonummer för kund:");
                    ParseSuccess = int.TryParse(Console.ReadLine(), out temp);
                    sa = b.AccountHelper(c, temp);
                    if (sa == null)
                    {
                        Console.WriteLine("Det finns ingen konto med det kontonumret");
                    }
                } while (!ParseSuccess && (sa == null));

                Console.WriteLine("Hur mycket pengar vill du sätta in?");
                Decimal tempDec = 0;
                do
                {
                    ParseSuccess = Decimal.TryParse(Console.ReadLine(), out tempDec);
                    if (ParseSuccess)
                    {
                        sa.DepositAmount(tempDec);
                    }
                    else Console.WriteLine("Du skrev inte in ett giltigt tal, försök igen");
                } while (!ParseSuccess);
                Console.WriteLine($"{tempDec} sattes in på konto {sa.GetAccountNo()}, tillhörande {c.FullName}");
                Console.WriteLine(" ");
                Console.WriteLine("Vill du se yttligare en kund? ");
                answer = Console.ReadLine().ToLower();
            }
        }
        public static void Case9(BankLogic b){
            string answer = "";
            bool ParseSuccess = false;
            while (answer != "nej")
            {
                long tempValue;
                foreach (var item in b.getAllCustomers())
                {
                    Console.WriteLine(item);
                }
                Customer c;
                do
                {
                    Console.WriteLine("Skriv personnummer för kund:");
                    ParseSuccess = long.TryParse(Console.ReadLine(), out tempValue);
                    c = b.CustomerHelper(tempValue);
                    if (c == null)
                    {
                        Console.WriteLine("Det finns ingen kund med det personnumret");
                    }
                } while (!ParseSuccess && (c == null));
                SavingsAccount sa;
                int temp;
                do
                {
                    Console.WriteLine("Skriv kontonummer för kund:");
                    ParseSuccess = int.TryParse(Console.ReadLine(), out temp);
                    sa = b.AccountHelper(c, temp);
                    if (sa == null)
                    {
                        Console.WriteLine("Det finns ingen konto med det kontonumret");
                    }
                } while (!ParseSuccess && (sa == null));
                Console.WriteLine("Hur mycket pengar vill du ta ut?");
                Decimal tempDec = 0;
                do
                {
                    ParseSuccess = Decimal.TryParse(Console.ReadLine(), out tempDec);
                    if (ParseSuccess)
                    {
                        if (sa.WithdrawAmount(tempDec))
                        {
                            Console.WriteLine($"{tempDec} togs ut från konto {sa.GetAccountNo()}. {sa.GetAmount()} finns kvar på kontot.");
                        }
                        else
                        {
                            Console.WriteLine("Det gick inte att ta ut mängden pengar från kontot.");
                            Console.WriteLine("Försökte du ta ut för mycket pengar?");
                        }
                    }
                    else Console.WriteLine("Du skrev inte in ett giltigt tal, försök igen");
                } while (!ParseSuccess);
                Console.WriteLine(" ");
                Console.WriteLine("Vill du se yttligare en kund? ");
                answer = Console.ReadLine().ToLower();
            }

        }


        static void Main()
        {
            BankLogic b = new();
            long[] ssn = {
                234823,
                373287,
                585932,
                174783,
                714822,
                327147
            };
            b.AddCustomer("John Andersson", ssn[0]);
            b.AddCustomer("Petter Eriksson", ssn[1]);
            b.AddCustomer("Elin Johansson", ssn[2]);
            b.AddCustomer("Niklas Andersson", ssn[3]);
            b.AddCustomer("Roger Linusson", ssn[4]);
            b.AddCustomer("Ander Johan Backe", ssn[5]);
            Random r = new();
            foreach (Customer c in b.GetListOfCustomers())
            {
                for (int i = 0; i < r.Next(1, 15); i++)
                {
                    b.AddSavingsAccount(c.SSN);
                }
            }

            bool loop = true;
            while (loop)
            {
                StringBuilder sb = new();
                sb.AppendLine("0. Avsluta");
                sb.AppendLine("1. Skriv ut en lista med bankens kunder (personnummer, för och efternamn) till en textfil");
                sb.AppendLine("2. Lägg till en ny kund med ett unikt personnummer");
                sb.AppendLine("3. Ändra en kunds namn (Personnummer ska inte kunna ändras )");
                sb.AppendLine("4. Ta bort en befintlig kund, befintliga konton måste också avslutas");
                sb.AppendLine("5. Skapa sparkonto till en befintlig kund.");
                sb.AppendLine("6. Avsluta konto");
                sb.AppendLine("7. Se information om vald kund");
                sb.AppendLine("8. Sätta in pengar på ett konto");
                sb.AppendLine("9. Ta ut pengar från ett konto (om saldot täcks)");
                Console.WriteLine(sb);
                sb.Length = 0;
                List<string> customers = b.getAllCustomers();
                switch (Console.ReadKey(intercept: true).KeyChar)
                {

                    case '1':
                        Case1(b);
                        break;
                    case '2':
                        Case2(b);
                        break;
                    case '3':
                        Case3(b);
                        break;
                    case '4':
                        Case4(b);
                        break;
                    case '5':
                        Case5(b);
                        break;
                    case '6':
                        Case6(b);
                        break;
                    case '7':
                        Case7(b);
                        break;
                    case '8':
                        Case8(b);
                        break;
                    case '9':
                        Case9(b);
                        break;
                    case '0':
                        loop = false;
                        break;

                }
            }
        }
    }
}
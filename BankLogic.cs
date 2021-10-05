﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank
{

    class BankLogic
    {
        readonly private List<Customer> ListOfCustomers = new();

        public List<Customer> GetListOfCustomers() {
            return ListOfCustomers;
        }
        public Customer CustomerHelper(long ssn)
        {
            IEnumerable<Customer> tempCustList = ListOfCustomers.Where(c => c.SSN == ssn);
            if (tempCustList.Count() == 1)
            {
                return tempCustList.First();
            }
            else return null;
        }

        public SavingsAccount AccountHelper(Customer c, int accountId) {
            IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
            if (temp.Count() == 1)
            {
                return temp.First();
            }
            else return null;
        }

        public List<String> GetCustomers()
        {
            List<string> returnList = new();
            ListOfCustomers.ForEach(cust => returnList.Add("Personnummer: " + cust.SSN + ", Namn: " + cust.FullName));
            return returnList;
        }

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
            else { 
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

        public bool ChangeCustomerName(String name, long pNr) {

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
                return tempList;
            }
            else return null;
        }

        public int AddSavingsAccount(long pNr) {
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
                else return "There was no account with that account-number.";
            }
            else return "There was no user with that Social Security Number";
        }
        public List<string> getAllCustomers()
        {
            List<string> tempString = new();
            foreach (Customer c in GetListOfCustomers())
            {
                GetCustomer(c.SSN).ForEach(line=>tempString.Add(line));
            }
            return tempString;
        }



        static void Main()
        {
            BankLogic b = new();
            long ssn = 2827328;
            b.AddCustomer("John Andersson", ssn);
            Customer c = b.CustomerHelper(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            while (true)
            {
                StringBuilder sb = new("Välj scenario");
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

                switch (Console.ReadKey().KeyChar)
                {
                    case '1':

                        break;
                    case '2':
                        break;
                    case '3':
                        break;
                    case '4':
                        break;
                    case '5':
                        break;
                    case '6':
                        break;
                    case '7':
                        break;
                    case '8':
                        break;
                    case '9':
                        break;
                }
            }
        }
    }
}

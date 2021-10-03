using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    class BankLogic
    {
        readonly List<Customer> ListOfCustomers = new();

        private Customer InternalGetCustomer(long ssn)
        {
            IEnumerable<Customer> tempCustList = ListOfCustomers.Where(c => c.SSN == ssn);
            if (tempCustList.Count() == 1)
            {
                return tempCustList.First();
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
                FirstName = name[0..SpaceIndex];
                LastName = name[(SpaceIndex + 1)..name.Length];
            }
            else { 
                FirstName = name;
                LastName = "";
            }
            Customer c = InternalGetCustomer(pNr);
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
            Customer c = InternalGetCustomer(pNr);
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

            Customer c = InternalGetCustomer(pNr);
            if (null != c)
            {
                int SpaceIndex = name.LastIndexOf(" ");
                string FirstName;
                string LastName;
                if (SpaceIndex > 0)
                {
                    FirstName = name[0..SpaceIndex];
                    LastName = name[(SpaceIndex + 1)..name.Length];
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
            Customer c = InternalGetCustomer(pNr);
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
            Customer c = InternalGetCustomer(pNr);
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

        public string GetAccount(long pNr, int accountId){
            Customer c = InternalGetCustomer(pNr);
            if (null != c)
            {
                IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
                if (temp.Count() == 1)
                {
                    SavingsAccount sa = temp.First();
                    return sa.ShowAccount();
                }
                else return null;
            }
            else return null;
        }

        public Boolean Deposit(long pNr, int accountId, decimal amount)
        {
            Customer c = InternalGetCustomer(pNr);
            if (null != c)
            {
                IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
                if (temp.Count() == 1)
                {
                    SavingsAccount sa = temp.First();
                    sa.DepositAmount(amount);
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public Boolean Withdraw(long pNr, int accountId, decimal amount)
        {
            Customer c = InternalGetCustomer(pNr);
            if (null != c)
            {
                IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
                if (temp.Count() == 1)
                {
                    SavingsAccount sa = temp.First();
                    return sa.WithdrawAmount(amount);
                }
                else return false;
            }
            else return false;
        }

        public string CloseAccount(long pNr, int accountId)
        {
            Customer c = InternalGetCustomer(pNr);
            if (null != c)
            {
                IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
                if (temp.Count() == 1)
                {
                    c.GetListOfAccounts().Remove(temp.First());
                }
            }
        }

/*
        public void CreateCustomerDialog() {
            bool correctInfo = false;
            string  FirstName = "";
            string  LastName = "";
            long    SSN = 0;
            while (!correctInfo)
            {
                bool correctName = false;
                while (!correctName)
                {
                    Console.WriteLine("Ny Kund:");
                    Console.WriteLine("Vilket namn?");
                    string inputString = Console.ReadLine();
                    if ((inputString.Trim().Length > 0))
                    {

                    }
                }
            }
        }
*/
        static void Main()
        {
            BankLogic b = new ();
            long ssn = 8711110299;
            if (b.AddCustomer("Peter Erik Eriksson", ssn))
            {
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
                b.AddSavingsAccount(ssn);
            }
            b.Deposit(ssn, 1005, 2000);
            b.Deposit(ssn, 1001, 93029);
            b.Deposit(ssn, 1002, 54200);
            Console.WriteLine(b.Withdraw(ssn, 1005, 1005));
            b.RemoveCustomer(ssn).ForEach(line => Console.WriteLine(line));

        }
    }
}

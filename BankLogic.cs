using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{

    class BankLogic
    {
        private Customer LoadedCustomer;
        private SavingsAccount LoadedAccount;

        public SavingsAccount GetCurrentAccount()
        {
            return LoadedAccount;
        }

        public void SetCurrentAccount(SavingsAccount sa)
        {
            LoadedAccount = sa;
        }

        public Customer GetCurrentUser() {
            return LoadedCustomer;
        }
        public void SetCurrentUser(Customer c) {
            LoadedCustomer = c;
        }
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

    }
}

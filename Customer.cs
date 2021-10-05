using System.Collections.Generic;

namespace Bank
{
    class Customer
    {
        public long SSN { get; }
        private string lastName;
        private string firstName;
        private readonly List<SavingsAccount> listOfAccounts;

        public List<SavingsAccount> GetListOfAccounts()
        {
            return listOfAccounts;
        }

        public string FullName => GetFirstName() + " " + GetLastName();

        public string GetFirstName()
        {
            return firstName;
        }

        public void SetFirstName(string value)
        {
            firstName = value;
        }

        public string GetLastName()
        {
            return lastName;
        }

        public void SetLastName(string value)
        {
            lastName = value;
        }

        public Customer(string FirstName, string LastName, long ssn)
        {
            this.SetFirstName(FirstName);
            this.SetLastName(LastName);
            this.SSN = ssn;
            listOfAccounts = new();
        }
         
        public List<string> PrintAccounts()
        {
            List<string> tempList = new();
            foreach (var account in GetListOfAccounts())
            {
                tempList.Add(account.ShowAccount());
            }
            return tempList;
        }
    }
}

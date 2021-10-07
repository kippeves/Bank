using System.Collections.Generic;

namespace Bank
{
    class Customer
    {
        public long SSN { get; }
        private string lastName;
        private string firstName;
        private readonly List<SavingsAccount> listOfAccounts;

        /// <summary>
        /// Hämtar en lista med bankkonton.
        /// </summary>
        /// <returns>List<Bank></returns>
        public List<SavingsAccount> GetListOfAccounts()
        {
            return listOfAccounts;
        }

        /// <summary>
        /// Skriver ut en kunds för- och efternamn.
        /// </summary>
        public string FullName => GetFirstName() + " " + GetLastName();

        /// <summary>
        /// Hämtar en kunds förnamn
        /// </summary>
        /// <returns></returns>
        public string GetFirstName()
        {
            return firstName;
        }

        /// <summary>
        /// Sätter en kunds förnamn
        /// </summary>
        /// <param name="value"></param>
        public void SetFirstName(string value)
        {
            firstName = value;
        }

        /// <summary>
        /// Hämtar en kunds efternamn.
        /// </summary>
        /// <returns></returns>
        public string GetLastName()
        {
            return lastName;
        }

        /// <summary>
        /// Sätter en kunds efternamn
        /// </summary>
        /// <param name="value"></param>
        public void SetLastName(string value)
        {
            lastName = value;
        }

        /// <summary>
        /// Skapar en bank-kund
        /// </summary>
        /// <param name="FirstName">Förnamnet på en kund</param>
        /// <param name="LastName">Efternamnet på en kund</param>
        /// <param name="ssn">Personnummer på en kund</param>
        public Customer(string FirstName, string LastName, long ssn)
        {
            this.SetFirstName(FirstName);
            this.SetLastName(LastName);
            this.SSN = ssn;
            listOfAccounts = new();
        }
         
        /// <summary>
        /// Skriver ut en lista över en kunds konton
        /// </summary>
        /// <returns>Ett List-objekt med strängar.</returns>
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

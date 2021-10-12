using System;

namespace Bank
{
    class SavingsAccount
    {
        private decimal                     Amount;
        private readonly double             Interest = 1;
        private readonly string             AccountType = "Savings";
        private readonly int                AccountNo;

        /// <summary>
        /// Skapar ett bankkonto
        /// </summary>
        /// <param name="AccountNo">Ett bankkontonummer.</param>
        public SavingsAccount(int AccountNo)
        {
            this.AccountNo = AccountNo;
        }


        /// <summary>
        /// Sätter in pengar på ett konto
        /// </summary>
        /// <param name="amount">Ett hel- eller decimaltal som skall sättas in.</param>
        public void DepositAmount(decimal amount)
        {
            this.Amount += amount;
        }

        /// <summary>
        /// Tar ut pengar från ett konto.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>Om summan är mindre eller lika med mängden i kontot så returneras true, annars false.</returns>
        public bool WithdrawAmount(decimal amount)
        {
        // Hämtar sant om summan som personen vill hämta ut resulterar i mer än eller lika med noll.
            if ((this.Amount - amount) >= 0)
            {
                this.Amount -= amount;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Hämtar räntan för kontot.
        /// </summary>
        /// <returns></returns>
        public double GetInterest() 
        {
            return Interest;
        }

        /// <summary>
        /// Hämtar värdet pengar som finns i kontot.
        /// </summary>
        /// <returns>Ett tal av typen Decimal.</returns>
        public decimal GetAmount()
        {
            return Amount;
        }

        /// <summary>
        /// Räknar ut räntevärdet för ett konto
        /// </summary>
        /// <returns></returns>
        public Decimal CalculateInterest()
        {
            // Räknar ut räntevärdet av kontosumman
            return (this.Amount * ((decimal)Interest) / 100);
        }

        /// <summary>
        /// Hämtar kontonumret för ett konto
        /// </summary>
        /// <returns>Ett kontonummer i heltalsform</returns>
        public int GetAccountNo() {
            return AccountNo;
        }

        /// <summary>
        /// Vilken kontotyp som kontot har
        /// Kontotyp är för tillfället låst på "Savings"
        /// </summary>
        /// <returns>En sträng med kontotyp</returns>
        public string GetAccountType()
        {
            return AccountType;
        }

        /// <summary>
        /// Visar information om kontot.
        /// </summary>
        /// <returns></returns>
        public string ShowAccount()
        {
            return AccountNo + "\t" + Amount + "\t" + AccountType + "\t" + CalculateInterest();
        }
    }

}

using System;

namespace Bank
{
    class SavingsAccount
    {
        private decimal             Amount;
        private readonly double Interest = 1;
        private readonly string     AccountType = "Savings";
        private readonly int                 AccountNo;

        public SavingsAccount(int AccountNo)
        {
            this.AccountNo = AccountNo;
        }

        public void DepositAmount(decimal amount)
        {
            this.Amount += amount;
        }

        public bool WithdrawAmount(decimal amount)
        {
            
            if ((this.Amount - amount) >= 0)
            {
                this.Amount -= amount;
                return true;
            }
            else return false;
        }

        public double GetInterest() 
        {
            return Interest;
        }

        public decimal GetAmount()
        {
            return Amount;
        }

        public Decimal CalculateInterest()
        {
            return ((this.Amount * (decimal)Interest) / 100);
        }

        public int GetAccountNo() {
            return AccountNo;
        }

        public string GetAccountType()
        {
            return AccountType;
        }

        public string ShowAccount()
        {
            return AccountNo + "\t" + Amount + "\t" + AccountType + "\t" + CalculateInterest();
        }
    }

}

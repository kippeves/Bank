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
        SELECT,
        DELETE
    }

    public class ConsoleGUI
    {


        static void Main()
        {
            BankLogic b = new();
/*            long ssn = 2827328;
            b.AddCustomer("John Andersson", ssn);
            Customer c = b.CustomerHelper(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.AddSavingsAccount(ssn);
            b.LoadedCustomer = c;
            b.LoadedAccount = null;*/
            while (true)
            {
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

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
            // Hämtar ut en lista med kunder som stämmer överens med det personnumret som söks efter i funktionsanropet.
            // HELST så bör endast en kund hittas, annars har någonting gått fel. 
            IEnumerable<Customer> tempCustList = ListOfCustomers.Where(c => c.SSN == ssn);
            // För att säkerställa att det inte skickas tillbaka någonting när det har hittats mer än en kund (som sagt, det BÖR inte hända).
            if (tempCustList.Count() == 1)
            {
                // Hämta tillbaka första objektet i listan, alltså, den enda kunden som hittades. Det går säkert satt lösa snyggare, men jag vet inte hur man gör
                return tempCustList.First();
            }
            else return null; // Skicka tillbaka ett nullvärde för felhantering senare
        }


        /// <summary>
        /// Hämtar ett sparkonto-objekt. Om objektet inte finns så returneras null.
        /// </summary>
        /// <param name="c">Ett kundobjekt.</param>
        /// <param name="accountId">Ett kontonummer i heltalsform</param>
        /// <returns></returns>
        public SavingsAccount AccountHelper(Customer c, int accountId)
        {
            // Hämtar ut de konton som hittades med kontonumret som söks efter i funktionsanropet.
            // Samma sak om i kundsökningsfunktionen, då det kollas om det finns ett konto med liknande nummer bör endast ett finnas.
            IEnumerable<SavingsAccount> temp = c.GetListOfAccounts().Where(item => item.GetAccountNo() == accountId);
            // Om det endast hittades ett objekt i sökningen
            if (temp.Count() == 1)
            {
                //Skicka tillbaka objektet. Det går säkert att lösa med temp[0] men jag är nöjd med den lösningen.
                return temp.First();
            }
            else return null; // Skickar tillbaka ett nullvärde för felhantering senare.
        }

        /// <summary>
        /// Hämtar en lista med information om alla kunder.
        /// </summary>
        /// <returns>En lista med strängar.</returns>
        public List<String> GetCustomers()
        {
            // Skapar ett listobjekt för att lägga till information i.
            List<string> returnList = new();
            // Lägger till "Personnummer: (En kunds personnummer), Namn: (Kundens namn)" och returnerar lista.
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
            // Då jag förstod det som att man skulle skicka in ett namn som en textsträng som skulle hanteras så byggde jag (Kristian)
            // följande anordning.
            
            // Den söker upp den sista förekomsten av ett mellanslag i namnet
            int SpaceIndex = name.LastIndexOf(" ");
            string FirstName;
            string LastName;
            // Om det finns ett mellanslag i namnet
            if (SpaceIndex > 0)
            {
                // Bryt namnet på det sista mellanslaget. Det betyder att man endast får ha ett efternamn, och hur många förnamn man vill.
                FirstName = name[0..SpaceIndex].Trim();
                // It's a feature, not a bug.
                LastName = name[(SpaceIndex + 1)..name.Length].Trim();
            }
            else
            {
                // Om man inte har något efternamn så sätts hela namnet i förnamnsfältet, och det sista fältet är tomt.
                FirstName = name;
                LastName = "";
            }
            // Hämta ut en kund med personnumret man söker efter.
            Customer c = CustomerHelper(pNr);
            // Om det inte finns någon kund med det personnumret (funktionen skickar tillbaka null om den inte hittar någon kund)
            if (null == c)
            {
                // Skapa en ny kund med det delade namnet, och med personnumret som skickades in i anropet.
                c = new(FirstName, LastName, pNr);
                // Lägg till kunden i bankens kundlista. Japp, direktanrop rakt in i listan.
                ListOfCustomers.Add(c);
                return true;
            }
            // Om det inte gick att göra, d.v.s om det redan fanns en kund med det personnumret, skicka tillbaka false för felhantering.
            else return false;
        }

        /// <summary>
        /// Hämtar information om en kund i textformat.
        /// </summary>
        /// <param name="pNr">Ett personnummer.</param>
        /// <returns></returns>
        public List<string> GetCustomer(long pNr)
        {
            // Skapar en lista att lägga till textobjekt i
            List<string> TempList = new();
            // Kollar om det finns någon kund.
            Customer c = CustomerHelper(pNr);
            // Om det hittades en kund, d.v.s om variabeln har ett värdet sparat.
            if (null != c)
            {
                // Lägg till Namn, Personnummer och tillhörande konton om kunder i listan.
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
            // Söker efter en kund i banksystemet med tillhörande personnummer.
            Customer c = CustomerHelper(pNr);
            // Om det hittades en kund, d.v.s om värdet har ett värde sparat i sig.
            if (null != c)
            {
                // Spara vilken plats det sista mellanslaget i namnet ligger någonstans.
                int SpaceIndex = name.LastIndexOf(" ");
                string FirstName;
                string LastName;

                // Om det finns ett mellanslag i namnet
                if (SpaceIndex > 0)
                {
                    // Dela upp för och efternamn.
                    FirstName = name[0..SpaceIndex].Trim();
                    LastName = name[(SpaceIndex + 1)..name.Length].Trim();
                }
                else
                {
                    FirstName = name;
                    LastName = "";
                }
                // Sätt för- och efternamn.
                c.SetFirstName(FirstName);
                c.SetLastName(LastName);
                return true;
            }
            // Om det inte hittades någon kund, skicka tillbaka för felhantering
            else return false;
        }

        /// <summary>
        /// Tar bort en kund i banksystemet.
        /// </summary>
        /// <param name="pNr">Personnumret för kunden som skall tas bort.</param>
        /// <returns></returns>
        public List<string> RemoveCustomer(long pNr)
        {
            // Sök efter kund med personnummer
            Customer c = CustomerHelper(pNr);
            // Om det hittades en kund
            if (null != c)
            {
                List<string> tempList = new();
                // Hämta information om kund. Funktionen hämtar ut alla informationsfält som returneras i funktionen för kunden.
                tempList.AddRange(GetCustomer(pNr));
                // Hämtar ut en lista av alla konton som kunden äger.
                List<SavingsAccount> accounts = c.GetListOfAccounts();
                // Om det finns minst ett konto
                if (accounts.Count > 0)
                {
                    decimal sum = 0;
                    decimal sumOfInterest = 0;

                    // Skriver ut summan av värdet för alla konton som kunden har.
                    accounts.ForEach(a => sum += a.GetAmount());
                    tempList.Add($"Sum of accounts:\t" + sum);
                    // Skriver ut summan för all ränta över alla konton som kunden äger.
                    accounts.ForEach(a => sumOfInterest += a.CalculateInterest());
                    tempList.Add($"Sum of interest:\t" + sumOfInterest);
                    // Tar bort alla konton i kundens 
                    accounts.Clear();

                }
                else tempList.Add("You have no accounts...");
                // Ta bort kunden ur kundlistan
                GetListOfCustomers().Remove(c);
                return tempList;
            }
            // Om inte kunden hittades, så skicka tillbaka null för felhantering
            else return null;
        }


        /// <summary>
        /// Lägger till ett sparkonto för en kund.
        /// </summary>
        /// <param name="pNr">Personnumret för den kund som skall få ett konto.</param>
        /// <returns></returns>
        public int AddSavingsAccount(long pNr)
        {
            // Hämtar en kund med sökt personnummer
            Customer c = CustomerHelper(pNr);
            // Om det finns en kund
            if (null != c)
            {
                // Om kunden har minst en lista
                if (c.GetListOfAccounts().Count > 0)
                {
                    // hämtar id-numret för det sista kontot i listan
                    int id = c.GetListOfAccounts().Last().GetAccountNo();
                    // Skapar ett nytt konto med id-numret ökat med ett ifrån tidigare konto (Om det första är 1001, så är nästa 1002, o.s.v)
                    c.GetListOfAccounts().Add(new SavingsAccount(++id));
                    // Skicka tillbaka det id-nummer som genererades
                    return id;
                }
                else
                {
                    // Om det inte finns något konto så skapa det första kontot med id-nummer 1001;
                    c.GetListOfAccounts().Add(new SavingsAccount(1001));
                    // Eftersom vi vet att det är 1001 kan vi lika gärna skicka tillbaka det.
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

            // Hämtar en kund ur banksystemet med sökt personnummer
            Customer c = CustomerHelper(pNr);
            // Om det finns en kund
            if (null != c)
            {
                // Sök efter konto för hittad kund, oh efterfrågat kontonummer
                SavingsAccount sa = AccountHelper(c, accountId);
                // Om funktionen hittar ett konto
                if (null != sa)
                {
                    // Skicka tillbaka textsträngen med beskrivning om kontot.
                    return sa.ShowAccount();
                }
                // Annars skicka tillbaka null för felhantering senare.
                else return null;
            }
            // Skickar tillbaka en textsträng för skojs skull.
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
            // Söker efter en kund i banksystemet med sökt personnummer
            Customer c = CustomerHelper(pNr);
            // Om du hittar en kund
            if (null != c)
            {
                // Sök efter konto i systemet för kund och lagra i en variabel.
                SavingsAccount sa = AccountHelper(c, accountId);
                // Om du hittar ett konto
                if (null != sa)
                {
                    // Lägg till mängden pengar på kontot.
                    sa.DepositAmount(amount);
                    return true;
                }
                // Om du inte hittar något konto med det numret
                else return false;
            }
            // Eller hittar någon kund
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
            // Sök efter kund med efterfrågat personnummer
            Customer c = CustomerHelper(pNr);
            // Om du hittar en kund
            if (null != c)
            {
                // Sök efter ett konto för kund med efterfrågat konto-id.
                SavingsAccount sa = AccountHelper(c, accountId);
                // Om du hittar ett konto
                if (null != sa)
                {
                    // Ta ut pengar. I funktionen finns felhantering för övertrassering.
                    // Vid övertrassering returneras false.
                    return sa.WithdrawAmount(amount);
                }
                // Om du inte hittar något konto
                else return false;
            }
            // Om du inte hittar någon kund.
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
            // Sök efter kund i banksystemet med personnummer
            Customer c = CustomerHelper(pNr);
            // Om du hittar en kund
            if (null != c)
            {
                // Sök efter ett konto för hittad kund med efterfrågat kontonummer
                SavingsAccount sa = AccountHelper(c, accountId);
                // Om du hittar ett konto
                if (null != sa)
                {
                    // Sparar information om kontot i en strängvariabel
                    string returnString = sa.ShowAccount();
                    // Ta bort kontot ur listan
                    c.GetListOfAccounts().Remove(sa);
                    // Skicka tillbaka strängen
                    return returnString;
                }
                // Om du inte hittar något konto
                else return null;
            }
            // Om du inte hittar någon kund
            else return null;
        }
        /// <summary>
        /// Hämtar en lista med alla kunder i textformat.
        /// </summary>
        /// <returns></returns>
        public List<string> getAllCustomers()
        {
            // Skapar en tom lista att lagra text i
            List<string> tempString = new();
            // För varje kund i kundlistan
            foreach (Customer c in GetListOfCustomers())
            {
                // Söker information, och lägger till informationen i textlistan.
                // Inser att en AddRange skulle fungera med.
                GetCustomer(c.SSN).ForEach(line => tempString.Add(line));
            }
            // Skicka tillbaka en lista med information
            return tempString;
        }


        /* *
         * 
         *  Skriv ut en lista med bankens kunder (personnummer, för och efternamn) till en textfil
         * 
         * */
        public static void Case1(BankLogic b)
        {
            // Skapar en ny sträng
            StringBuilder sb = new();
            // Rensar konsollen
            Console.Clear();
            Console.WriteLine("Skapar textfilen \"list.txt\" i mappen som .EXE-filen körs i.");
            //Lägger till informationen om alla kunder, lägger till i listan med text.
            foreach (string line in b.getAllCustomers())
            {
                sb.AppendLine(line);
            }
            Console.WriteLine(sb);
            // Sparar strängen bokstavligt.
            string path = @"list.txt";
            // Skapar en filvariabel och öppnar en ström att skriva till
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(sb);
            }
            // Skriver ut information och pausar för att få information.
            Console.WriteLine($"Lista skriven till {path}");
            Console.ReadLine();
            Console.Clear();
        }

        /***
         * 
         * Lägg till en ny kund med ett unikt personnummer
         * 
         * */

        public static void Case2(BankLogic b)
        {
            string name;
            bool correctSSN;
            long pNr;
            // Skapar en ny textvariabel
            StringBuilder sb = new();
            // Frågar efter namn så länge man man inte skriver någonting.
            // Den tar bort mellanslag.
            do
            {
                Console.WriteLine("Vad är ditt namn?");
                name = Console.ReadLine();
            } while (name.Trim().Length == 0);
            Console.WriteLine();
            // Frågar efter personnummer
            do
            {
                // Frågar efter personnummer så länge det inte 
                Console.WriteLine("Vad är ditt personnummer?");
                // Tolkar strängen till en int.
                correctSSN = long.TryParse(Console.ReadLine(), out pNr);
                // Om du inte skrev rätt personnummer
                if (!correctSSN)
                {
                    Console.WriteLine("Du skrev inte in ditt nummer korrekt. Vänligen försök igen");
                }
                // Gör loopen så länge du inte skriver rätt personnummer
            } while (!correctSSN);
            sb.AppendLine("Denna information kommer att sparas:");
            sb.AppendLine($"Namn:\t{name}");
            sb.AppendLine($"Personnummer:\t{pNr}");
            Console.WriteLine(sb);
            Console.WriteLine();
            // Om det lyckades att lägga till användare.
            bool registeredUser = b.AddCustomer(name, pNr);
            // Om det lyckades att lägga till användare.
            if (registeredUser)
            {
                Console.WriteLine("Din användare lades till i listan.");
            }
            else // Eller inte lyckades.
            {
                Console.WriteLine("Du kunde inte läggas till i systemet");
                Console.WriteLine("Do you already have a user with that Social Security Number registered?");
            }
            Console.WriteLine();
        }


        /***
         * 
         * Ändra en kunds namn (Personnummer ska inte kunna ändras )
         * 
         * */

        public static void Case3(BankLogic b)
        {
            bool correctSSN;
            string name;
            long pNr;

            Console.WriteLine("Vilken användare vill du ändra namn på?");
             // Skriver ut information om alla användare i systemet;
             // Mest för användarläsbarhet.
            foreach(Customer c in b.GetListOfCustomers())
            {
                Console.WriteLine($"{c.SSN}: {c.FullName}");
            }


            // Söker efter personnummer
            do
            {
                Console.WriteLine("What is your social security number?");
                correctSSN = long.TryParse(Console.ReadLine(), out pNr); // Kollar om man har rätt typ av information.
                if (!correctSSN)
                {
                    Console.WriteLine("You did not enter your number correctly. Please try again."); // Skriver ut felmeddelande
                }
            } while (!correctSSN);
            do
            {
                Console.WriteLine("What name do you want to change to?");
                name = Console.ReadLine();
            }
            while (name.Trim().Length == 0); // Frågar efter namnet så länge man inte skriver någonting, eller om man bara skriver mellanslag.
            b.ChangeCustomerName(name, pNr); // Byt namn på kunden.
        }

        /***
         * 
         * Ta bort en befintlig kund, befintliga konton måste också avslutas
         * 
         */

        public static void Case4(BankLogic b)
        {
            //Skriver ut information om alla kunder i banksystemet.
            foreach (var item in b.getAllCustomers())
            {
                Console.WriteLine(item);
            }

            string answer = "";
            while (answer != "nej") // Så länge användaren ber om att få söka efter en kund till
            {
                Console.Write("Mata in personnummret på kunden du vill ta bort: ");
                long pNr = long.Parse(Console.ReadLine()); // Spara personnumret
                List<string> s;
                if ((s = b.RemoveCustomer(pNr)) != null) // Sparar informationen om borttagna kunden i variabeln.
                {
                    Console.WriteLine("Du tog bort denna kund:");
                    Console.WriteLine("");
                    foreach (var item in s) // Och skriver ut informationen.
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

                foreach (var item in b.getAllCustomers()) // Hämtar information om alla kunder så kunden har möjlighet att söka igen.
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("");
                Console.WriteLine("Vill du ta bort yttligare en kund? ja/nej");
                answer = Console.ReadLine().ToLower(); // Ger kunden en möjlighet att söka igen.
            }
        }

        /***
         * 
         * Skapa sparkonto till en befintlig kund.
         * 
         * */

        public static void Case5(BankLogic b)
        {
            string answer = "";
            while (answer != "nej") // Så länge kunden vill söka efter information
            {
                foreach (var item in b.getAllCustomers()) // Skriv ut information alla kunder
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("");
                Console.Write("Mata in personnummer på den kund som du vill skapa ett nytt konto åt: ");
                long svar3 = long.Parse(Console.ReadLine());

                int status = b.AddSavingsAccount(svar3); // Hämtar information om det gick att skapa ett konto

                if (status == -1)
                {
                    Console.WriteLine("Felaktigt personnummer.");
                }
                else
                {
                    Console.WriteLine($"Ett sparkonto skapades. Kontonummer {status}");
                }

                foreach (var item in b.getAllCustomers()) // Skriver ut information om alla kunder.
                {
                    Console.WriteLine(item);
                }

                Console.Write("Vill du skapa yttligare ett sparkonto åt en befintlig kund? ja/nej ");
                answer = Console.ReadLine().ToLower(); // Frågar kunden efter beslut.
            }
        }


        /***
         * 
         * Avsluta konto
         * 
         * */
        public static void Case6(BankLogic b)
        {
            string answer = "";
            while (answer != "nej") // Så länge det finns kunder att avsluta.
            {
                foreach (var item in b.getAllCustomers()) // Skriv ut information om alla kunden.
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("");
                Console.Write("Mata in det personnummer till det kontot du vill avsluta: ");
                long svar6 = long.Parse(Console.ReadLine()); // Tar in personnummer för hantering

                Console.WriteLine(" ");

                Console.Write("Mata in konto Id till kontot du vill avsluta: ");
                int svar7 = int.Parse(Console.ReadLine()); // Tar in kontonummer för hantering
                string status = b.CloseAccount(svar6, svar7); // Kollar om det går att stänga kontot.

                if (status != null) // Om det gick att ta bort konto
                {
                    Console.WriteLine("Ditt konto har tagits bort.");
                }
                else //Annars skriv ut felmeddelande.
                {
                    Console.WriteLine("Felaktig inmatning!");
                }

                Console.WriteLine("Vill du ta bort yttligare ett konto? ja/nej");
                answer = Console.ReadLine().ToLower();
            }
        }

        /***
         * 
         * Se information om vald kund
         * 
         * */

        public static void Case7(BankLogic b)
        {
            string answer = "";
            while (answer != "nej") // Så länge användare vill söka information
            {
                foreach (var item in b.getAllCustomers()) // Skriv ut information om alla användare.
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine(" ");
                Console.Write("Ange kundens personnummer som du vill se information om? ");
                long svar8 = long.Parse(Console.ReadLine()); // Hämtar svaret

                Customer s = b.CustomerHelper(svar8); // Försöker hämta kund efter sökt personnummer
                if (s == null) // Om det inte hittades någon kund.
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

        /***
         * 
         * Sätta in pengar på ett konto
         * 
         * */
        public static void Case8(BankLogic b)
        {
            string answer = "";
            Customer c = null;
            long ssn = 0;
            SavingsAccount sa = null;

            while (answer != "nej") // Så länge användaren vill utföra insättningar.
            {
                foreach (var item in b.getAllCustomers()) // Skriv ut information om alla kunder.
                {
                    Console.WriteLine(item);
                }
                
                do
                {
                    Console.WriteLine("Skriv personnummer för kund:");
                    if (long.TryParse(Console.ReadLine(), out ssn)){
                        c = b.CustomerHelper(ssn); // Söker efter kund på skrivet personnnummer. Sparar kund om den hittas.
                    }
                    else
                    {
                        Console.WriteLine("Det finns ingen kund med det personnumret");
                    }
                } while (c==null);
                Console.WriteLine("test");
                do
                {
                    // Frågar efter konto.
                    Console.WriteLine("Vilket konto vill du välja?");
                    int account;
                    if (int.TryParse(Console.ReadLine(), out account))
                    {
                        // Sparar konto om hittas. Annars sparas null.
                        sa = b.AccountHelper(c, account);
                    }
                } while (sa == null); // Så länge det inte är något giltigt konto.

                Console.WriteLine("Hur mycket pengar vill du sätta in?");
                Decimal tempDec = 0;
                bool ParseSuccess;
                do
                {
                    ParseSuccess = Decimal.TryParse(Console.ReadLine(), out tempDec); // Försöker läsa strängen, och sparar mängden med pengar till en variabel..
                    if (ParseSuccess)
                    {
                        b.Deposit(c.SSN, sa.GetAccountNo(), tempDec); // Om det lyckades, sätt in pengar på kontot.
                    }
                    else Console.WriteLine("Du skrev inte in ett giltigt tal, försök igen"); // Annars skrivs ett felmeddelande ut.
                } while (!ParseSuccess);
                Console.WriteLine($"{tempDec} sattes in på konto {sa.GetAccountNo()}, tillhörande {c.FullName}"); // Skriv ut information
                Console.WriteLine(" ");
                Console.WriteLine("Vill du se yttligare en kund? ");
                answer = Console.ReadLine().ToLower(); // Ge kunden en möjlighet att söka igen.
            }
        }

        /***
         * 
         * Ta ut pengar från ett konto (om saldot täcks)
         * 
         * */
        public static void Case9(BankLogic b){
            string answer = "";
            bool ParseSuccess = false;
            while (answer != "nej") // Så länge kunden vill utföra handlingar.
            {
                long tempValue;
                foreach (var item in b.getAllCustomers()) // Skriv ut information om alla kunder i banksystemet.
                {
                    Console.WriteLine(item);
                }
                Customer c; // Skapar en tom variabel att spara i
                do
                {
                    Console.WriteLine("Skriv personnummer för kund:");
                    ParseSuccess = long.TryParse(Console.ReadLine(), out tempValue);
                    c = b.CustomerHelper(tempValue); // Försök hämta ut kund ur banksystemet och spara för senare användning.
                    if (c == null) // Om det inte fanns någon kund.
                    {
                        Console.WriteLine("Det finns ingen kund med det personnumret");
                    }
                } while (!ParseSuccess && (c == null)); // Så länge det inte gick och det inte fanns någon kund
                SavingsAccount sa;
                do
                {
                    Console.WriteLine("Skriv kontonummer för kund:");
                    ParseSuccess = int.TryParse(Console.ReadLine(), out int temp);
                    sa = b.AccountHelper(c, temp); // Försöker hämta ut efterfrågat konto för tidigare hittad kund.
                    if (sa == null)
                    { // Om det inte hittades något konto, skriv ut felmeddelande.
                        Console.WriteLine("Det finns ingen konto med det kontonumret");
                    }
                } while (!ParseSuccess && (sa == null));
                Console.WriteLine("Hur mycket pengar vill du ta ut?");
                Decimal tempDec = 0;
                do
                {
                    ParseSuccess = Decimal.TryParse(Console.ReadLine(), out tempDec);
                    if (ParseSuccess)
                    { // Försök ta ut pengar
                        if (sa.WithdrawAmount(tempDec))
                        {
                            Console.WriteLine($"{tempDec} togs ut från konto {sa.GetAccountNo()}. {sa.GetAmount()} finns kvar på kontot.");
                        } // Om det övertrasserar kontot, så skrivs ett felmeddelande ut.
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
            BankLogic b = new(); // Skapar en ny instans av banksystemet för användning
            // Finns ingen anledning att vara i array egentligen. Bara för senare användning. 
            long[] ssn = {
                234823,
                373287,
                585932,
                174783,
                714822,
                327147
            };

            // Lägger till placeholder-kunder för systemet att använda.
            b.AddCustomer("John Andersson", ssn[0]);
            b.AddCustomer("Petter Eriksson", ssn[1]);
            b.AddCustomer("Elin Johansson", ssn[2]);
            b.AddCustomer("Niklas Andersson", ssn[3]);
            b.AddCustomer("Roger Linusson", ssn[4]);
            b.AddCustomer("Ander Johan Backe", ssn[5]);
            Random r = new();
            // Skapar slumpmässigt antal bankkonton till varje kund.
            // För att hantera menyer och kundlistor.
            // 
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
                StringBuilder sb = new(); // Skapar en tom sträng och fyller i meny
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
                Console.WriteLine(sb); // Skriv ut meny
                sb.Length = 0; // rensa sträng för att använda igen.
                
                // Läser tangentbordstryck för att kunna välja varje case i menyn. Intercept true för att skriva ut knapptryckningen i konsollen.
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
                        loop = false; // Bryter ur loopen och avslutar programmet.
                        break;

                }
            }
        }
    }
}
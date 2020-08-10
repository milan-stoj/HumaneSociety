using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class Customer : User
    {
        Client client;
        public override void LogIn()
        {
            if (CheckIfNewUser())
            {
                CreateClient();
                LogInPreExistingUser();
            }
            else
            {
                Console.Clear();
                LogInPreExistingUser();
            }
            RunUserMenus();
        }
        protected override void LogInPreExistingUser()
        {
            List<string> options = new List<string>() { "Please log in", "Enter your username (CaSe SeNsItIvE)" };
            UserInterface.DisplayUserOptions(options);
            userName = UserInterface.GetUserInput();
            UserInterface.DisplayUserOptions("Enter your password (CaSe SeNsItIvE)");
            string password = UserInterface.GetUserInput();
            try
            {
                client = Query.GetClient(userName, password);
                name = client.FirstName;
            }
            catch
            {
                UserInterface.DisplayUserOptions("User not found. Please try another username, contact support or type 'reset' to restart");
                LogIn();
                return;
            }
        }
        protected override void RunUserMenus()
        {
            List<string> options = new List<string>() { "1. Search for animals", "2. Update info", "3. Apply for Adoption" };
            Console.Clear();            
            UserInterface.DisplayUserOptions(options);
            int input = UserInterface.GetIntegerData();
            RunUserInput(input);
            
        }

        private void RunUserInput(int input)
        {
            switch (input)
            {
                case 1:
                    RunSearch();
                    RunUserMenus();
                    break;
                case 2:
                    UpdateClientInfo();
                    RunUserMenus();
                    break;
                case 3:
                    ApplyForAdoption();
                    RunUserMenus();
                    break;
                default:
                    UserInterface.DisplayUserOptions("Input not accepted please try again");
                    break;
            }            
        }     

        private void ApplyForAdoption()
        {
            Console.Clear();
            UserInterface.DisplayUserOptions("Please enter the ID of the animal you wish to adopt or type reset or exit");
            int iD = UserInterface.GetIntegerData();
            var animal = Query.GetAnimalByID(iD);
            UserInterface.DisplayAnimalInfo(animal);
            UserInterface.DisplayUserOptions("Would you like to adopt?");
            if ((bool)UserInterface.GetBitData())
            {
                Query.Adopt(animal, client);
                UserInterface.DisplayUserOptions("Adoption request sent we will hold $75 adoption fee until processed");
            }
        }

        private void RunSearch()
        {
            Console.Clear();
            var updates = UserInterface.GetAnimalSearchCriteria();
            var animals = Query.SearchForAnimalsByMultipleTraits(updates).ToList();

            if (animals.Count > 1)
            {
                UserInterface.DisplayUserOptions("Several animals found");
                UserInterface.DisplayAnimals(animals);
            }
            else if(animals.Count == 0)
            {
                UserInterface.DisplayUserOptions("No animals found please try another search");
            }
            else
            {
                UserInterface.DisplayAnimalInfo(animals[0]);
            }
            UserInterface.DisplayUserOptions("Press enter to continue");
            Console.ReadLine();
        }

        public static string GetUserName()
        {
            UserInterface.DisplayUserOptions("Please enter a username");
            string username = UserInterface.GetUserInput();
            var clients = Query.GetClients();
            var clientUsernames = from client in clients select client.UserName;
            if (CheckForValue(clientUsernames.ToList(), username))
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Username already in use please try another username");
                return GetUserName();
            }
            return username;
        }
        public static bool CheckForValue<T>(List<T> items, T value)
        {
            if (items.Contains(value))
            {
                return true;
            }
            return false;
        }
        public static string GetEmail()
        {
            var clients = Query.GetClients();
            var clientEmails = from client in clients select client.Email;
            UserInterface.DisplayUserOptions("Please enter your email");
            string email = UserInterface.GetUserInput();
            if (email.Contains("@") && email.Contains("."))
            {
                if (CheckForValue(clientEmails.ToList(), email))
                {
                    Console.Clear();
                    UserInterface.DisplayUserOptions("Email already in use please try another email or contact support for forgotten account info");
                    return GetEmail();
                }
                return email;
            }
            else
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("Email not valid please enter a valid email address");
                return GetEmail();
            }

        }
        private static int GetState()
        {
            UserInterface.DisplayUserOptions("Please enter your state (abbreviation or full state name");
            string state = UserInterface.GetUserInput();
            var states = Query.GetStates();
            var stateNames = from territory in states select territory.Name.ToLower();
            var stateAbrreviations = from territory in states select territory.Abbreviation;
            if (stateNames.ToList().Contains(state.ToLower()) || stateAbrreviations.ToList().Contains(state.ToUpper()))
            {
                try
                {
                    var stateReturn = from territory in states where territory.Name.ToLower() == state.ToLower() select territory.USStateId;
                    int stateNumber = stateReturn.ToList()[0];
                    return stateNumber;
                }
                catch
                {
                    var stateReturn = from territory in states where territory.Abbreviation == state.ToUpper() select territory.USStateId;
                    int stateNumber = stateReturn.ToList()[0];
                    return stateNumber;
                }
            }
            else
            {
                Console.Clear();
                UserInterface.DisplayUserOptions("State not Found");
                return GetState();
            }
        }
        public bool CreateClient()
        {
            try
            {
                var clients = Query.GetClients();
                var clientUsernames = from client in clients select client.UserName;
                string username = GetUserName();
                string email = GetEmail();
                Console.Clear();
                UserInterface.DisplayUserOptions("Please enter password (Warning password is CaSe SeNsItIvE)");
                string password = UserInterface.GetUserInput();
                UserInterface.DisplayUserOptions("Enter your first name.");
                string firstName = UserInterface.GetUserInput();
                UserInterface.DisplayUserOptions("Enter your last name.");
                string lastName = UserInterface.GetUserInput();
                int zipCode = GetZipCode();
                int stateId = GetState();
                UserInterface.DisplayUserOptions("Please enter your street address");
                string streetAddress = UserInterface.GetUserInput();
                Query.AddNewClient(firstName, lastName, username, password, email, streetAddress, zipCode, stateId);
                Console.Clear();
                UserInterface.DisplayUserOptions("Profile successfully added");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool CreateClient(IQueryable<Client> clients)
        {
            try
            {
                var clientUsernames = from client in clients select client.UserName;
                var clientEmails = from client in clients select client.Email;
                string username = GetUserName();
                string email = GetEmail();
                if (CheckForValue(clientUsernames.ToList(), username))
                {
                    Console.Clear();
                    UserInterface.DisplayUserOptions("Username already in use please try another username");
                    return CreateClient(clients);
                }
                else if (CheckForValue(clientEmails.ToList(), email))
                {
                    Console.Clear();
                    UserInterface.DisplayUserOptions("Email already in use please try another email or contact support for forgotten account info");
                    return CreateClient(clients);
                }
                else
                {
                    Console.Clear();
                    UserInterface.DisplayUserOptions("Please enter password (Warning password is CaSe SeNsItIvE)");
                    string password = UserInterface.GetUserInput();
                    UserInterface.DisplayUserOptions("Enter your first name.");
                    string firstName = UserInterface.GetUserInput();
                    UserInterface.DisplayUserOptions("Enter your last name.");
                    string lastName = UserInterface.GetUserInput();
                    int zipCode = GetZipCode();
                    int stateId = GetState();
                    UserInterface.DisplayUserOptions("Please enter your street address");
                    string streetAddress = UserInterface.GetUserInput();
                    Query.AddNewClient(firstName, lastName, username, password, email, streetAddress, zipCode, stateId);
                    Console.Clear();
                    UserInterface.DisplayUserOptions("Profile successfully added");

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void UpdateClientInfo()
        {
            List<string> options = new List<string>() { "What would you like to update? (Please enter number of option)", "1: Name", "2: Address", "3: Email", "4: Username", "5: Password", "6. Back" };
            int input = default(int);
            while (input != 6)
            {
                try
                {
                    UserInterface.DisplayUserOptions(options);
                    input = int.Parse(UserInterface.GetUserInput());
                    RunUpdateInput(input);
                }
                catch
                {
                    UserInterface.DisplayUserOptions("Input not recognized please enter an integer number of the option you would like to update");
                }
            }

        }
        private void RunUpdateInput(int input)
        {
            switch (input)
            {
                case 1:
                    UpdateName();
                    break;
                case 2:
                    UpdateAddress();
                    break;
                case 3:
                    UpdateEmail();
                    break;
                case 4:
                    UpdateUsername();
                    break;
                case 5:
                    UpdatePassword();
                    break;
                case 6:
                    break;
                default:
                    UserInterface.DisplayUserOptions("You have reached this message in error please contact support or administator and give them code 10928849");
                    break;
            }

        }

        private void UpdatePassword()
        {
            Console.Clear();
            Console.Clear();
            UserInterface.DisplayUserOptions("Current Password: " + client.Password + " What is your new Password?");
            client.Password = UserInterface.GetUserInput();
            Query.UpdateClient(client);
        }

        private void UpdateUsername()
        {
            Console.Clear();
            UserInterface.DisplayUserOptions("Current Username: " + client.UserName);
            client.UserName = GetUserName();
            Query.UpdateClient(client);
        }

        private void UpdateEmail()
        {
            Console.Clear();
            UserInterface.DisplayUserOptions("Current email: " + client.Email);
            client.Email = GetEmail();
            Query.UpdateClient(client);
        }

        public int GetZipCode()
        {
            UserInterface.DisplayUserOptions("Please enter 5 digit zip");
            try
            {
                int zipCode = int.Parse(UserInterface.GetUserInput());
                return zipCode;
            }
            catch
            {
                UserInterface.DisplayUserOptions("Invalid Zip code please enter a 5 digit zipcode");
                return GetZipCode();
            }
        }
        public void DisplayCurrentAddress(Client client)
        {
            string address = client.Address.AddressLine1;
            string zipCode = client.Address.Zipcode.ToString();
            string state = client.Address.USState.Name;
            UserInterface.DisplayUserOptions("Current address:");
            UserInterface.DisplayUserOptions($"{address}, {zipCode}, {state}");
        }
        public void UpdateAddress()
        {
            Console.Clear();
            DisplayCurrentAddress(client);
            client.Address.Zipcode = GetZipCode();
            client.Address.USStateId = GetState();
            UserInterface.DisplayUserOptions("Please enter your street address");
            client.Address.AddressLine1 = UserInterface.GetUserInput();
            Query.UpdateClient(client);

        }
        public void UpdateName()
        {
            Console.Clear();
            List<string> options = new List<string>() { "Current Name:", client.FirstName, client.LastName, "Would you like to update?", "1. First", "2. Last", "3. Both" };
            UserInterface.DisplayUserOptions(options);
            string input = UserInterface.GetUserInput().ToLower();
            if (input == "first" || input == "1")
            {
                UserInterface.DisplayUserOptions("Please enter your new first name.");
                client.FirstName = UserInterface.GetUserInput();
                Query.UpdateClient(client);

            }
            else if (input == "last" || input == "2")
            {
                UserInterface.DisplayUserOptions("Please enter your new last name.");
                client.LastName = UserInterface.GetUserInput();
                Query.UpdateClient(client);
            }
            else
            {
                UserInterface.DisplayUserOptions("Please enter your new first name.");
                client.FirstName = UserInterface.GetUserInput();
                Query.UpdateClient(client);
                UserInterface.DisplayUserOptions("Please enter your new last name.");
                client.LastName = UserInterface.GetUserInput();
                Query.UpdateClient(client);
            }
        }
    }
}

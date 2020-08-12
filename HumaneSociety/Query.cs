using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch (crudOperation)
            {
                case "update":
                    Employee employeeToUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    if (employeeToUpdate == null) { break; }
                    employeeToUpdate.FirstName = employee.FirstName;
                    employeeToUpdate.LastName = employee.LastName;
                    employeeToUpdate.Email = employee.Email;
                    db.SubmitChanges();
                    break;
                case "read":
                    Employee employeeToRead = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    if (employeeToRead == null) { break; }
                    Console.WriteLine($"First Name: {employeeToRead.FirstName}\n" +
                        $"Last Name: {employeeToRead.LastName}\n" +
                        $"Email: {employeeToRead.Email}\n" +
                        $"Employee Number: {employeeToRead.EmployeeNumber}\n" );
                    Console.ReadLine();
                    break;
                case "delete":
                    Employee employeeToDelete = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).FirstOrDefault();
                    if (employeeToDelete == null) { break; }
                    db.Employees.DeleteOnSubmit(employeeToDelete);
                    db.SubmitChanges();
                    break;
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    db.SubmitChanges();
                    break;
            }

            // throw new NotImplementedException();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id) 
        {
            Animal animalToGet = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return animalToGet;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animalToUpdate = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            var keyList = new List<int>(updates.Keys);
            foreach (int key in keyList)
            {
                switch (key)
                {
                    case 1:
                        animalToUpdate.Category = db.Categories.Where(c => c.CategoryId == GetCategoryId(updates[key])).FirstOrDefault();
                        db.SubmitChanges();
                        break;
                    case 2:
                        animalToUpdate.Name = updates[key];
                        db.SubmitChanges();
                        break;
                    case 3:
                        animalToUpdate.Age = Convert.ToInt32(updates[key]);
                        db.SubmitChanges();
                        break;
                    case 4:
                        animalToUpdate.Demeanor = updates[key];
                        db.SubmitChanges();
                        break;
                    case 5:
                        animalToUpdate.KidFriendly = Convert.ToBoolean(updates[key]);
                        db.SubmitChanges();
                        break;
                    case 6:
                        animalToUpdate.PetFriendly = Convert.ToBoolean(updates[key]);
                        db.SubmitChanges();
                        break;
                    case 7:
                        animalToUpdate.Weight = Convert.ToInt32(updates[key]);
                        db.SubmitChanges();
                        break;
                }
            }
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> query = db.Animals;

            var keyList = new List<int>(updates.Keys);
            foreach (int key in keyList)
            {
                switch (key)
                {
                    case 1:
                        query = query.Where(a => a.CategoryId == GetCategoryId(updates[key]));
                        break;
                    case 2:
                        query = query.Where(a => a.Name == updates[key]);
                        break;
                    case 3:
                        query = query.Where(a => a.Age == Convert.ToInt32(updates[key]));
                        break;
                    case 4:
                        query = query.Where(a => a.Demeanor == updates[key]);
                        break;
                    case 5:
                        query = query.Where(a => a.KidFriendly == Convert.ToBoolean(updates[key]));
                        break;
                    case 6:
                        query = query.Where(a => a.PetFriendly == Convert.ToBoolean(updates[key]));
                        break;
                    case 7:
                        query = query.Where(a => a.Weight == Convert.ToInt32(updates[key]));
                        break;
                    case 8:
                        query = query.Where(a => a.AnimalId == Convert.ToInt32(updates[key]));
                        break;
                }
            }

            return query;
            // Ask about 'multiple traits' -- seek guidance on searching with multiple traits.
        }
        
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            return db.Categories.Where(c => c.Name == categoryName).FirstOrDefault().CategoryId;
        }
        
        internal static Room GetRoom(int animalId)
        {
            return db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            //throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            return db.DietPlans.Where(c => c.Name == dietPlanName).FirstOrDefault().DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoptionToInsert = new Adoption();
            adoptionToInsert.AnimalId = animal.AnimalId;
            adoptionToInsert.ClientId = client.ClientId;
            adoptionToInsert.ApprovalStatus = "Pending";
            adoptionToInsert.AdoptionFee = 75;
            adoptionToInsert.PaymentCollected = Convert.ToBoolean(0);
            db.Adoptions.InsertOnSubmit(adoptionToInsert);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions() // Test for multiple pending adoptions.
        {
            IQueryable<Adoption> query = from adoption in db.Adoptions
                                         where adoption.ApprovalStatus == "Pending"
                                         select adoption;
            return query;
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            Adoption adoptionToUpdate = adoption;
            // Adoption Approved (bool = true)
            if (isAdopted == true)
            {
                adoptionToUpdate.ApprovalStatus = "Approved";
                adoptionToUpdate.PaymentCollected = true;
                Animal animalToUpdate = db.Animals.Where(a => a.AnimalId == adoptionToUpdate.AnimalId).FirstOrDefault();
                animalToUpdate.AdoptionStatus = "adopted";
                db.SubmitChanges();
            }
            else if(isAdopted == false)
            {
                adoptionToUpdate.ApprovalStatus = "Denied";
                adoptionToUpdate.PaymentCollected = false;
                Animal animalToUpdate = db.Animals.Where(a => a.AnimalId == adoptionToUpdate.AnimalId).FirstOrDefault();
                animalToUpdate.AdoptionStatus = "available";
                RemoveAdoption(adoption.AnimalId, adoption.ClientId);
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoptionToDelete = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoptionToDelete);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var query = from animalShot in db.AnimalShots
                        where animalShot.AnimalId == animal.AnimalId
                        select animalShot;
            return query;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot animalShotToAdd = new AnimalShot();
            animalShotToAdd.AnimalId = animal.AnimalId;
            animalShotToAdd.ShotId = db.Shots.Where(s => s.Name == shotName).FirstOrDefault().ShotId;
            animalShotToAdd.DateReceived = DateTime.Now;
            db.AnimalShots.InsertOnSubmit(animalShotToAdd);
            db.SubmitChanges();
        }
    }
}
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Linq;
using System.Collections.Generic;
using System.Linq;
using zad_1.ViewModel;
using zad_1.Model;

namespace zad_1.DAL
{
    public class ContactRepository : IContactRepository
    {
        private IEmbeddedConfiguration _config;
        private IObjectContainer _database;

        public List<Telephone> Telephones { get; set; }
        public List<Address> Addresses { get; set; }
        public Address ContactAddress { get; set; }
        public List<Contact> Contacts { get; set; }
        public Contact Contact { get; set; }
        public string DatabaseName { get; set; }

        public void SetConfigUpdate()
        {
            _config = Db4oEmbedded.NewConfiguration();
            //_config.UpdateDepth(int.MaxValue);
            _config.Common.UpdateDepth = int.MaxValue;
            //_config.Common.ObjectClass(typeof(Contact)).CascadeOnUpdate(true);
            if (MainWindowViewModel.DatabaseOpen)
            {
                CloseDatabase();
                OpenDatabase();
            }
        }

        public void SetConfigureDelete()
        {
            _config = Db4oEmbedded.NewConfiguration();
            _config.Common.UpdateDepth = int.MaxValue;
            //_config.Common.ObjectClass(typeof(Contact)).CascadeOnDelete(true);
            if (MainWindowViewModel.DatabaseOpen)
            {
                CloseDatabase();
                OpenDatabase();
            }
        }
        public void OpenDatabase()
        {
            MainWindowViewModel.DatabaseOpen = true;
            if (_config != null)
                _database = Db4oEmbedded.OpenFile(_config, DatabaseName);
            else
                _database = Db4oEmbedded.OpenFile(DatabaseName);
        }

        public void CloseDatabase()
        {
            MainWindowViewModel.DatabaseOpen = false;
            _database.Close();
        }
        public void GetAllObjectsInDatabase()
        {
            GetAllAddresses();
            GetAllContacts();
            GetAllTelephones();
        }
        public void GetAllContacts()
        {
            Contacts = (from Contact contact in _database
                        select contact).ToList();
        }
        public void GetAllAddresses()
        {
            Addresses = (from Address a in _database
                         select a).ToList();
        }
        public void GetAllTelephones()
        {
            Telephones = (from Telephone t in _database
                          select t).ToList();
        }

        public void GetContact(Contact contact)
        {
            Contact = new Contact();
            Contact = (from Contact c in _database
                       where c.FirstName == contact.FirstName && c.LastName == contact.LastName
                       select c).FirstOrDefault();
        }
        public void GetTelephones(Contact contact)
        {
            GetContact(contact);
            if (Contact.Telephones != null)
                Telephones = Contact.Telephones.ToList();
            
        }

        public void GetAddress(Contact selectedContact)
        {
            GetContact(selectedContact);
            ContactAddress = Contact.Address;
        }

        public void AddContact(Contact newContact)
        {
            SetConfigUpdate();
            GetContact(newContact);

            if (Contact == null)
            {
                Contact = new Contact();
                Contact.FirstName = newContact.FirstName;
                Contact.LastName = newContact.LastName;

                if (newContact.Telephones != null)
                {
                    Contact.Telephones = new List<Telephone>();
                    Contact.Telephones.Add(newContact.Telephones.First());
                }
                    

                if (newContact.Address != null)
                {
                    Contact.Address = new Address();
                    Contact.Address = newContact.Address;
                }
                    

                _database.Store(Contact);
                _database.Commit();

                GetAllObjectsInDatabase();
            }
        }
        public void DeleteContact(Contact selectedContact)
        {
            SetConfigureDelete();
            GetContact(selectedContact);

            if (Contact != null)
            {
                GetTelephones(selectedContact);
                var telephonesList = new List<Telephone>(Telephones);
                foreach (var item in telephonesList)
                    _database.Delete(item);

                Contact.Telephones.Clear();

                if (Contact.Address !=null)
                    _database.Delete(Contact.Address);

                _database.Delete(Contact);
                _database.Commit();

                GetAllObjectsInDatabase();
            }
        }
        public void AddTelephone(Telephone newTelephone, Contact selectedContact)
        {
            SetConfigUpdate();
            GetContact(selectedContact);
            if (Contact.Telephones == null)
                Contact.Telephones = new List<Telephone>();
            Contact.Telephones.Add(newTelephone);
            _database.Store(Contact);
            _database.Commit();

            GetAllObjectsInDatabase();
        }
        public void DeleteTelephone(Telephone selectedTelephone, Contact selectedContact)
        {            
            SetConfigureDelete();
            GetContact(selectedContact);
            var telToRemove = Contact.Telephones.Where(x => x.Number == selectedTelephone.Number).FirstOrDefault();

            _database.Delete(telToRemove);
            Contact.Telephones.Remove(telToRemove);
            _database.Store(Contact);
            _database.Commit();

            GetAllObjectsInDatabase();
        }

        public void EditAddress(Address newAddress, Contact selectedContact)
        {
            SetConfigUpdate();
            GetContact(selectedContact);
            if (Contact.Address == null)
                Contact.Address = new Address();

            Contact.Address = newAddress;
            _database.Store(Contact);
            _database.Commit();

            GetAllObjectsInDatabase();
        }

        public void EditContact(Contact newContact, Contact selectedContact)
        {
            SetConfigUpdate();
            GetContact(selectedContact);

            Contact.FirstName = newContact.FirstName;
            Contact.LastName = newContact.LastName;
            _database.Store(Contact);
            _database.Commit();

            GetAllObjectsInDatabase();

        }

        public void EditTelephone(Telephone newTelephone, Telephone oldTelephone, Contact selectedContact)
        {
            SetConfigUpdate();
            GetContact(selectedContact);

            var selectedTelephone = Contact.Telephones.FirstOrDefault(x => x.Number == oldTelephone.Number);

            if (selectedTelephone != null)
            {
                _database.Delete(selectedTelephone);
                Contact.Telephones.Remove(selectedTelephone);
                Contact.Telephones.RemoveAll(x => x == null);
                Contact.Telephones.Add(newTelephone);
                _database.Store(Contact);
                _database.Commit();
            }

            GetAllObjectsInDatabase();  
        }
    }
}

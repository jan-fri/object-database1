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
        public Address ContactAddress { get; set; }
        public List<Contact> Contacts { get; set; }
        public Contact Contact { get; set; }
        public string DatabaseName { get; set; }

        public void SetConfigUpdate()
        {
            _config = Db4oEmbedded.NewConfiguration();
            _config.Common.ObjectClass(typeof(Contact)).CascadeOnUpdate(true);
            if (MainWindowViewModel.DatabaseOpen)
            {
                CloseDatabase();
                OpenDatabase();
            }
        }

        public void SetConfigureDelete()
        {
            _config = Db4oEmbedded.NewConfiguration();
            _config.Common.ObjectClass(typeof(Contact)).CascadeOnDelete(true);
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
        public void GetAllContacts()
        {
            Contacts = (from Contact contact in _database
                        select contact).ToList();
        }
        public List<Telephone> ShowAllData()
        {
            var result = (from Telephone t in _database
                          select t).ToList();

            return result;
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
                Contact.Address = newContact.Address;
                Contact.Telephones.Add(newContact.Telephones.First());

                _database.Store(Contact);
                _database.Commit();
            }
        }
        public void DeleteContact(Contact selectedContact)
        {
            SetConfigureDelete();
            GetContact(selectedContact);

            if (Contact != null)
            {
                GetTelephones(selectedContact);
                var tel = Contact.Telephones.ToList();
                foreach (var item in tel)
                    DeleteTelephone(item, selectedContact);

                _database.Delete(Contact.Address);
                _database.Store(Contact);

                _database.Delete(Contact);
            }
        }
        public void AddTelephone(Telephone newTelephone, Contact selectedContact)
        {
            SetConfigUpdate();

            GetContact(selectedContact);
            Contact.Telephones.Add(newTelephone);
            _database.Store(Contact);
        }
        public void DeleteTelephone(Telephone selectedTelephone, Contact selectedContact)
        {            
            SetConfigureDelete();
            GetContact(selectedContact);
            Contact.Telephones.RemoveAll(x => x == null);
            var tel = Contact.Telephones.Where(x => x.Number == selectedTelephone.Number).FirstOrDefault();
            _database.Delete(tel);
            Contact.Telephones.Remove(tel);
            _database.Store(Contact);
        }

        public void EditAddress(Address newAddress, Contact selectedContact)
        {
            SetConfigUpdate();
            GetContact(selectedContact);

            Contact.Address = newAddress;
            _database.Store(Contact);
        }

        public void EditContact(Contact newContact, Contact selectedContact)
        {
            SetConfigUpdate();
            GetContact(selectedContact);

            Contact.FirstName = newContact.FirstName;
            Contact.LastName = newContact.LastName;
            _database.Store(Contact);

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
            }
        }
    }
}


using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using zad_1.DAL;
using zad_1.Model;
using System.Collections.Generic;

namespace zad_1.ViewModel
{
    [ImplementPropertyChanged]
    public class MainWindowViewModel
    {
        public static bool DatabaseOpen { get; set; }

        private int _selectedContactIndexList;
        private int _selectedTelephoneIndex;
        private ContactRepository _contactRepository;

        public string NewDatabaseName { get; set; }
        public string ExtistingDatabaseName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public int ContactTelephoneNumber { get; set; }
        public string ContactPhoneOperator { get; set; }
        public string ContactPhoneType { get; set; }
        public string ContactAddressStreet { get; set; }
        public string ContactAddressCity { get; set; }
        public string ContactPostCode { get; set; }
        public string ContactAddressStreetLabel { get; set; }
        public string ContactAddressCityLabel { get; set; }
        public string ContactPostCodeLabel { get; set; }
        public string NumberOfContacts { get; set; }
        public string NumberOfTelephones { get; set; }
        public string NumberOfAddresses { get; set; }

        public ICommand CreateDatabaseCommand { get; set; }
        public ICommand LoadDatabaseCommand { get; set; }
        public ICommand DeleteContactCommand { get; set; }
        public ICommand AddNewContactCommand { get; set; }
        public ICommand ClearTextBoxesCommand { get; set; }
        public ICommand EditContactCommand { get; set; }
        public ICommand AddNewPhoneCommand { get; set; }
        public ICommand EditTelephoneCommand { get; set; }
        public ICommand DeleteTelephoneCommand { get; set; }
        public ICommand EditAddressCommand { get; set; }

        public ObservableCollection<Contact> ContactList { get; set; }
        public ObservableCollection<Telephone> ContactTelephpones { get; set; }

        public int SelectedContactIndexList
        {
            get { return _selectedContactIndexList; }
            set
            {
                _selectedContactIndexList = value;
                GetContactDetails();
                RefreshTelephones();
            }
        }
        private void GetContactDetails()
        {
            if (SelectedContactIndexList < 0)
                SelectedContactIndexList = 0;

            if (ContactList.Any())
            {
                if (ContactList[SelectedContactIndexList].FirstName !=null)
                {
                    ContactFirstName = ContactList[SelectedContactIndexList].FirstName;
                    ContactLastName = ContactList[SelectedContactIndexList].LastName;
                }
                else
                {
                    ContactFirstName = string.Empty;
                    ContactLastName = string.Empty;
                }

                if (ContactList[SelectedContactIndexList].Address != null)
                {
                    ContactAddressStreet = ContactList[SelectedContactIndexList].Address.Street;
                    ContactAddressCity = ContactList[SelectedContactIndexList].Address.City;
                    ContactPostCode = ContactList[SelectedContactIndexList].Address.PostCode;
                    ContactAddressStreetLabel = "ul. " + ContactList[SelectedContactIndexList].Address.Street;
                    ContactAddressCityLabel = ContactList[SelectedContactIndexList].Address.City;
                    ContactPostCodeLabel = ContactList[SelectedContactIndexList].Address.PostCode;
                }
                else
                {
                    ContactAddressStreet = string.Empty;
                    ContactAddressCity = string.Empty;
                    ContactPostCode = string.Empty;
                    ContactAddressStreetLabel = string.Empty;
                    ContactAddressCityLabel = string.Empty;
                    ContactPostCodeLabel = string.Empty;
                }
            }
        }
        public int SelectedTelephoneIndex
        {
            get { return _selectedTelephoneIndex; }
            set
            {
                _selectedTelephoneIndex = value;
                GetTelephoneDetails();
            }
        }
        private void GetTelephoneDetails()
        {
            if (SelectedTelephoneIndex < 0)
                SelectedTelephoneIndex = 0;
            if (ContactTelephpones.Any())
            {
                ContactTelephoneNumber = ContactTelephpones[SelectedTelephoneIndex].Number;
                ContactPhoneOperator = ContactTelephpones[SelectedTelephoneIndex].Operator;
                ContactPhoneType = ContactTelephpones[SelectedTelephoneIndex].Type;
            }
            else
            {
                ContactTelephoneNumber = 0;
                ContactPhoneOperator = string.Empty;
                ContactPhoneType = string.Empty;
            }
        }
        private void GetNumberofObjectsInDatabase()
        {
            NumberOfAddresses = _contactRepository.Addresses.Count.ToString();
            NumberOfContacts = _contactRepository.Contacts.Count.ToString();
            NumberOfTelephones = _contactRepository.Telephones.Count.ToString();

        }
        public MainWindowViewModel()
        {
            NewDatabaseName = string.Empty;
            ContactList = new ObservableCollection<Contact>();
            ContactTelephpones = new ObservableCollection<Telephone>();
            CreateDatabaseCommand = new RelayCommand(CreateDatabase, CheckDatabaseName);
            LoadDatabaseCommand = new RelayCommand(LoadDatabase);
            DeleteContactCommand = new RelayCommand(DeleteContact, CheckIfContactExists);
            AddNewContactCommand = new RelayCommand(AddNewContact);
            ClearTextBoxesCommand = new RelayCommand(ClearText);
            EditContactCommand = new RelayCommand(EditContact, CheckIfContactExists);
            AddNewPhoneCommand = new RelayCommand(AddNewPhone, CheckIfContactExists);
            EditTelephoneCommand = new RelayCommand(EditTelephone, CheckIfTelephoneExists);
            DeleteTelephoneCommand = new RelayCommand(DeleteTelephone, CheckIfTelephoneExists);
            EditAddressCommand = new RelayCommand(EditAddress, CheckIfContactExists);
            _contactRepository = new ContactRepository();
           
        }
        private bool CheckIfContactExists(object obj)
        {
            if (!ContactList.Any())
                return false;
            return true;
        }
        private bool CheckIfTelephoneExists(object obj)
        {

            if (ContactTelephpones.Any())
                return true;                
            return false;
        }
        private bool CheckDatabaseName(object obj)
        {
            if (!string.IsNullOrEmpty(NewDatabaseName))
                return true;

            return false;
        }
        private void CreateDatabase(object obj)
        {
            _contactRepository.DatabaseName = NewDatabaseName;
            if (DatabaseOpen)
            {
                _contactRepository.CloseDatabase();
                _contactRepository.OpenDatabase();
            }
            else
                _contactRepository.OpenDatabase();

            ExtistingDatabaseName = NewDatabaseName;
        }
        private void LoadDatabase(object obj)
        {
            if (DatabaseOpen)
                _contactRepository.CloseDatabase();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)| *.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ExtistingDatabaseName = openFileDialog.FileName;
                _contactRepository.DatabaseName = ExtistingDatabaseName;
                _contactRepository.OpenDatabase();
                RefreshContactLists();
                SelectedContactIndexList = 0;
                GetContactDetails();
                if (ContactList.Count > 0)
                {
                    RefreshTelephones();
                    GetTelephoneDetails();
                }
                _contactRepository.GetAllObjectsInDatabase();
                GetNumberofObjectsInDatabase();
            }
        }
        private void RefreshContactLists()
        {
            _contactRepository.GetAllContacts();
            if (_contactRepository.Contacts.Any())
            {
                ContactList = new ObservableCollection<Contact>(_contactRepository.Contacts);
                GetContactDetails();
            }
        }
        private void RefreshTelephones()
        {
            if (ContactList.Any())
            {
                _contactRepository.GetTelephones(ContactList[SelectedContactIndexList]);
                if (_contactRepository.Telephones != null)
                    ContactTelephpones = new ObservableCollection<Telephone>(_contactRepository.Telephones);
                else
                    ContactTelephpones = new ObservableCollection<Telephone>(); 
            }
        }
        private void AddNewContact(object obj)
        {
            Contact contact = new Contact();
            if (ContactTelephoneNumber != 0)
            {
                Telephone newTelephone = new Telephone
                {
                    Number = ContactTelephoneNumber,
                    Operator = ContactPhoneOperator,
                    Type = ContactPhoneType
                };
                contact.Telephones = new List<Telephone>();
                contact.Telephones.Add(newTelephone);

                ContactTelephpones.Add(newTelephone);
            }

            if (!string.IsNullOrEmpty(ContactAddressStreet))
            {
                Address address = new Address
                {
                    City = ContactAddressCity,
                    PostCode = ContactPostCode,
                    Street = ContactAddressStreet
                };
                contact.Address = new Address();
                contact.Address = address;
            }

            contact.FirstName = ContactFirstName;
            contact.LastName = ContactLastName;

            _contactRepository.AddContact(contact);
            ContactList.Add(contact);
            
            GetContactDetails();
            GetTelephoneDetails();
            GetNumberofObjectsInDatabase();
        }
        private void AddNewPhone(object obj)
        {
            Telephone newTelephone = new Telephone
            {
                Number = ContactTelephoneNumber,
                Operator = ContactPhoneOperator,
                Type = ContactPhoneType
            };

            _contactRepository.AddTelephone(newTelephone, ContactList[SelectedContactIndexList]);
            ContactTelephpones.Add(newTelephone);
            GetNumberofObjectsInDatabase();
        }
        private void DeleteTelephone(object obj)
        {
            _contactRepository.DeleteTelephone(ContactTelephpones[SelectedTelephoneIndex], ContactList[SelectedContactIndexList]);
            ContactTelephpones.Remove(ContactTelephpones[SelectedTelephoneIndex]);
            GetNumberofObjectsInDatabase();
        }
        private void EditAddress(object obj)
        {
            Address newAddress = new Address
            {
                City = ContactAddressCity,
                PostCode = ContactPostCode,
                Street = ContactAddressStreet
            };

            _contactRepository.EditAddress(newAddress, ContactList[SelectedContactIndexList]);
            ContactList[SelectedContactIndexList].Address = newAddress;
            GetContactDetails();
            GetNumberofObjectsInDatabase();
        }        
        private void EditTelephone(object obj)
        {
            Telephone newTelephone = new Telephone
            {
                Number = ContactTelephoneNumber,
                Operator = ContactPhoneOperator,
                Type = ContactPhoneType
            };
            _contactRepository.EditTelephone(newTelephone, ContactTelephpones[SelectedTelephoneIndex], ContactList[SelectedContactIndexList]);
            ContactTelephpones[SelectedTelephoneIndex] = newTelephone;
            GetTelephoneDetails();
            GetNumberofObjectsInDatabase();
        }
        private void EditContact(object obj)
        {
            Contact newContact = new Contact();
            newContact.FirstName = ContactFirstName;
            newContact.LastName = ContactLastName;

            _contactRepository.EditContact(newContact, ContactList[SelectedContactIndexList]);

            EditAddress(true);
            EditTelephone(true);
        }
        private void ClearText(object obj)
        {            
            ContactFirstName = string.Empty;
            ContactLastName = string.Empty;
            ContactPostCode = string.Empty;
            ContactAddressCity = string.Empty;
            ContactAddressStreet = string.Empty;
            ContactTelephoneNumber = 0;
            ContactPhoneOperator = string.Empty;
            ContactPhoneType = string.Empty;
        }
        private void DeleteContact(object obj)
        {
            _contactRepository.DeleteContact(ContactList[SelectedContactIndexList]);
            ContactList.Remove(ContactList[SelectedContactIndexList]);
            RefreshTelephones();
            GetNumberofObjectsInDatabase();
        }
    }
}

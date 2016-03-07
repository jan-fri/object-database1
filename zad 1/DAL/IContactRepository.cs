using System.Collections.Generic;
using zad_1.Model;

namespace zad_1.DAL
{
    interface IContactRepository
    {
        void GetContact(Contact newContact);
        void AddContact(Contact newContact);
        void DeleteContact(Contact selectedContact);
        void EditContact(Contact newContact, Contact selectedContact);
        void AddTelephone(Telephone newTelephone, Contact selectedContact);
        void DeleteTelephone(Telephone selectedTelephone, Contact selectedContact);
        void EditTelephone(Telephone newTelephone, Telephone oldTelephone, Contact selectedContact);
        void EditAddress(Address newAddress, Contact selectedContact);


    }
}

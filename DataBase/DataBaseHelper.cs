using DataBase.Models;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Dtos;

namespace DataBase
{
    public class DataBaseHelper : DataBaseHandler
    {
        public DataBaseHelper(string connectionString = "") : base(connectionString)
        {
        }

        public object GetMenuItems()
        {
            var query = Session.Query<MenuItem>().ToFuture();
            if (!query.Any())
                return null;
            return query;
        }

        public List<ContactResponseDto> GetContactList()
        {
            var report = new List<ContactResponseDto>();
            var contacts = Session.Query<Contact>().ToFuture();
            if (!contacts.Any())
                return null;
            
            foreach (var contact in contacts)
            {
                var thisContact = new ContactFlatDto
                {
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Email = contact.Email,
                    Phone = contact.Phone
                };
                var existingRole = report.Where(c => c.SwedishRole == contact.SwedishRole).FirstOrDefault();
                if (existingRole == null) {
                    report.Add(new ContactResponseDto
                    {
                        SwedishRole = contact.SwedishRole,
                        EnglishRole = contact.EnglishRole,
                        Contacts = new List<ContactFlatDto>{thisContact }
                    });
                } else
                {
                    for (int i = 0; i < report.Count; i++)
                    {
                        if (report[i].SwedishRole == contact.SwedishRole)
                        {
                            report[i].Contacts.Add(thisContact);
                        }
                    }
                }
            }
            return report;
        }

        public List<InfoDto> GetInfos()
        {
            var query = Session.Query<Info>().ToFuture();
            if (!query.Any())
                return null;
            var output = new List<InfoDto>();
            foreach (var info in query)
            {
                output.Add(new InfoDto
                {
                    English = info.English,
                    Swedish = info.Swedish,
                    Name = info.Name
                });
            }
            return output;
        }

        public void AddOrUpdateContact(ContactPostDto newContact)
        {
            using (var transaction = Session.BeginTransaction())
            {
                Contact contact;
                var existContact = Session.Query<Contact>().Where(c => c.FirstName.ToLower() == newContact.FirstName.ToLower() && c.LastName.ToLower() == newContact.LastName.ToLower()).ToFuture();
                if (existContact.Any())
                {
                    contact = existContact.First();
                } else
                {
                    contact = new Contact();
                }
                contact.FirstName = newContact.FirstName;
                contact.LastName = newContact.LastName;
                contact.Email = newContact.Email;
                contact.Phone = newContact.Phone;
                contact.SwedishRole = newContact.SwedishRole;
                contact.EnglishRole = newContact.EnglishRole;
                if (existContact.Any())
                    Session.Update(contact);
                else
                    Session.Save(contact);
                transaction.Commit();
            }
        }

        public void CreateOrUpdateInfoItem(InfoDto newInfo)
        {
            using (var transaction = Session.BeginTransaction())
            {
                Info info;
                var existInfo = Session.Query<Info>().Where(i => i.Name.ToLower() == newInfo.Name.ToLower()).ToFuture();
                if (existInfo.Any())
                {
                    info = existInfo.First();
                    info.Name = newInfo.Name;
                    info.Swedish = newInfo.Swedish;
                    info.English = newInfo.English;
                } else
                {
                    info = new Info
                    {
                        Name = newInfo.Name,
                        Swedish = newInfo.Swedish,
                        English = newInfo.English
                    };
                }
                Session.SaveOrUpdate(info);
                transaction.Commit();
            }
        }

        public Guid CreateOrUpdateMenuItem(MenuItem menu)
        {
            using (var transaction = Session.BeginTransaction())
            {
                var exists = Session.Query<MenuItem>().Where(m => m.Swedish.ToLower() == menu.Swedish.ToLower() || m.English.ToLower() == menu.English.ToLower() || m.Navigation == menu.Navigation).ToList();
                if (exists.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(menu.Swedish) && !string.IsNullOrWhiteSpace(menu.English) && !string.IsNullOrWhiteSpace(menu.Navigation))
                    {
                        Session.Save(menu);
                        transaction.Commit();
                        return menu.Id;
                    }
                    return Guid.Empty;
                }
                else if (exists.Count == 1)
                {
                    var dbMenu = exists[0];
                    var update = new MenuItem { Swedish = dbMenu.Swedish, English = dbMenu.English, Navigation = dbMenu.Navigation, Id = dbMenu.Id };
                    Session.Update(update);
                    transaction.Commit();
                    return update.Id;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
    }
}

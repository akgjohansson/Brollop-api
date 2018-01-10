using DataBase;
using DataBase.Dtos;
using DataBase.Models;
using MailManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API_brollop.Controllers
{
    [RoutePrefix("person"), EnableCors(origins: "http://localhost:9000", headers: "*", methods: "*")]
    public class PersonController : ApiController
    {
        [Route("{referenceCode}"),HttpGet]
        public IHttpActionResult Get(string referenceCode)
        {
            using (var helper = new DataBaseHelper())
            {
                var company = helper.GetCompanyIdByReferenceCode(referenceCode);
                if (company == null)
                    return BadRequest("Anmälan hittades inte");
                var returnPersons = new List<Person>();
                company.Persons.ToList().ForEach(p => {
                    returnPersons.Add(new Person
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Phone = p.Phone,
                        Email = p.Email,
                        Going = p.Going,
                        FoodPreferences = p.FoodPreferences.Select(f => new FoodPreference { EnglishName = f.EnglishName, SwedishName = f.SwedishName }).ToList()
                    });
                });
                var returnCompany = new Company
                {
                    Id = company.Id,
                    Comment = company.Comment,
                    Persons = returnPersons
                };
                return Ok(returnCompany);
            }
        }

        [Route("registration"),HttpPost]
        public IHttpActionResult Post(CompanyPostDto persons)
        {
            using (var helper = new DataBaseHelper())
            {
                var accessCode = "";
                persons.Persons.ForEach(x => accessCode += $"{x.FirstName.Substring(0, 2)}{x.LastName.Substring(0, 2)}");
                var id = helper.RegisterCompany(persons, accessCode);
                var emails = new List<string>();
                persons.Persons.ForEach(x => emails.Add(x.Email));
                using (var mailManager = new MailManager.MailManager("smtp.gmail.com", 587, "ja19maj@gmail.com","ja19maj@gmail.com","JA2018-pass"))
                {
                    var text = $"Hej {FormatGuestList(persons.Persons)}!\n\nVad roligt det ska bli att se {GetAckusativePronoun(persons.Persons.Count)} "+
                        $"på vårt bröllop! Håll gärna koll på hemsidan framöver, för där kommer all nödvändig information att vara med. Tveka inte "+
                        $"att höra av {GetAckusativePronoun(persons.Persons.Count)} om det är något {GetPronoun(persons.Persons.Count)} undrar över!\n\nOm "+
                        $"{GetPronoun(persons.Persons.Count)} vill redigera {GetPossessivePronoun(persons.Persons.Count)} anmälan, kan "+
                        $"{GetPronoun(persons.Persons.Count)} göra det genom att gå in på anmälningssidan hos hemsidan och där trycka "+
                        $"\"redigera anmälning\". Där fyller {GetPronoun(persons.Persons.Count)} i koden {accessCode} och trycker enter.\n\n"+
                        "Vi ses på bröllopet!\n\nJohanna och Andreas";
                    mailManager.SendMail(emails, "Välkommen på bröllop!", text);
                }

                    return Ok(accessCode);
            }
        }


        [Route("registration/{id:Guid}"),HttpPut]
        public IHttpActionResult Put(Guid id, CompanyPostDto company)
        {
            using (var helper = new DataBaseHelper())
            {
                bool success = helper.UpdateCompany(id, company);
                if (success)
                {
                    using (var mailManager = new MailManager.MailManager("smtp.gmail.com", 587, "ja19maj@gmail.com", "ja19maj@gmail.com", "JA2018-pass"))
                    {
                        var text = $"Hej {FormatGuestList(company.Persons)}!\n\n{GetPossessivePronoun(company.Persons.Count, true)} anmälan är uppdaterad. Vi ses på bröllopet!\n\nVarma hälsningar,\n" +
                            $"Johanna och Andreas";
                        mailManager.SendMail(company.Persons.Select(c => c.Email), "Bröllopsanmälan är uppdaterad", text);
                    }
                        return Ok();
                }
                return BadRequest();
            }
        }

        private string FormatGuestList(List<PersonDto> persons)
        {
            string output = "";
            for (int i = 0; i < persons.Count; i++)
            {
                output += persons[i].FirstName;
                if (i == persons.Count - 2)
                    output += " & ";
                else if (i < persons.Count - 2)
                    output += ", ";
            }
            return output;
        }

        private string GetAckusativePronoun(int count)
        {
            return count == 1 ? "dig": "er";
                
        }

        private string GetPossessivePronoun(int count, bool capitalLetter = false)
        {
            if (count == 1)
                return capitalLetter?"Din":"din";
            return capitalLetter?"Er":"er";
        }

        private string GetPronoun(int count)
        {
            if (count == 1)
                return "du";
            return "ni";
        }
    }
}
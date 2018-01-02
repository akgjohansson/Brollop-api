using DataBase.Models;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Mapping
{
    class Mapping
    {
        private readonly ModelMapper _modelMapper;

        public Mapping()
        {
            _modelMapper = new ModelMapper();
        }

        public HbmMapping Map()
        {
            MapPerson();
            MapInfo();
            MapFoodPreference();
            MapCompany();
            MapContact();
            MapMenuItem();

            return _modelMapper.CompileMappingForAllExplicitlyAddedEntities();
        }

        private void MapMenuItem()
        {
            _modelMapper.Class<MenuItem>(e =>
            {
                e.Id(x => x.Id, p => p.Generator(Generators.GuidComb));
                e.Property(p => p.Swedish, p => { p.NotNullable(true); p.Length(100000); });
                e.Property(p => p.English, p => { p.NotNullable(true); p.Length(100000); });
                e.Property(p => p.Navigation, p => p.NotNullable(true));
            });
        }

        private void MapContact()
        {
            _modelMapper.Class<Contact>(e =>
            {
                e.Id(x => x.Id, p => p.Generator(Generators.GuidComb));
                e.Property(x => x.Email, p => p.NotNullable(true));
                e.Property(p => p.FirstName, p => p.NotNullable(true));
                e.Property(p => p.LastName, p => p.NotNullable(true));
                e.Property(p => p.Phone, p => p.NotNullable(true));
                e.Property(p => p.SwedishRole, p => p.NotNullable(true));
                e.Property(p => p.EnglishRole, p => p.NotNullable(true));
            });
        }

       private void MapCompany()
        {
            _modelMapper.Class<Company>(e =>
            {
                e.Id(x => x.Id, p => p.Generator(Generators.GuidComb));
                e.Set(p => p.Persons, p =>
                {
                    p.Inverse(true);
                    p.Cascade(Cascade.All);
                    p.Key(k => k.Column(col => col.Name("CompanyId")));
                }, p => p.OneToMany());
            });
        }

        private void MapFoodPreference()
        {
            _modelMapper.Class<FoodPreference>(e =>
            {
                e.Id(x => x.Id, p => p.Generator(Generators.GuidComb));
                e.Property(p => p.SwedishName, p => p.NotNullable(true));
                e.Property(p => p.EnglishName, p => p.NotNullable(true));
                e.Set(x => x.Persons, mapper =>
                {
                    mapper.Inverse(true);
                    mapper.Table("PersonFoodPreference");
                    mapper.Cascade(Cascade.None);
                    mapper.Key(k => k.Column("FoodPreferenceId"));
                }, map => map.ManyToMany(p =>
                {
                    p.Column("PersonId");
                    p.ForeignKey("FK_PersonFoodPreference_Person");
                    p.Class(typeof(Person));
                }));
            });
        }

        
        private void MapInfo()
        {
           _modelMapper.Class<Info>(e =>
           {
               e.Id(x => x.Id, p => p.Generator(Generators.GuidComb));
               e.Property(p => p.Name);
               e.Property(p => p.Swedish, p => p.Length(100000));
               e.Property(p => p.English, p => p.Length(100000));
           });
        }

        private void MapPerson()
        {
            _modelMapper.Class<Person>(e =>
            {
                e.Id(x => x.Id, p => p.Generator(Generators.GuidComb));
                e.Property(p => p.FirstName, p => p.NotNullable(true));
                e.Property(p => p.LastName, p => p.NotNullable(true));
                e.Property(p => p.Phone, p => p.NotNullable(true));
                e.Property(p => p.Email, p => p.NotNullable(true));
                e.Property(p => p.Going);
                e.ManyToOne(p => p.Company, mapper =>
                {
                    mapper.Column("CompanyId");
                    mapper.NotNullable(true);
                    mapper.Cascade(Cascade.None);
                });
                e.Set(p => p.FoodPreferences, mapper =>
                {
                    mapper.Inverse(false);
                    mapper.Table("PersonFoodPreference");
                    mapper.Cascade(Cascade.None);
                    mapper.Key(k => k.Column("PersonId"));
                }, map => map.ManyToMany(p =>
                {
                    p.Column("FoodPreferenceId");
                    p.ForeignKey("FK_PersonFoodPreference_FoodPreference");
                    p.Class(typeof(FoodPreference));
                }));
            });

        }
    }
}

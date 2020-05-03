using System;
using CsvHelper.Configuration.Attributes;

namespace MCCR.Data
{
    public class Customer
    {
        [Name("ID")] public int Id { get; set; }
        public string Title { get; set; }
        [Name("FirstName")] public string GivenNames { get; set; }
        [Name("Surname")] public string Surname { get; set; }
        public Gender Gender { get; set; }
        [Name("DOB")] public DateTime BirthDate { get; set; }
    }
}
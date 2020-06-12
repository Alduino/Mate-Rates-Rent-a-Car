using System;
using MRRC.Cursive;

namespace MCCR.Data
{
    public class Customer
    {
        [Name("ID")] public int Id { get; set; }
        public string Title { get; set; }
        [Name("FirstName")] public string GivenNames { get; set; }
        [Name("LastName")] public string Surname { get; set; }
        public Gender Gender { get; set; }
        [Name("DOB")] public DateTime BirthDate { get; set; }

        public override string ToString() => $"{Id}: {GivenNames} {Surname}";
    }
}
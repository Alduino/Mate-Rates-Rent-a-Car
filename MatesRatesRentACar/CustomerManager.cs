using System;
using System.Globalization;
using System.Text.RegularExpressions;
using MCCR.Data;
using MRRC.Guacamole.Components.Forms;
using MRRC.Guacamole.MenuGeneration;

namespace MateRatesRentACar
{
    public class CustomerManager
    {
        private static readonly Regex DobRegex = new Regex("^[0-3]\\d/[0-1]\\d/\\d{4}$");

        private readonly CustomerResourceManager _crm;
        
        [MenuItem]
        public Form AddCustomer { get; } = new Form("Add Customer", new []
        {
            new Form.Item("Title", new TextBox()),
            new Form.Item("Given Names", new TextBox()),
            new Form.Item("Surname", new TextBox()),
            new Form.Item("DOB (dd/mm/yyyy)", new TextBox()),
            new Form.Item("Gender", new Select<Gender>())
        }, new Button("Submit"));

        public CustomerManager(CustomerResourceManager crm)
        {
            _crm = crm;
            AddCustomer.Submitted += AddCustomerOnSubmitted;
        }

        private void AddCustomerOnSubmitted(object sender, Form.SubmittedEventArgs e)
        {
            if (!e.Data.TryGet("Title", out string title) || title.IsEmpty())
            {
                e.Result = "Title is required";
                return;
            }

            if (!e.Data.TryGet("Given Names", out string givenNames) || givenNames.IsEmpty())
            {
                e.Result = "Given names are required";
                return;
            }

            if (!e.Data.TryGet("Surname", out string surname) || surname.IsEmpty())
            {
                e.Result = "Surname is required";
                return;
            }

            if (!e.Data.TryGet("DOB (dd/mm/yyyy)", out string dobStr) || !DobRegex.IsMatch(dobStr))
            {
                e.Result = "Invalid date of birth";
                return;
            }

            if (!e.Data.TryGet("Gender", out Gender gender))
            {
                e.Result = "Gender is required";
                return;
            }

            var customer = new Customer
            {
                Title = title,
                GivenNames = givenNames,
                Surname = surname,
                Gender = gender,
                BirthDate = DateTime.ParseExact(dobStr, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Id = _crm.NextId()
            };

            if (!_crm.AddCustomer(customer)) e.Result = "Couldn't add customer";
        }
    }
}
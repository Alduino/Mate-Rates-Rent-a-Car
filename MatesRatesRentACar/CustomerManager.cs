using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MCCR.Data;
using MRRC.Guacamole;
using MRRC.Guacamole.Components;
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
        
        [MenuItem]
        public OneOf ModifyCustomer { get; } = new OneOf(new Dictionary<string, Component>
        {
            {
                "search",
                new Form("Find Customer", new []
                {
                    new Form.Item("Customer ID", new TextBox())
                }, new Button("Search"))
            },
            {
                "modify",
                new Form("Update Customer", new []
                {
                    new Form.Item("ID", new TextBox { ReadOnly = true }),
                    new Form.Item("Title", new TextBox()),
                    new Form.Item("Given Names", new TextBox()),
                    new Form.Item("Surname", new TextBox()),
                    new Form.Item("DOB (dd/mm/yyyy)", new TextBox()),
                    new Form.Item("Gender", new Select<Gender>())
                }, new Button("Submit"))
            }
        }, "search", "Modify Customer");

        public CustomerManager(CustomerResourceManager crm)
        {
            _crm = crm;
            AddCustomer.Submitted += AddCustomerOnSubmitted;
            ModifyCustomer.GetComponent<Form>("search").Submitted += ModifyCustomerOnSearch;
            ModifyCustomer.GetComponent<Form>("modify").Submitted += ModifyCustomerOnSubmitted;
        }

        private void ModifyCustomerOnSubmitted(object sender, Form.SubmittedEventArgs e)
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

            var customer = _crm.Customers
                .First(c => c.Id == int.Parse(e.Data.Get<string>("ID")));

            customer.Title = title;
            customer.GivenNames = givenNames;
            customer.Surname = surname;
            customer.BirthDate = DateTime.ParseExact(dobStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            customer.Gender = gender;

            e.Result = "Successfully updated customer";
        }

        private void ModifyCustomerOnSearch(object sender, Form.SubmittedEventArgs e)
        {
            if (!int.TryParse(e.Data.Get<string>("Customer ID"), out var customerId))
            {
                e.Result = "Invalid ID";
                return;
            }

            var customer = _crm.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                e.Result = "No customer with that ID";
                return;
            }
            
            var modifyForm = ModifyCustomer.GetComponent<Form>("modify");
            modifyForm.Set("ID", customerId.ToString());
            modifyForm.Set("Title", customer.Title);
            modifyForm.Set("Given Names", customer.GivenNames);
            modifyForm.Set("Surname", customer.Surname);
            modifyForm.Set("DOB (dd/mm/yyyy)", customer.BirthDate.ToString("dd/MM/yyyy"));
            modifyForm.Set("Gender", customer.Gender);

            ModifyCustomer.ActiveComponent = "modify";
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
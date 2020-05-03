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
        private readonly Fleet _fleet;

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

        [MenuItem] public OneOf DeleteCustomer { get; } = new OneOf(new Dictionary<string, Component>
        {
            {
                "select",
                new Form("Select Customer", new []
                {
                    new Form.Item("Customer ID", new TextBox())
                }, new Button("Search"))
            },
            {
                "confirm",
                new Form("Confirm Deletion", new []
                {
                    new Form.Item("ID", new TextBox { ReadOnly = true }),
                    new Form.Item("Given Names", new TextBox { ReadOnly = true }),
                    new Form.Item("Surname", new TextBox { ReadOnly = true }),
                    new Form.Item("Date of birth", new TextBox { ReadOnly = true }),
                }, new Button("Confirm"))
            }
        }, "select", "Delete Customer");

        public CustomerManager(CustomerResourceManager crm, Fleet fleet)
        {
            _crm = crm;
            _fleet = fleet;
            AddCustomer.Submitted += AddCustomerOnSubmitted;
            ModifyCustomer.GetComponent<Form>("search").Submitted += ModifyCustomerOnSearch;
            ModifyCustomer.GetComponent<Form>("modify").Submitted += ModifyCustomerOnSubmitted;
            DeleteCustomer.GetComponent<Form>("select").Submitted += DeleteCustomerOnSearch;
            DeleteCustomer.GetComponent<Form>("confirm").Submitted += DeleteCustomerOnSubmitted;
        }

        private void DeleteCustomerOnSubmitted(object sender, Form.SubmittedEventArgs e)
        {
            if (!_crm.RemoveCustomer(int.Parse(e.Data.Get<string>("ID"))))
            {
                e.Result = "Could not delete";
            }
        }

        private void DeleteCustomerOnSearch(object sender, Form.SubmittedEventArgs e)
        {
            if (!int.TryParse(e.Data.Get<string>("Customer ID"), out var customerId))
            {
                e.Result = "Invalid ID";
                return;
            }

            if (_crm.Customers.All(c => c.Id != customerId))
            {
                e.Result = "No customer with that ID";
                return;
            }

            if (_fleet.IsRenting(customerId))
            {
                e.Result = "Customer still renting";
                return;
            }

            var customer = _crm.Customers.First(c => c.Id == customerId);

            var confirmForm = DeleteCustomer.GetComponent<Form>("confirm");
            confirmForm.Set("ID", customerId.ToString());
            confirmForm.Set("Given Names", customer.GivenNames);
            confirmForm.Set("Surname", customer.Surname);
            confirmForm.Set("Date of birth", customer.BirthDate.ToString("dd/MM/yyyy"));

            DeleteCustomer.ActiveComponent = "confirm";
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

            e.Result = _crm.AddCustomer(customer) ? $"Added customer {customer.Id}" : "Couldn't add customer";
        }
    }
}
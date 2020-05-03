using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace MCCR.Data
{
    public class CustomerResourceManager
    {
        private List<Customer> _customers;
        
        private readonly string _source;

        public IEnumerable<Customer> Customers => _customers;
        
        public CustomerResourceManager(string source)
        {
            _source = source;
            
            LoadVehicles();
        }

        /// <summary>
        /// Writes customers to the source file
        /// </summary>
        public void Save()
        {
            using (var writer = new StreamWriter(_source))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(Customers);
            }
        }

        /// <summary>
        /// Overwrites the current list of customers with the ones saved in the source file
        /// </summary>
        public void LoadVehicles()
        {
            using (var reader = new StreamReader(_source))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                _customers = csv.GetRecords<Customer>().ToList();
            }
        }

        /// <summary>
        /// Adds a customer if they don't already exist
        /// </summary>
        /// <returns>True if the customer was added, false otherwise</returns>
        public bool AddCustomer(Customer customer)
        {
            if (Customers.Any(c => c.Id == customer.Id)) return false;
            _customers.Add(customer);
            return true;
        }

        /// <summary>
        /// Removes the specified customer if they exist
        /// </summary>
        /// <returns>True if the customer was removed, false if they didn't exist</returns>
        public bool RemoveCustomer(int id)
        {
            var customer = Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return false;
            _customers.Remove(customer);
            return true;
        }

        /// <summary>
        /// Returns the next available ID
        /// </summary>
        public int NextId() => Customers.Max(c => c.Id) + 1;
    }
}
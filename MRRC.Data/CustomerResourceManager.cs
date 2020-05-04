using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MRRC.Cursive;

namespace MCCR.Data
{
    /// <summary>
    /// Manages customers
    /// </summary>
    public class CustomerResourceManager
    {
        private List<Customer> _customers;
        
        private readonly string _source;

        /// <summary>
        /// List of customers
        /// </summary>
        public IEnumerable<Customer> Customers => _customers;
        
        public CustomerResourceManager(string source)
        {
            _source = source;
            
            Load();
        }

        /// <summary>
        /// Writes customers to the source file
        /// </summary>
        public void Save()
        {
            using (var writer = new StreamWriter(_source))
            {
                var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);
                csv.WriteRecords(Customers);
            }
        }

        /// <summary>
        /// Overwrites the current list of customers with the ones saved in the source file
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_source))
            {
                _customers = new List<Customer>();
                return;
            }
            
            using (var reader = new StreamReader(_source))
            {
                var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace MCCR.Data
{
    /// <summary>
    /// Stores information about the fleet of vehicles owned by MRRC, and what customers are renting them
    /// </summary>
    public class Fleet
    {
        /// <summary>
        /// Used for reading and writing rentals to/from the source CSV
        /// </summary>
        private class Rental
        {
            [Name("CustomerID")]
            public int CustomerId { get; }
            public string Registration { get; }
            
            public Rental(string registration, int customerId)
            {
                Registration = registration;
                CustomerId = customerId;
            }
        }
        
        private List<Vehicle> _vehicles;
        private readonly Dictionary<int, string> _rentals = new Dictionary<int, string>();

        private readonly string _fleetDb;
        private readonly string _rentalDb;

        /// <summary>
        /// List of every vehicle owned by MCCR
        /// </summary>
        public IEnumerable<Vehicle> Vehicles => _vehicles;

        public Fleet(string fleetDb, string rentalDb)
        {
            _fleetDb = fleetDb;
            _rentalDb = rentalDb;

            LoadVehicles();
            LoadRentals();
        }

        /// <summary>
        /// Writes the vehicles to the <see cref="_fleetDb"/> file specified in the constructor
        /// </summary>
        public void SaveVehicles()
        {
            using (var writer = new StreamWriter(_fleetDb))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(Vehicles);
            }
        }

        /// <summary>
        /// Writes the current renting state to the <see cref="_rentalDb"/> file specified in the constructor
        /// </summary>
        public void SaveRentals()
        {
            using (var writer = new StreamWriter(_rentalDb))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(_rentals.Select(kvp => new Rental(kvp.Value, kvp.Key)));
            }
        }

        /// <summary>
        /// Overwrites the current list of vehicles with the ones specified by <see cref="_fleetDb"/>
        /// </summary>
        public void LoadVehicles()
        {
            using (var reader = new StreamReader(_fleetDb))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                _vehicles = csv.GetRecords<Vehicle>().ToList();
            }
        }

        /// <summary>
        /// Overwrites the current list of rentals with the ones specified by <see cref="_rentalDb"/>
        /// </summary>
        public void LoadRentals()
        {
            using (var reader = new StreamReader(_rentalDb))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var rentals = csv.GetRecords<Rental>();

                _rentals.Clear();
                foreach (var rental in rentals)
                {
                    _rentals.Add(rental.CustomerId, rental.Registration);
                }
            }
        }

        /// <summary>
        /// Adds a new vehicle to the fleet if we don't already have a vehicle with the same registration.
        /// </summary>
        /// <param name="vehicle">New vehicle to be added</param>
        /// <returns>True when the vehicle was added, false if it already existed</returns>
        public bool AddVehicle(Vehicle vehicle)
        {
            if (Vehicles.Any(v => v.Registration == vehicle.Registration)) return false;
            _vehicles.Add(vehicle);
            return true;
        }

        /// <summary>
        /// Removes a vehicle from the fleet
        /// </summary>
        /// <param name="registration">The registration code of the vehicle</param>
        /// <returns>True if the vehicle was removed, false otherwise</returns>
        public bool RemoveVehicle(string registration)
        {
            var vehicle = Vehicles.FirstOrDefault(v => v.Registration == registration);
            if (vehicle == null) return false;
            
            _vehicles.Remove(vehicle);
            return true;
        }

        /// <summary>
        /// Returns true if the specified vehicle is currently being rented out
        /// </summary>
        public bool IsRented(string registration) => _rentals.ContainsValue(registration);

        /// <summary>
        /// Returns true if the specified customer is currently renting a vehicle
        /// </summary>
        public bool IsRenting(int customerId) => _rentals.ContainsKey(customerId);

        /// <summary>
        /// Returns the ID of the customer renting the specified vehicle, or -1 if it is not being rented.
        /// </summary>
        public int RentedBy(string registration) => _rentals.ContainsValue(registration) ?
            _rentals.First(v => v.Value == registration).Key : -1;

        /// <summary>
        /// Rents out a vehicle to the specified customer
        /// </summary>
        /// <returns>True if the renting was successful, false if the vehicle is already being rented or the customer is
        /// already renting a vehicle</returns>
        public bool RentVehicle(string registration, int customerId)
        {
            if (IsRenting(customerId) || IsRented(registration)) return false;
            _rentals.Add(customerId, registration);
            return true;
        }

        /// <summary>
        /// Returns a vehicle and returns the ID of the customer who was renting it (or -1 if it wasn't being rented)
        /// </summary>
        public int ReturnVehicle(string registration)
        {
            if (!IsRented(registration)) return -1;
            var rentedBy = RentedBy(registration);
            _rentals.Remove(rentedBy);
            return rentedBy;
        }
    }
}
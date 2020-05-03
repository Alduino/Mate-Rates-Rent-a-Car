using System.Text.RegularExpressions;
using MCCR.Data;
using MRRC.Guacamole.Components.Forms;
using MRRC.Guacamole.MenuGeneration;

namespace MateRatesRentACar
{
    public class FleetManager
    {
        private static readonly Regex RegoRegex = new Regex("^\\d{3}[A-Za-z]{3}");
        
        private readonly Fleet _fleet;

        [MenuItem] public Form AddVehicle { get; } = new Form("Add Vehicle", new []
        {
            new Form.Item("Registration", new TextBox { Placeholder = "123ABC"}), 
            new Form.Item("Grade", new Select<VehicleGrade>()), 
            new Form.Item("Make", new TextBox()), 
            new Form.Item("Model", new TextBox()), 
            new Form.Item("Year", new TextBox { MaxLength = 4}), 
            new Form.Item("Seat count", new TextBox { Placeholder = "Default: 4"}), 
            new Form.Item("Transmission", new Select<TransmissionType>().WithDefault(TransmissionType.Manual)), 
            new Form.Item("Fuel type", new Select<FuelType>().WithDefault(FuelType.Petrol)), 
            new Form.Item("Has GPS", new Checkbox()), 
            new Form.Item("Has sunroof", new Checkbox()), 
            new Form.Item("Daily rate $", new TextBox { Placeholder = "Default: 50.0"}), 
            new Form.Item("Colour", new TextBox { Placeholder = "Black"})
        }, new Button("Submit"));

        public FleetManager(Fleet fleet)
        {
            _fleet = fleet;
            AddVehicle.Submitted += AddVehicleOnSubmitted;
        }

        private void AddVehicleOnSubmitted(object sender, Form.SubmittedEventArgs e)
        {
            if (!e.Data.TryGet("Registration", out string rego) || !RegoRegex.IsMatch(rego))
            {
                e.Result = "Invalid rego";
                return;
            }

            if (!e.Data.TryGet("Grade", out VehicleGrade grade))
            {
                e.Result = "Grade is required";
                return;
            }

            if (!e.Data.TryGet("Make", out string make) || make.IsEmpty())
            {
                e.Result = "Make is required";
                return;
            }

            if (!e.Data.TryGet("Model", out string model) || make.IsEmpty())
            {
                e.Result = "Model is required";
                return;
            }

            if (!e.Data.TryGet("Year", out string yearStr) || 
                yearStr.Length != 4 || !int.TryParse(yearStr, out var year))
            {
                e.Result = "Year is invalid";
                return;
            }

            if (!e.Data.TryGet("Seat count", out string seatCountStr) || 
                 !int.TryParse(seatCountStr.IsEmpty() ? "4" : seatCountStr, out var seatCount))
            {
                e.Result = "Seat count is invalid";
                return;
            }

            if (!e.Data.TryGet("Daily rate $", out string dailyRateStr) || 
                 !double.TryParse(dailyRateStr.IsEmpty() ? "50" : dailyRateStr, out var dailyRate))
            {
                e.Result = $"Daily rate is invalid";
                return;
            }

            if (!e.Data.TryGet("Colour", out string colour)) {
                e.Result = "Colour is required";
                return;
            }

            if (colour.IsEmpty()) colour = "Black";
            
            var vehicle = new Vehicle
            {
                Registration = rego.ToUpper(),
                Grade = grade,
                Make = make,
                Model = model,
                Year = year,
                SeatCount = seatCount,
                Transmission = e.Data.Get<TransmissionType>("Transmission"),
                Fuel = e.Data.Get<FuelType>("Fuel type"),
                HasGps = e.Data.Get<bool>("Has GPS"),
                HasSunRoof = e.Data.Get<bool>("Has sunroof"),
                DailyRate = dailyRate,
                Colour = colour
            };

            e.Result = _fleet.AddVehicle(vehicle) ? "Added vehicle" : "Registration in use";
        }
    }
}
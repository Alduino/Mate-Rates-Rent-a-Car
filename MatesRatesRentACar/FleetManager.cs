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
using MRRC.SearchParser;
using MRRC.SearchParser.Parts;

namespace MateRatesRentACar
{
    /// <summary>
    /// Fleet management menu
    /// </summary>
    public class FleetManager
    {
        private static readonly Regex RegoRegex = new Regex("^\\d{3}[A-Za-z]{3}");
        
        private readonly Fleet _fleet;

        [MenuItem] public OneOf FleetSearch { get; } = new OneOf(new Dictionary<string, Component>
        {
            {
                "initial search",
                new Form("Find Vehicle", new []
                {
                    new Form.Item("Search", new TextBox<SearchBoxRenderer.State>(contentsRenderer: new SearchBoxRenderer())), 
                }, new Button("Submit"))
            },
            {
                "customer list",
                new Form("Find Vehicle", new []
                {
                    new Form.Item("Search", new TextBox<SearchBoxRenderer.State>(contentsRenderer: new SearchBoxRenderer())),
                    new Form.Item("Results", new Select(new [] { "Select one" }).WithDefault("Select one"))
                }, new Button("Search again"))
            }
        }, "initial search", "Find Vehicle");

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

        [MenuItem] public OneOf ModifyVehicle { get; } = new OneOf(new Dictionary<string, Component>
        {
            {
                "search",
                new Form("Find Vehicle", new []
                {
                    new Form.Item("Registration", new TextBox { Placeholder = "123ABC" }), 
                }, new Button("Search"))
            },
            {
                "modify",
                new Form("Update Vehicle", new []
                {
                    new Form.Item("Registration", new TextBox { ReadOnly = true}), 
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
                }, new Button("Submit"))
            }
        }, "search", "Modify Vehicle");

        [MenuItem] public OneOf DeleteVehicle { get; } = new OneOf(new Dictionary<string, Component>
        {
            {
                "select",
                new Form("Find Vehicle", new []
                {
                    new Form.Item("Registration", new TextBox { Placeholder = "123ABC"}), 
                }, new Button("Search"))
            },
            {
                "confirm",
                new Form("Confirm Deletion", new []
                {
                    new Form.Item("Registration", new TextBox { ReadOnly = true }), 
                    new Form.Item("Make", new TextBox { ReadOnly = true }), 
                    new Form.Item("Model", new TextBox { ReadOnly = true }), 
                    new Form.Item("Year", new TextBox { ReadOnly = true }), 
                }, new Button("Confirm"))
            }
        }, "select", "Delete Vehicle");

        public FleetManager(Fleet fleet)
        {
            _fleet = fleet;
            AddVehicle.Submitted += AddVehicleOnSubmitted;
            FleetSearch.GetComponent<Form>("initial search").Submitted += FleetSearchOnSearch;
            FleetSearch.GetComponent<Form>("customer list").Submitted += FleetSearchOnSearch;
            ModifyVehicle.GetComponent<Form>("search").Submitted += ModifyVehicleOnSearch;
            ModifyVehicle.GetComponent<Form>("modify").Submitted += ModifyVehicleOnSubmit;
            DeleteVehicle.GetComponent<Form>("select").Submitted += DeleteVehicleOnSearch;
            DeleteVehicle.GetComponent<Form>("confirm").Submitted += DeleteVehicleOnSubmitted;
        }

        private void FleetSearchOnSearch(object sender, Form.SubmittedEventArgs e)
        {
            var options = _fleet.Vehicles.Select(v => new[]
            {
                new Tuple<string, Vehicle>(v.Registration.ToString(), v),
                new Tuple<string, Vehicle>(v.Grade.ToString(), v),
                new Tuple<string, Vehicle>(v.Make, v),
                new Tuple<string, Vehicle>(v.Model, v),
                new Tuple<string, Vehicle>(v.Year.ToString(), v),
                new Tuple<string, Vehicle>(v.SeatCount.ToString(), v),
                new Tuple<string, Vehicle>(v.Transmission.ToString(), v),
                new Tuple<string, Vehicle>(v.Fuel.ToString(), v),
                new Tuple<string, Vehicle>(v.HasGps ? "GPS" : "No GPS", v),
                new Tuple<string, Vehicle>(v.HasSunRoof ? "Sun roof" : "No sun roof", v),
                new Tuple<string, Vehicle>(v.Colour, v)
            }).SelectMany(it => it).ToArray();

            if (e.Data.Get<string>("Search").IsEmpty())
            {
                e.Result = "  Enter a search term";
                return;
            }
            
            var parser = new MrrcParser(e.Data.Get<string>("Search").ToUpperInvariant());
            var result = parser.Parse(parser.Tokenise());

            if (result is FailedParseResult<Expression> failure)
            {
                e.Result = failure.Message;
                return;
            }

            var success = (SuccessfulParseResult<Expression>) result;

            var matches = success.Result.Matches(options);

            if (matches.Length == 0)
            {
                e.Result = "  No results where found";
                return;
            }

            var customerList = FleetSearch.GetComponent<Form>("customer list");
            
            customerList.Set("Search", e.Data.Get<string>("Search"));
            customerList.GetComponent<Select>("Results").SetNewMembers(matches.Select(m => m.Item2.ToString()).ToArray());
            
            FleetSearch.ActiveComponent = "customer list";
        }

        private void DeleteVehicleOnSubmitted(object sender, Form.SubmittedEventArgs e)
        {
            if (!_fleet.RemoveVehicle(e.Data.Get<string>("Registration")))
            {
                e.Result = "Could not delete";
            }
            else
            {
                e.Result = "Deleted vehicle";
            }
        }

        private void DeleteVehicleOnSearch(object sender, Form.SubmittedEventArgs e)
        {
            var rego = e.Data.Get<string>("Registration").ToUpper();
            if (!RegoRegex.IsMatch(rego))
            {
                e.Result = "Invalid rego";
                return;
            }

            if (_fleet.Vehicles.All(v => v.Registration != rego))
            {
                e.Result = "No vehicle with that rego";
                return;
            }

            if (_fleet.IsRented(rego))
            {
                e.Result = "Vehicle is under rent";
                return;
            }

            var vehicle = _fleet.Vehicles.First(v => v.Registration == rego);

            var confirmForm = DeleteVehicle.GetComponent<Form>("confirm");
            confirmForm.Set("Registration", rego);
            confirmForm.Set("Make", vehicle.Make);
            confirmForm.Set("Model", vehicle.Model);
            confirmForm.Set("Year", vehicle.Year.ToString());

            DeleteVehicle.ActiveComponent = "confirm";
        }

        private void ModifyVehicleOnSubmit(object sender, Form.SubmittedEventArgs e)
        {
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

            var vehicle = _fleet.Vehicles
                .First(v => v.Registration == e.Data.Get<string>("Registration"));

            vehicle.Grade = grade;
            vehicle.Make = make;
            vehicle.Model = model;
            vehicle.Year = year;
            vehicle.SeatCount = seatCount;
            vehicle.Transmission = e.Data.Get<TransmissionType>("Transmission");
            vehicle.Fuel = e.Data.Get<FuelType>("Fuel type");
            vehicle.HasGps = e.Data.Get<bool>("Has GPS");
            vehicle.HasSunRoof = e.Data.Get<bool>("Has sunroof");
            vehicle.DailyRate = dailyRate;
            vehicle.Colour = colour;

            e.Result = "Successfully updated vehicle";
        }

        private void ModifyVehicleOnSearch(object sender, Form.SubmittedEventArgs e)
        {
            var rego = e.Data.Get<string>("Registration").ToUpper();
            if (!RegoRegex.IsMatch(rego))
            {
                e.Result = "Invalid rego";
                return;
            }

            var vehicle = _fleet.Vehicles.FirstOrDefault(v => v.Registration == rego.ToUpper());
            if (vehicle == null)
            {
                e.Result = "No vehicle with that rego";
                return;
            }

            var modifyForm = ModifyVehicle.GetComponent<Form>("modify");
            modifyForm.Set("Registration", rego);
            modifyForm.Set("Grade", vehicle.Grade);
            modifyForm.Set("Make", vehicle.Make);
            modifyForm.Set("Model", vehicle.Model);
            modifyForm.Set("Year", vehicle.Year.ToString());
            modifyForm.Set("Seat count", vehicle.SeatCount.ToString());
            modifyForm.Set("Transmission", vehicle.Transmission);
            modifyForm.Set("Fuel type", vehicle.Fuel);
            modifyForm.Set("Has GPS", vehicle.HasGps);
            modifyForm.Set("Has sunroof", vehicle.HasSunRoof);
            modifyForm.Set("Daily rate $", vehicle.DailyRate.ToString(CultureInfo.InvariantCulture));
            modifyForm.Set("Colour", vehicle.Colour);

            ModifyVehicle.ActiveComponent = "modify";
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
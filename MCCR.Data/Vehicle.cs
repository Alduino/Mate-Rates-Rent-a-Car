﻿using CsvHelper.Configuration.Attributes;

namespace MCCR.Data
{
    public class Vehicle
    {
        public string Registration { get; set; }
        public VehicleGrade Grade { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        [Name("NumSeats")] public int SeatCount { get; set; } = 4;
        public TransmissionType Transmission { get; set; } = TransmissionType.Manual;
        public FuelType Fuel { get; set; } = FuelType.Petrol;
        [Name("GPS")] public bool HasGps { get; set; } = false;
        [Name("SunRoof")] public bool HasSunRoof { get; set; } = false;
        public double DailyRate { get; set; } = 50;
        public string Colour { get; set; } = "Black";

        public Vehicle(string rego, VehicleGrade grade, string make, string model, int year)
        {
            Registration = rego;
            Grade = grade;
            Make = make;
            Model = model;
            Year = year;
        }
    }
}
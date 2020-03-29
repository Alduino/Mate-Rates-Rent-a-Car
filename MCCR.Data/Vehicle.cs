using CsvHelper.Configuration.Attributes;

namespace MCCR.Data
{
    public class Vehicle
    {
        public string Registration { get; set; }
        public VehicleGrade Grade { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        [Name("NumSeats")]
        public int SeatCount { get; set; }
        public TransmissionType Transmission { get; set; }
        public FuelType Fuel { get; set; }
        [Name("GPS")]
        public bool HasGps { get; set; }
        [Name("SunRoof")]
        public bool HasSunRoof { get; set; }
        public double DailyRate { get; set; }
        public string Colour { get; set; }
    }
}
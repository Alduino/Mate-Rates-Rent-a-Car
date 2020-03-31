using MRRC.Guacamole.Components;
using MRRC.Guacamole.MenuGeneration;

namespace MateRatesRentACar
{
    public class CustomerManager
    {
        [MenuItem]
        public TextBox AddCustomer { get; } = new TextBox("Add Customer");
    }
}
using MRRC.Guacamole.Components.Forms;
using MRRC.Guacamole.MenuGeneration;

namespace MateRatesRentACar
{
    public class CustomerManager
    {
        [MenuItem]
        public Form AddCustomer { get; } = new Form("Add Customer", new []
        {
            new Form.Item("Name", new TextBox()), 
        }, new Button("Submit"));
    }
}
using MRRC.Guacamole.Components.Forms;
using MRRC.Guacamole.MenuGeneration;

namespace MateRatesRentACar
{
    public class CustomerManager
    {
        [MenuItem]
        public Form AddCustomer { get; } = new Form("Add Customer", new []
        {
            new Form.Item("Title", new TextBox()),
            new Form.Item("Given Names", new TextBox()),
            new Form.Item("Surname", new TextBox()),
            new Form.Item("DOB (dd/mm/yyyy)", new TextBox()),
        }, new Button("Submit"));
    }
}
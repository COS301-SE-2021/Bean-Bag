namespace BeanBag.Models
{
    /* This class is used to get and set all the variables related to the QRCode*/
    public class QrCodeModel
    {
        // The unique identifier for every QR code. This QR number is used in coupling
        // the QR code to the Item through coupling with itemID as the primary key and QRNumber as the foreign key
        // in the item table.
        public string QrCodeNumber { get; set; }

    }
}

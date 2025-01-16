namespace SimpleNET.Models
{
    public class Rental
    {
        public int RentalID { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }  // 추가된 Navigation Property
        public string RentedBy { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}

namespace Luftreise_Command_project_.Models
{
    public class SearchModels
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DateTime FlightDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}

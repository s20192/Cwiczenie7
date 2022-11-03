namespace ClientTripExercise.Models.DTOs.response
{
    public class TripDto
    {
        public TripDto()
        {
            Clients = new HashSet<ClientDto>();
            Countries = new HashSet<CountryDto>();
        }

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }

        public virtual ICollection<ClientDto> Clients { get; set; }

        public virtual ICollection<CountryDto> Countries { get; set; }
    }

    public class ClientDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CountryDto
    {
        public string Name { get; set; }
    }
}

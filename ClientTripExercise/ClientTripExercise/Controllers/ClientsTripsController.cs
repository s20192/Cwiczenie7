using ClientTripExercise.Models;
using ClientTripExercise.Models.DTOs.request;
using ClientTripExercise.Models.DTOs.response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientTripExercise.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ClientsTripsController : ControllerBase
    {
        private s20192Context _context;

        public ClientsTripsController(s20192Context context)
        {
            _context = context;
        }

        [HttpGet("trips")]
        public async Task<IActionResult> GetTrips()
        {
            var context = new s20192Context();

            var trips = context.Trips
                .Include(c => c.IdCountries)
                .Include(c => c.ClientTrips).ThenInclude(c => c.IdClientNavigation)
                .OrderByDescending(d => d.DateFrom)
                .Select(t =>
                new TripDto()
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = (ICollection<CountryDto>)t.IdCountries.Select(c => new CountryDto
                    {
                        Name = c.Name
                    }),
                    Clients = (ICollection<ClientDto>)t.ClientTrips.Select(c => c.IdClientNavigation)
                    .Select(c => new ClientDto
                    {
                        FirstName = c.FirstName,
                        LastName = c.LastName
                    })
                });

            return Ok(trips);
        }

        [HttpDelete("clients/{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var context = new s20192Context();

            Client client = context.Clients
                .Include(c => c.ClientTrips)
                .Where(c => c.IdClient == idClient).FirstOrDefault();

            if (client == null)
            {
                return NotFound("Nie ma takiego klienta w bazie");

            }

            if (client.ClientTrips.Count() != 0)
            {
                return BadRequest("Nie można usunąć - klient posiada przypisane wycieczki");
            }

            context.Clients.Attach(client);
            context.Clients.Remove(client);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("trips/{idTrip}/clients")]
        public async Task<IActionResult> AddClientToTrip(int idTrip, ClientDtoReq clientDto)
        {
            var context = new s20192Context();

            Client client = context.Clients.Where(c => c.Pesel == clientDto.Pesel).FirstOrDefault();
            if (client == null)
            {
                client = new Client
                {
                    FirstName = clientDto.FirstName,
                    LastName = clientDto.LastName,
                    Email = clientDto.Email,
                    Pesel = clientDto.Pesel,
                    Telephone = clientDto.Telephone
                };
                context.Clients.Add(client);
                context.SaveChanges();
            }

            Trip trip = context.Trips.Where(c => c.IdTrip == idTrip).FirstOrDefault();
            if (trip == null)
            {
                return NotFound("Podana wycieczka nie istnieje");
            }
            int clientId = context.Clients.Where(c => c.Pesel == clientDto.Pesel).Select(c => c.IdClient).FirstOrDefault();

            ClientTrip clientTrip = context.ClientTrips.Where(c => c.IdTrip == clientDto.IdTrip &&
                                                                  c.IdClient == clientId).FirstOrDefault();

            if (clientTrip != null)
            {
                return BadRequest("Klient jest już zapisany na daną wycieczkę");
            }

            clientTrip = new ClientTrip
            {
                IdClient = clientId,
                IdTrip = idTrip,
                PaymentDate = clientDto.PaymentDate,
                RegisteredAt = DateTime.Now
            };

            context.ClientTrips.Add(clientTrip);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}

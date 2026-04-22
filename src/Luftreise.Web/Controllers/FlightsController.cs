using Luftreise.Application.DTOs;
using Luftreise.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Luftreise.Web.Controllers;

public class FlightsController : Controller
{
    private readonly IMediator _mediator;

    public FlightsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Search()
    {
        return View(new FlightSearchDto());
    }

    [HttpPost]
    public async Task<IActionResult> Search(FlightSearchDto searchDto)
    {
        if (!ModelState.IsValid)
            return View(searchDto);

        var query = new SearchFlightsQuery(
            searchDto.DepartureCity,
            searchDto.ArrivalCity,
            searchDto.DepartureDate
        );

        var flights = await _mediator.Send(query);
        return View("Results", flights);
    }

    [HttpGet]
    public async Task<IActionResult> Results(string departureCity, string arrivalCity, DateTime departureDate)
    {
        var query = new SearchFlightsQuery(departureCity, arrivalCity, departureDate);
        var flights = await _mediator.Send(query);
        return View(flights);
    }
}

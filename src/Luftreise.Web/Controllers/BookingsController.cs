using Luftreise.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Luftreise.Web.Controllers;

public class BookingsController : Controller
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult Create(int flightId)
    {
        ViewBag.FlightId = flightId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int flightId, int numberOfPassengers)
    {
        var userId = 1; // TODO: Get from authentication

        var command = new CreateBookingCommand(flightId, userId, numberOfPassengers);
        var booking = await _mediator.Send(command);

        return RedirectToAction("Confirmation", new { id = booking.Id });
    }

    [HttpGet]
    public IActionResult Confirmation(int id)
    {
        return View(id);
    }
}

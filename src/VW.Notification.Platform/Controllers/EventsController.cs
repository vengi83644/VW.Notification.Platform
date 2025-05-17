namespace VW.Notification.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationEventDTO request)
    {
        if (request == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid request data.");
        }

        var notificationEvent = new NotificationEvent(request.EventId, request.EventType, request.CustomerId, request.Payload);

        await _eventService.ProcessEventAsync(notificationEvent);

        return Accepted("Event received and is being processed.");
    }
}

namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class EventController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(PaginatedResponse<EventGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Event", "List")]
        public async Task<ActionResult<PaginatedResponse<EventGridModel>>> GetAllEventFilterAsync(GetAllEventFilterQuery getAllEventQuery)
        {
            var getEvents = await Mediator.Send(getAllEventQuery);
            return Ok(getEvents);
        }
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<EventGridModel>), StatusCodes.Status200OK)]
        [CheckAuthorize("Event", "List")]
        public async Task<ActionResult<ICollection<EventGridModel>>> GetAllAsync()
        {
            var getEvents = await Mediator.Send(new GetAllEventQuery());
            return Ok(getEvents);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<EventViewModel>> GetByIdAsync(string id)
        {
            var eventViewModel = new EventViewModel
            {
                UpdateModel = await Mediator.Send(new GetEventDetailsQuery { Id = id })
            };

            return Ok(eventViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EventCreateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Event", "Create")]
        public async Task<ActionResult<EventCreateModel>> CreateAsync(CreateEventCommand eventCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createEvent = await Mediator.Send(eventCreateModel);
                return Ok(createEvent);
            }

            return BadRequest(eventCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(EventUpdateModel), StatusCodes.Status200OK)]
        [CheckAuthorize("Event", "Update")]
        public async Task<ActionResult<EventUpdateModel>> UpdateAsync(UpdateEventCommand eventUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateEvent = await Mediator.Send(eventUpdateModel);
                return Ok(updateEvent);
            }

            return BadRequest(eventUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [CheckAuthorize("Event", "Delete")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteEvent = await Mediator.Send(new DeleteEventCommand { Id = id });
            return Ok(deleteEvent);
        }

        [HttpGet("{accountUnitId}")]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListEventByAccountUnitAsync(int accountUnitId)
        {
            var expenseEvent = await Mediator.Send(new SelectListEventByAccountUnitQuery { AccountUnitId = accountUnitId });
            return Ok(expenseEvent);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SelectModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SelectModel>>> GetSelectListEventAsync()
        {
            var eventList = await Mediator.Send(new SelectListEventQuery());
            return Ok(eventList);
        }
    }
}
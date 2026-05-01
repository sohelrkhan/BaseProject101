namespace SadaqaAccounting.Api.Controllers.MasterSettings
{
    public class NotificationController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<NotificationGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<NotificationGridModel>>> GetAllAsync()
        {
            var getNotifications = await Mediator.Send(new GetAllNotificationQuery());
            return Ok(getNotifications);
        }

        [HttpGet("{receiverEmployeeId}")]
        [ProducesResponseType(typeof(ICollection<NotificationGridModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ICollection<NotificationGridModel>>> GetAllUnreadNotificationByReceiverId(string receiverEmployeeId)
        {
            var getNotifications = await Mediator.Send(new GetAllUnreadNotificationByReceiverQuery { ReceiverEmployeeId = receiverEmployeeId });
            return Ok(getNotifications);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificationViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationViewModel>> GetByIdAsync(int id)
        {
            var notificationViewModel = new NotificationViewModel
            {
                UpdateModel = await Mediator.Send(new GetNotificationDetailQuery { Id = id })
            };

            return Ok(notificationViewModel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(NotificationCreateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationCreateModel>> CreateAsync(CreateNotificationCommand notificationCreateModel)
        {
            if (ModelState.IsValid)
            {
                var createNotification = await Mediator.Send(notificationCreateModel);
                return Ok(createNotification);
            }

            return BadRequest(notificationCreateModel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(NotificationUpdateModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationUpdateModel>> UpdateAsync(UpdateNotificationCommand notificationUpdateModel)
        {
            if (ModelState.IsValid)
            {
                var updateNotification = await Mediator.Send(notificationUpdateModel);
                return Ok(updateNotification);
            }

            return BadRequest(notificationUpdateModel);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var deleteNotification = await Mediator.Send(new DeleteNotificationCommand { Id = id });
            return Ok(deleteNotification);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ReadNotificationById(int id)
        {
            var readNotification = await Mediator.Send(new ReadNotificationCommand { Id = id });
            return Ok(readNotification);
        }

        [HttpGet("{receiverEmployeeId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> ReadAllUnreadNotificationByReceiverId(string receiverEmployeeId)
        {
            var isAllUnread = await Mediator.Send(new ReadAllUnreadNotificationByReceiverIdCommand { ReceiverEmployeeId = receiverEmployeeId });
            return Ok(isAllUnread);
        }
    }
}
namespace SadaqaAccounting.Shared.Logger
{
    public class JobSchedulerLogger
    {
        private readonly ILogger _logger;
        public JobSchedulerLogger(ILogger<JobSchedulerLogger> logger) => _logger = logger;

        public void WriteJobScheduleLog(string logMessage)
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt} {logMessage}");
        }
    }
}
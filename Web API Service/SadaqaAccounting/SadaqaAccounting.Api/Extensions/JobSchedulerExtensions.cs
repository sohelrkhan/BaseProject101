namespace SadaqaAccounting.Api.Extensions
{
    public static class JobSchedulerExtensions
    {
        public static void ConfigureRecurringJobs(this IServiceProvider serviceProvider)
        {
            var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

            // Schedule daily job
            //jobManager.AddOrUpdate<EmployeeSalarySchedulerJob>("update-employee-salary-daily", job => job.Execute(),"0 0 * * *"); // Every day at 12:00 AM
            //jobManager.AddOrUpdate<DailyAttendanceJobScheduler>("update-attendance-daily", job => job.Execute(), "0 10 * * *"); // Every day at 10:00 AM
        }
    }
}
using Hangfire;
using ManekiApp.Server.Models.ManekiAppDB;

namespace ManekiApp.TelegramPayBot;

public class UserSubscriptionJobManager
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserSubscriptionJobManager(IBackgroundJobClient backgroundJobClient, IServiceScopeFactory serviceScopeFactory)
    {
        _backgroundJobClient = backgroundJobClient;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void ScheduleUserSubscriptionDeletionJob(UserSubscription userSubscription)
    {
        // Remove the existing job if any
        if (!string.IsNullOrEmpty(userSubscription.JobId))
        {
            _backgroundJobClient.Delete(userSubscription.JobId);
        }

        // Schedule a new job for the subscription's EndsAt date
        var delay = userSubscription.EndsAt - DateTime.UtcNow;
        userSubscription.JobId = _backgroundJobClient.Schedule<UserSubscriptionJobManager>(
            x => x.DeleteUserSubscription(userSubscription.Id, null),
            delay);
    }

    public async Task DeleteUserSubscription(Guid id, IJobCancellationToken token)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        var userSubscription = await context.UserSubscriptions.FindAsync(id);

        if (userSubscription != null)
        {
            context.UserSubscriptions.Remove(userSubscription);
            await context.SaveChangesAsync();
        }
    }
}
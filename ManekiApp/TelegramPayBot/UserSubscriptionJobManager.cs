using Hangfire;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramPayBot;

/// <summary>
/// Class UserSubscriptionJobManager.
/// </summary>
public class UserSubscriptionJobManager
{
    /// <summary>
    /// The background job client
    /// </summary>
    private readonly IBackgroundJobClient _backgroundJobClient;
    /// <summary>
    /// The service scope factory
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSubscriptionJobManager"/> class.
    /// </summary>
    /// <param name="backgroundJobClient">The background job client.</param>
    /// <param name="serviceScopeFactory">The service scope factory.</param>
    public UserSubscriptionJobManager(IBackgroundJobClient backgroundJobClient, IServiceScopeFactory serviceScopeFactory)
    {
        _backgroundJobClient = backgroundJobClient;
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// Schedules the user subscription deletion job.
    /// </summary>
    /// <param name="userSubscription">The user subscription.</param>
    /// <param name="level0SubscriptionId">The level0 subscription identifier.</param>
    /// <returns>System.String.</returns>
    public string ScheduleUserSubscriptionDeletionJob(UserSubscription userSubscription, Guid level0SubscriptionId)
    {
        // Remove the existing job if any
        if (!string.IsNullOrEmpty(userSubscription.JobId))
        {
            _backgroundJobClient.Delete(userSubscription.JobId);
        }

        // Schedule a new job for the subscription's EndsAt date
        var delay = userSubscription.EndsAt - DateTime.UtcNow;
        return _backgroundJobClient.Schedule<UserSubscriptionJobManager>(
            x => x.UpdateUserSubscription(userSubscription.Id, level0SubscriptionId, null),
            delay);
    }

    /// <summary>
    /// Updates the user subscription.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="level0SubscriptionId">The level0 subscription identifier.</param>
    /// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    public async Task UpdateUserSubscription(Guid id, Guid level0SubscriptionId, IJobCancellationToken token)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        var userSubscription = await context.UserSubscriptions.FirstOrDefaultAsync(x => x.Id == id);
        if (userSubscription != null)
        {
            userSubscription.SubscriptionId = level0SubscriptionId;
            await context.SaveChangesAsync();
        }
    }
}
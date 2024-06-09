using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages;

/// <summary>
/// Class Analytics.
/// Implements the <see cref="ComponentBase" />
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class Analytics
{
    /// <summary>
    /// Gets or sets the js runtime.
    /// </summary>
    /// <value>The js runtime.</value>
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the navigation manager.
    /// </summary>
    /// <value>The navigation manager.</value>
    [Inject] protected NavigationManager NavigationManager { get; set; }

    /// <summary>
    /// Gets or sets the dialog service.
    /// </summary>
    /// <value>The dialog service.</value>
    [Inject] protected DialogService DialogService { get; set; }

    /// <summary>
    /// Gets or sets the tooltip service.
    /// </summary>
    /// <value>The tooltip service.</value>
    [Inject] protected TooltipService TooltipService { get; set; }

    /// <summary>
    /// Gets or sets the context menu service.
    /// </summary>
    /// <value>The context menu service.</value>
    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    /// <summary>
    /// Gets or sets the notification service.
    /// </summary>
    /// <value>The notification service.</value>
    [Inject] protected NotificationService NotificationService { get; set; }

    /// <summary>
    /// Gets or sets the security.
    /// </summary>
    /// <value>The security.</value>
    [Inject] protected SecurityService Security { get; set; }

    /// <summary>
    /// Gets or sets the maneki application database service.
    /// </summary>
    /// <value>The maneki application database service.</value>
    [Inject] protected ManekiAppDBService manekiAppDbService { get; set; }

    /// <summary>
    /// The user subscriptions
    /// </summary>
    private IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
    /// <summary>
    /// The subscriptions
    /// </summary>
    private IEnumerable<Subscription> subscriptions = new List<Subscription>();
    /// <summary>
    /// The chart items
    /// </summary>
    private DataItem[] chartItems;
    /// <summary>
    /// The subscribers data
    /// </summary>
    private IQueryable<SubscriberDetails> subscribersData;
    /// <summary>
    /// Gets or sets the lifetime profit.
    /// </summary>
    /// <value>The lifetime profit.</value>
    private decimal? lifetimeProfit { get; set; } = 0;
    /// <summary>
    /// The show data labels
    /// </summary>
    private bool showDataLabels = false;
    /// <summary>
    /// The is loading
    /// </summary>
    private bool isLoading;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        var userId = Security.User.Id;

        //get author page
        var filter = $"UserId eq '{userId}'";
        var authorPageOdata = await manekiAppDbService.GetAuthorPages(filter, top: 1);
        var authorPage = authorPageOdata.Value.FirstOrDefault();
        if (authorPage == null)
        {
            NavigationManager.NavigateTo("/feed");
            return;
        }
        //get all author subscriptions
        filter = $"AuthorPageId eq {authorPage.Id}";
        var subscriptionsOData = await manekiAppDbService.GetSubscriptions(filter);
        subscriptions = subscriptionsOData.Value.ToList();


        //get all author patrons
        if (subscriptions != null && subscriptions.Any())
        {
            var subscriptionIds = subscriptions.Select(sub => sub.Id).ToList();
            var idsFilter = string.Join(" or ", subscriptionIds.Select(id => $"SubscriptionId eq {id}"));
            var userSubscriptionsOData = await manekiAppDbService.GetUserSubscriptions(idsFilter);
            userSubscriptions = userSubscriptionsOData.Value.AsODataEnumerable();
        }

        chartItems = new DataItem[subscriptions.Count()];

        var index = 0;
        foreach (var subscription in subscriptions)
        {
            var subscriptionUserCount = userSubscriptions.Count(x => x.SubscriptionId == subscription.Id);
            chartItems[index] = new DataItem
            {
                SubscriptionName = subscription.Title,
                SubscriptionUserCount = subscriptionUserCount
            };
            index++;
        }

        await LoadTableData();
    }

    /// <summary>
    /// Loads the table data.
    /// </summary>
    private async Task LoadTableData()
    {
        isLoading = true;
        var subscriberDetails = new List<SubscriberDetails>();
        foreach (var userSubscription in userSubscriptions)
            subscriberDetails.Add(new SubscriberDetails()
            {
                SubscriberId = userSubscription.UserId,
                SubscriptionType = subscriptions
                    .FirstOrDefault(x => x.Id == userSubscription.SubscriptionId)?
                    .Title,
                Amount = subscriptions
                    .FirstOrDefault(x => x.Id == userSubscription.SubscriptionId)?
                    .Price,
                StartDate = userSubscription.SubscribedAt.Date
            });

        subscribersData = subscriberDetails.AsQueryable();
        lifetimeProfit = subscribersData.Sum(x => x.Amount);
        isLoading = false;
    }

    /// <summary>
    /// Class DataItem.
    /// </summary>
    private class DataItem
    {
        /// <summary>
        /// Gets or sets the name of the subscription.
        /// </summary>
        /// <value>The name of the subscription.</value>
        public string SubscriptionName { get; set; }
        /// <summary>
        /// Gets or sets the subscription user count.
        /// </summary>
        /// <value>The subscription user count.</value>
        public int SubscriptionUserCount { get; set; }
    }

    /// <summary>
    /// Class SubscriberDetails.
    /// </summary>
    private class SubscriberDetails
    {
        /// <summary>
        /// Gets or sets the subscriber identifier.
        /// </summary>
        /// <value>The subscriber identifier.</value>
        public string SubscriberId { get; set; }
        /// <summary>
        /// Gets or sets the type of the subscription.
        /// </summary>
        /// <value>The type of the subscription.</value>
        public string SubscriptionType { get; set; }
        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public decimal? Amount { get; set; }
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }
    }
}
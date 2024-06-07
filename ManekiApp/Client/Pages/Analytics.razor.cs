using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages;

public partial class Analytics
{
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Inject] protected DialogService DialogService { get; set; }

    [Inject] protected TooltipService TooltipService { get; set; }

    [Inject] protected ContextMenuService ContextMenuService { get; set; }

    [Inject] protected NotificationService NotificationService { get; set; }

    [Inject] protected SecurityService Security { get; set; }

    [Inject] protected ManekiAppDBService manekiAppDbService { get; set; }

    private IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
    private IEnumerable<Subscription> subscriptions = new List<Subscription>();
    private DataItem[] chartItems;
    private IQueryable<SubscriberDetails> subscribersData;
    private bool showDataLabels = false;
    private bool isLoading;

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

        isLoading = false;
    }

    private class DataItem
    {
        public string SubscriptionName { get; set; }
        public int SubscriptionUserCount { get; set; }
    }

    private class SubscriberDetails
    {
        public string SubscriberId { get; set; }
        public string SubscriptionType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime StartDate { get; set; }
    }
}
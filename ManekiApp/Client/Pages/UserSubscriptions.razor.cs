using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    public partial class UserSubscriptions
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject] protected ManekiAppDBService ManekiAppDB { get; set; }


        private IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
        private IEnumerable<Subscription> subscriptions = new List<Subscription>();


        private string GetSubscriptionTitle(Guid userSubscriptionId)
        {
            
            return subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)?.Title;
        }
        private string GetSubscriptionPrice(Guid userSubscriptionId)
        {
            
            return subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)?.Price + "$";
        }

        protected override async Task OnInitializedAsync()
        {
            var userId = Security.User.Id;
            var filter = $"UserId eq '{userId}'";
            var userSubscriptionsOData = await ManekiAppDB.GetUserSubscriptions(filter: filter);
            userSubscriptions = userSubscriptionsOData.Value;
            var userSubscriptionIds = userSubscriptions.Select(sub => sub.SubscriptionId).ToList();
            var idsFilter = string.Join(" or ", userSubscriptionIds.Select(id => $"Id eq {id}"));
            var subscriptionsOData = await ManekiAppDB.GetSubscriptions(filter: idsFilter);

            subscriptions = subscriptionsOData.Value;
        }
        
        private async Task CancelSubscription(UserSubscription userSubscription)
        {
            userSubscription.IsCanceled = true;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }
    }
}
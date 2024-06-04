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
    public partial class Feed
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
        
        [Inject]
        protected ManekiAppDBService ManekiAppDBService { get; set; }

        protected List<Post> userFeedPosts = new List<Post>();
        protected IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
        protected IEnumerable<Subscription> subscriptions = new List<Subscription>();
        protected IEnumerable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> authorsUserFollows = new List<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>();        
        protected override async Task OnInitializedAsync()
        {
            if (Security.User != null && !string.IsNullOrEmpty(Security.User.Id))
            {
                var userId = Security.User.Id;
                await LoadFeedData(userId);
            }
        }

        private async Task LoadFeedData(string userId)
        {
            await LoadUserSubscriptions(userId);
            await LoadAuthorsAndSubscriptions();
            await LoadPostsFromFollowedAuthors();
        }

        private async Task LoadUserSubscriptions(string userId)
            // loads user subscriptions from the database
        {
            var filter = $"UserId eq '{userId}'";
            var userSubscriptionsOData = await ManekiAppDBService.GetUserSubscriptions(filter: filter);
            userSubscriptions = userSubscriptionsOData.Value;
        }

        private async Task LoadAuthorsAndSubscriptions()
        // loads authors and their subscriptions from the database
        {
            var subscriptionIds = userSubscriptions.Select(sub => sub.SubscriptionId).ToList();
            if (subscriptionIds.Any())
            {
                var idsFilter = string.Join(" or ", subscriptionIds.Select(id => $"Id eq {id}"));
                var subscriptionsOData = await ManekiAppDBService.GetSubscriptions(filter: idsFilter);
                subscriptions = subscriptionsOData.Value;

                var authorIds = subscriptions.Select(sub => sub.AuthorPageId).Distinct().ToList();
                if (authorIds.Any())
                {
                    var authorsFilter = string.Join(" or ", authorIds.Select(id => $"Id eq {id}"));
                    var authorsOData = await ManekiAppDBService.GetAuthorPages(filter: authorsFilter);
                    authorsUserFollows = authorsOData.Value;
                }
            }
        }

        private async Task LoadPostsFromFollowedAuthors()
            // loads posts from authors on which the user is subscribed
        {
            var authorIds = authorsUserFollows.Select(author => author.Id).ToList();
            if (authorIds.Any())
            {
                var postsFilter = string.Join(" or ", authorIds.Select(id => $"AuthorPageId eq {id}"));
                var postsOData = await ManekiAppDBService.GetPosts(filter: postsFilter, top: 10, expand: "AuthorPage");
                var allPosts = postsOData.Value;

                // apply filtering based on subscription level
                userFeedPosts = allPosts.Where(post => 
                    {
                        var subscription = subscriptions.FirstOrDefault(sub => sub.AuthorPageId == post.AuthorPageId);
                        if (subscription != null)
                        {
                            var userSubscription = userSubscriptions.FirstOrDefault(us => us.SubscriptionId == subscription.Id);
                            if (userSubscription != null)
                            {
                                return subscription.PermissionLevel >= post.MinLevel;
                            }
                        }
                        return false;
                    })
                    .OrderByDescending(post => post.CreatedAt)
                    .ToList();
            }
        }
    }
}
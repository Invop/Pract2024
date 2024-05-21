using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using ManekiApp.Server.Data;

namespace ManekiApp.Server
{
    public partial class ManekiAppDBService
    {
        ManekiAppDBContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ManekiAppDBContext context;
        private readonly NavigationManager navigationManager;

        public ManekiAppDBService(ManekiAppDBContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportAuthorPagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAuthorPagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAuthorPagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> items);

        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPages(Query query = null)
        {
            var items = Context.AuthorPages.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAuthorPagesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAuthorPageGet(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnGetAuthorPageById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> items);


        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPageById(Guid id)
        {
            var items = Context.AuthorPages
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetAuthorPageById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAuthorPageGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnAfterAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> CreateAuthorPage(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorpage)
        {
            OnAuthorPageCreated(authorpage);

            var existingItem = Context.AuthorPages
                              .Where(i => i.Id == authorpage.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.AuthorPages.Add(authorpage);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(authorpage).State = EntityState.Detached;
                throw;
            }

            OnAfterAuthorPageCreated(authorpage);

            return authorpage;
        }

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> CancelAuthorPageChanges(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnAfterAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> UpdateAuthorPage(Guid id, ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorpage)
        {
            OnAuthorPageUpdated(authorpage);

            var itemToUpdate = Context.AuthorPages
                              .Where(i => i.Id == authorpage.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(authorpage);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAuthorPageUpdated(authorpage);

            return authorpage;
        }

        partial void OnAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnAfterAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> DeleteAuthorPage(Guid id)
        {
            var itemToDelete = Context.AuthorPages
                              .Where(i => i.Id == id)
                              .Include(i => i.Subscriptions)
                              .Include(i => i.Posts)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAuthorPageDeleted(itemToDelete);


            Context.AuthorPages.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAuthorPageDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportImagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportImagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnImagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image> items);

        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image>> GetImages(Query query = null)
        {
            var items = Context.Images.AsQueryable();

            items = items.Include(i => i.Post);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnImagesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnImageGet(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnGetImageById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image> items);


        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> GetImageById(Guid id)
        {
            var items = Context.Images
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Post);
 
            OnGetImageById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnImageGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnAfterImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> CreateImage(ManekiApp.Server.Models.ManekiAppDB.Image image)
        {
            OnImageCreated(image);

            var existingItem = Context.Images
                              .Where(i => i.Id == image.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Images.Add(image);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(image).State = EntityState.Detached;
                throw;
            }

            OnAfterImageCreated(image);

            return image;
        }

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> CancelImageChanges(ManekiApp.Server.Models.ManekiAppDB.Image item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnAfterImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> UpdateImage(Guid id, ManekiApp.Server.Models.ManekiAppDB.Image image)
        {
            OnImageUpdated(image);

            var itemToUpdate = Context.Images
                              .Where(i => i.Id == image.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(image);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterImageUpdated(image);

            return image;
        }

        partial void OnImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnAfterImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> DeleteImage(Guid id)
        {
            var itemToDelete = Context.Images
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnImageDeleted(itemToDelete);


            Context.Images.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterImageDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPostsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPostsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPostsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post> items);

        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post>> GetPosts(Query query = null)
        {
            var items = Context.Posts.AsQueryable();

            items = items.Include(i => i.AuthorPage);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPostsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPostGet(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnGetPostById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post> items);


        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> GetPostById(Guid id)
        {
            var items = Context.Posts
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AuthorPage);
 
            OnGetPostById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPostGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnAfterPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> CreatePost(ManekiApp.Server.Models.ManekiAppDB.Post post)
        {
            OnPostCreated(post);

            var existingItem = Context.Posts
                              .Where(i => i.Id == post.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Posts.Add(post);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(post).State = EntityState.Detached;
                throw;
            }

            OnAfterPostCreated(post);

            return post;
        }

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> CancelPostChanges(ManekiApp.Server.Models.ManekiAppDB.Post item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnAfterPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> UpdatePost(Guid id, ManekiApp.Server.Models.ManekiAppDB.Post post)
        {
            OnPostUpdated(post);

            var itemToUpdate = Context.Posts
                              .Where(i => i.Id == post.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(post);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPostUpdated(post);

            return post;
        }

        partial void OnPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnAfterPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> DeletePost(Guid id)
        {
            var itemToDelete = Context.Posts
                              .Where(i => i.Id == id)
                              .Include(i => i.Images)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPostDeleted(itemToDelete);


            Context.Posts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPostDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription> items);

        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription>> GetSubscriptions(Query query = null)
        {
            var items = Context.Subscriptions.AsQueryable();

            items = items.Include(i => i.AuthorPage);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSubscriptionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnSubscriptionGet(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnGetSubscriptionById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription> items);


        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscriptionById(Guid id)
        {
            var items = Context.Subscriptions
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.AuthorPage);
 
            OnGetSubscriptionById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnSubscriptionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnAfterSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> CreateSubscription(ManekiApp.Server.Models.ManekiAppDB.Subscription subscription)
        {
            OnSubscriptionCreated(subscription);

            var existingItem = Context.Subscriptions
                              .Where(i => i.Id == subscription.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Subscriptions.Add(subscription);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(subscription).State = EntityState.Detached;
                throw;
            }

            OnAfterSubscriptionCreated(subscription);

            return subscription;
        }

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> CancelSubscriptionChanges(ManekiApp.Server.Models.ManekiAppDB.Subscription item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnAfterSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> UpdateSubscription(Guid id, ManekiApp.Server.Models.ManekiAppDB.Subscription subscription)
        {
            OnSubscriptionUpdated(subscription);

            var itemToUpdate = Context.Subscriptions
                              .Where(i => i.Id == subscription.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(subscription);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterSubscriptionUpdated(subscription);

            return subscription;
        }

        partial void OnSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnAfterSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> DeleteSubscription(Guid id)
        {
            var itemToDelete = Context.Subscriptions
                              .Where(i => i.Id == id)
                              .Include(i => i.UserSubscriptions)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnSubscriptionDeleted(itemToDelete);


            Context.Subscriptions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterSubscriptionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportUserSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUserSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUserSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> items);

        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>> GetUserSubscriptions(Query query = null)
        {
            var items = Context.UserSubscriptions.AsQueryable();

            items = items.Include(i => i.Subscription);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUserSubscriptionsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUserSubscriptionGet(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnGetUserSubscriptionById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> items);


        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscriptionById(Guid id)
        {
            var items = Context.UserSubscriptions
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Subscription);
 
            OnGetUserSubscriptionById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserSubscriptionGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnAfterUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> CreateUserSubscription(ManekiApp.Server.Models.ManekiAppDB.UserSubscription usersubscription)
        {
            OnUserSubscriptionCreated(usersubscription);

            var existingItem = Context.UserSubscriptions
                              .Where(i => i.Id == usersubscription.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.UserSubscriptions.Add(usersubscription);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(usersubscription).State = EntityState.Detached;
                throw;
            }

            OnAfterUserSubscriptionCreated(usersubscription);

            return usersubscription;
        }

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> CancelUserSubscriptionChanges(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnAfterUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> UpdateUserSubscription(Guid id, ManekiApp.Server.Models.ManekiAppDB.UserSubscription usersubscription)
        {
            OnUserSubscriptionUpdated(usersubscription);

            var itemToUpdate = Context.UserSubscriptions
                              .Where(i => i.Id == usersubscription.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(usersubscription);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserSubscriptionUpdated(usersubscription);

            return usersubscription;
        }

        partial void OnUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnAfterUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> DeleteUserSubscription(Guid id)
        {
            var itemToDelete = Context.UserSubscriptions
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnUserSubscriptionDeleted(itemToDelete);


            Context.UserSubscriptions.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserSubscriptionDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportUserVerificationCodesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportUserVerificationCodesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnUserVerificationCodesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> items);

        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>> GetUserVerificationCodes(Query query = null)
        {
            var items = Context.UserVerificationCodes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUserVerificationCodesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUserVerificationCodeGet(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnGetUserVerificationCodeById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> items);


        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCodeById(Guid id)
        {
            var items = Context.UserVerificationCodes
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetUserVerificationCodeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserVerificationCodeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnAfterUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> CreateUserVerificationCode(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode userverificationcode)
        {
            OnUserVerificationCodeCreated(userverificationcode);

            var existingItem = Context.UserVerificationCodes
                              .Where(i => i.Id == userverificationcode.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.UserVerificationCodes.Add(userverificationcode);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(userverificationcode).State = EntityState.Detached;
                throw;
            }

            OnAfterUserVerificationCodeCreated(userverificationcode);

            return userverificationcode;
        }

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> CancelUserVerificationCodeChanges(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnAfterUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> UpdateUserVerificationCode(Guid id, ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode userverificationcode)
        {
            OnUserVerificationCodeUpdated(userverificationcode);

            var itemToUpdate = Context.UserVerificationCodes
                              .Where(i => i.Id == userverificationcode.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(userverificationcode);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserVerificationCodeUpdated(userverificationcode);

            return userverificationcode;
        }

        partial void OnUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnAfterUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> DeleteUserVerificationCode(Guid id)
        {
            var itemToDelete = Context.UserVerificationCodes
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnUserVerificationCodeDeleted(itemToDelete);


            Context.UserVerificationCodes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserVerificationCodeDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}
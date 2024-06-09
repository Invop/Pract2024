using ManekiApp.Server.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Text.Encodings.Web;

namespace ManekiApp.Server
{
    /// <summary>
    /// Class ManekiAppDBService.
    /// </summary>
    public partial class ManekiAppDBService
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        ManekiAppDBContext Context
        {
            get
            {
                return this.context;
            }
        }

        /// <summary>
        /// The context
        /// </summary>
        private readonly ManekiAppDBContext context;
        /// <summary>
        /// The navigation manager
        /// </summary>
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManekiAppDBService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navigationManager">The navigation manager.</param>
        public ManekiAppDBService(ManekiAppDBContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        /// <summary>
        /// Applies the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="query">The query.</param>
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


        /// <summary>
        /// Exports the author pages to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportAuthorPagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the author pages to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportAuthorPagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [author pages read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnAuthorPagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> items);

        /// <summary>
        /// Gets the author pages.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPages(Query query = null)
        {
            var items = Context.AuthorPages.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAuthorPagesRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [author page get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageGet(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [get author page by identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetAuthorPageById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> items);


        /// <summary>
        /// Gets the author page by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.AuthorPage.</returns>
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

        /// <summary>
        /// Called when [author page created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [after author page created].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Cancels the author page changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.AuthorPage.</returns>
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

        /// <summary>
        /// Called when [author page updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [after author page updated].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Called when [author page deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [after author page deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> DeleteAuthorPage(Guid id)
        {
            var itemToDelete = Context.AuthorPages
                              .Where(i => i.Id == id)
                              .Include(i => i.Posts)
                              .Include(i => i.Subscriptions)
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

        /// <summary>
        /// Exports the images to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportImagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the images to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportImagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [images read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnImagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image> items);

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.Image&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image>> GetImages(Query query = null)
        {
            var items = Context.Images.AsQueryable();

            items = items.Include(i => i.Post);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnImagesRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [image get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageGet(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [get image by identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetImageById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image> items);


        /// <summary>
        /// Gets the image by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Image.</returns>
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

        /// <summary>
        /// Called when [image created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [after image created].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Cancels the image changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Image.</returns>
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

        /// <summary>
        /// Called when [image updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [after image updated].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Called when [image deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [after image deleted].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Exports the posts to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportPostsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the posts to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportPostsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [posts read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnPostsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post> items);

        /// <summary>
        /// Gets the posts.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.Post&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post>> GetPosts(Query query = null)
        {
            var items = Context.Posts.AsQueryable();

            items = items.Include(i => i.AuthorPage);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPostsRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [post get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostGet(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [get post by identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetPostById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post> items);


        /// <summary>
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Post.</returns>
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

        /// <summary>
        /// Called when [post created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [after post created].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Cancels the post changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Post.</returns>
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

        /// <summary>
        /// Called when [post updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [after post updated].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Called when [post deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [after post deleted].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Exports the subscriptions to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the subscriptions to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [subscriptions read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription> items);

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.Subscription&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription>> GetSubscriptions(Query query = null)
        {
            var items = Context.Subscriptions.AsQueryable();

            items = items.Include(i => i.AuthorPage);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnSubscriptionsRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [subscription get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionGet(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [get subscription by identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetSubscriptionById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription> items);


        /// <summary>
        /// Gets the subscription by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Subscription.</returns>
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

        /// <summary>
        /// Called when [subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [after subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Cancels the subscription changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Subscription.</returns>
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

        /// <summary>
        /// Called when [subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [after subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Called when [subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [after subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Exports the user subscriptions to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user subscriptions to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [user subscriptions read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> items);

        /// <summary>
        /// Gets the user subscriptions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.UserSubscription&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>> GetUserSubscriptions(Query query = null)
        {
            var items = Context.UserSubscriptions.AsQueryable();

            items = items.Include(i => i.Subscription);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUserSubscriptionsRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [user subscription get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionGet(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [get user subscription by identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetUserSubscriptionById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> items);


        /// <summary>
        /// Gets the user subscription by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserSubscription.</returns>
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

        /// <summary>
        /// Called when [user subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [after user subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Cancels the user subscription changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserSubscription.</returns>
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

        /// <summary>
        /// Called when [user subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [after user subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Called when [user subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [after user subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Exports the user verification codes to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserVerificationCodesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user verification codes to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserVerificationCodesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [user verification codes read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserVerificationCodesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> items);

        /// <summary>
        /// Gets the user verification codes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>> GetUserVerificationCodes(Query query = null)
        {
            var items = Context.UserVerificationCodes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUserVerificationCodesRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [user verification code get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeGet(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [get user verification code by identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetUserVerificationCodeById(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> items);


        /// <summary>
        /// Gets the user verification code by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode.</returns>
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

        /// <summary>
        /// Called when [user verification code created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [after user verification code created].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Cancels the user verification code changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode.</returns>
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

        /// <summary>
        /// Called when [user verification code updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [after user verification code updated].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Called when [user verification code deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [after user verification code deleted].
        /// </summary>
        /// <param name="item">The item.</param>
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

        /// <summary>
        /// Exports the user chat payments to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserChatPaymentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatpayments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatpayments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user chat payments to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserChatPaymentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatpayments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatpayments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [user chat payments read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserChatPaymentsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> items);

        /// <summary>
        /// Gets the user chat payments.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatPayment&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>> GetUserChatPayments(Query query = null)
        {
            var items = Context.UserChatPayments.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUserChatPaymentsRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [user chat payment get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentGet(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [get user chat payment by user identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetUserChatPaymentByUserId(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> items);


        /// <summary>
        /// Gets the user chat payment by user identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatPayment.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> GetUserChatPaymentByUserId(string userid)
        {
            var items = Context.UserChatPayments
                              .AsNoTracking()
                              .Where(i => i.UserId == userid);


            OnGetUserChatPaymentByUserId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserChatPaymentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        /// <summary>
        /// Called when [user chat payment created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [after user chat payment created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatPaymentCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> CreateUserChatPayment(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment userchatpayment)
        {
            OnUserChatPaymentCreated(userchatpayment);

            var existingItem = Context.UserChatPayments
                              .Where(i => i.UserId == userchatpayment.UserId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.UserChatPayments.Add(userchatpayment);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(userchatpayment).State = EntityState.Detached;
                throw;
            }

            OnAfterUserChatPaymentCreated(userchatpayment);

            return userchatpayment;
        }

        /// <summary>
        /// Cancels the user chat payment changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatPayment.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> CancelUserChatPaymentChanges(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        /// <summary>
        /// Called when [user chat payment updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [after user chat payment updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatPaymentUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> UpdateUserChatPayment(string userid, ManekiApp.Server.Models.ManekiAppDB.UserChatPayment userchatpayment)
        {
            OnUserChatPaymentUpdated(userchatpayment);

            var itemToUpdate = Context.UserChatPayments
                              .Where(i => i.UserId == userchatpayment.UserId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(userchatpayment);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserChatPaymentUpdated(userchatpayment);

            return userchatpayment;
        }

        /// <summary>
        /// Called when [user chat payment deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [after user chat payment deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatPaymentDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> DeleteUserChatPayment(string userid)
        {
            var itemToDelete = Context.UserChatPayments
                              .Where(i => i.UserId == userid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnUserChatPaymentDeleted(itemToDelete);


            Context.UserChatPayments.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserChatPaymentDeleted(itemToDelete);

            return itemToDelete;
        }

        /// <summary>
        /// Exports the user chat notifications to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserChatNotificationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatnotifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatnotifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user chat notifications to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async Task ExportUserChatNotificationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatnotifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatnotifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [user chat notifications read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserChatNotificationsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> items);

        /// <summary>
        /// Gets the user chat notifications.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatNotification&gt;.</returns>
        public async Task<IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>> GetUserChatNotifications(Query query = null)
        {
            var items = Context.UserChatNotifications.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnUserChatNotificationsRead(ref items);

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Called when [user chat notification get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationGet(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [get user chat notification by user identifier].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnGetUserChatNotificationByUserId(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> items);


        /// <summary>
        /// Gets the user chat notification by user identifier.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatNotification.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> GetUserChatNotificationByUserId(string userid)
        {
            var items = Context.UserChatNotifications
                              .AsNoTracking()
                              .Where(i => i.UserId == userid);


            OnGetUserChatNotificationByUserId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserChatNotificationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        /// <summary>
        /// Called when [user chat notification created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [after user chat notification created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatNotificationCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> CreateUserChatNotification(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification userchatnotification)
        {
            OnUserChatNotificationCreated(userchatnotification);

            var existingItem = Context.UserChatNotifications
                              .Where(i => i.UserId == userchatnotification.UserId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
                throw new Exception("Item already available");
            }

            try
            {
                Context.UserChatNotifications.Add(userchatnotification);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(userchatnotification).State = EntityState.Detached;
                throw;
            }

            OnAfterUserChatNotificationCreated(userchatnotification);

            return userchatnotification;
        }

        /// <summary>
        /// Cancels the user chat notification changes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatNotification.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> CancelUserChatNotificationChanges(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        /// <summary>
        /// Called when [user chat notification updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [after user chat notification updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatNotificationUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> UpdateUserChatNotification(string userid, ManekiApp.Server.Models.ManekiAppDB.UserChatNotification userchatnotification)
        {
            OnUserChatNotificationUpdated(userchatnotification);

            var itemToUpdate = Context.UserChatNotifications
                              .Where(i => i.UserId == userchatnotification.UserId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
                throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(userchatnotification);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserChatNotificationUpdated(userchatnotification);

            return userchatnotification;
        }

        /// <summary>
        /// Called when [user chat notification deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [after user chat notification deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatNotificationDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> DeleteUserChatNotification(string userid)
        {
            var itemToDelete = Context.UserChatNotifications
                              .Where(i => i.UserId == userid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
                throw new Exception("Item no longer available");
            }

            OnUserChatNotificationDeleted(itemToDelete);


            Context.UserChatNotifications.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterUserChatNotificationDeleted(itemToDelete);

            return itemToDelete;
        }
    }
}
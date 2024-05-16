using System.Linq.Dynamic.Core;
using System.Text.Encodings.Web;
using ManekiApp.Server.Data;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace ManekiApp.Server
{
    public partial class ManekiAppDBService
    {
        private ManekiAppDBContext Context { get; }

        private readonly NavigationManager navigationManager;

        public ManekiAppDBService(ManekiAppDBContext context, NavigationManager navigationManager)
        {
            Context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset()
        {
            Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList()
                .ForEach(e => e.State = EntityState.Detached);
        }

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


        public async Task ExportUserVerificationCodesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(
                query != null
                    ? query.ToUrl(
                        $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')")
                    : $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')",
                true);
        }

        public async Task ExportUserVerificationCodesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(
                query != null
                    ? query.ToUrl(
                        $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')")
                    : $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')",
                true);
        }

        partial void OnUserVerificationCodesRead(
            ref IQueryable<UserVerificationCode> items);

        public async Task<IQueryable<UserVerificationCode>>
            GetUserVerificationCodes(Query query = null)
        {
            var items = Context.UserVerificationCodes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand) items = items.Include(p.Trim());
                }

                ApplyQuery(ref items, query);
            }

            OnUserVerificationCodesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnUserVerificationCodeGet(UserVerificationCode item);

        partial void OnGetUserVerificationCodeById(
            ref IQueryable<UserVerificationCode> items);


        public async Task<UserVerificationCode> GetUserVerificationCodeById(Guid id)
        {
            var items = Context.UserVerificationCodes
                .AsNoTracking()
                .Where(i => i.Id == id);


            OnGetUserVerificationCodeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnUserVerificationCodeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnUserVerificationCodeCreated(UserVerificationCode item);
        partial void OnAfterUserVerificationCodeCreated(UserVerificationCode item);

        public async Task<UserVerificationCode> CreateUserVerificationCode(
            UserVerificationCode userverificationcode)
        {
            OnUserVerificationCodeCreated(userverificationcode);

            var existingItem = Context.UserVerificationCodes
                .Where(i => i.Id == userverificationcode.Id)
                .FirstOrDefault();

            if (existingItem != null) throw new Exception("Item already available");

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

        public async Task<UserVerificationCode> CancelUserVerificationCodeChanges(
            UserVerificationCode item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
                entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
                entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnUserVerificationCodeUpdated(UserVerificationCode item);
        partial void OnAfterUserVerificationCodeUpdated(UserVerificationCode item);

        public async Task<UserVerificationCode> UpdateUserVerificationCode(Guid id,
            UserVerificationCode userverificationcode)
        {
            OnUserVerificationCodeUpdated(userverificationcode);

            var itemToUpdate = Context.UserVerificationCodes
                .Where(i => i.Id == userverificationcode.Id)
                .FirstOrDefault();

            if (itemToUpdate == null) throw new Exception("Item no longer available");

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(userverificationcode);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterUserVerificationCodeUpdated(userverificationcode);

            return userverificationcode;
        }

        partial void OnUserVerificationCodeDeleted(UserVerificationCode item);
        partial void OnAfterUserVerificationCodeDeleted(UserVerificationCode item);

        public async Task<UserVerificationCode> DeleteUserVerificationCode(Guid id)
        {
            var itemToDelete = Context.UserVerificationCodes
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if (itemToDelete == null) throw new Exception("Item no longer available");

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
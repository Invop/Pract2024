
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Radzen;

namespace ManekiApp.Client
{
    public partial class ManekiAppDBService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public ManekiAppDBService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/ManekiAppDB/");
        }


        public async System.Threading.Tasks.Task ExportAuthorPagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportAuthorPagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetAuthorPages(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPages(Query query)
        {
            return await GetAuthorPages(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPages(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AuthorPages");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAuthorPages(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>>(response);
        }

        partial void OnCreateAuthorPage(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> CreateAuthorPage(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage = default(ManekiApp.Server.Models.ManekiAppDB.AuthorPage))
        {
            var uri = new Uri(baseUri, $"AuthorPages");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(authorPage), Encoding.UTF8, "application/json");

            OnCreateAuthorPage(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>(response);
        }

        partial void OnDeleteAuthorPage(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteAuthorPage(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"AuthorPages({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAuthorPage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetAuthorPageById(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPageById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"AuthorPages({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAuthorPageById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>(response);
        }

        partial void OnUpdateAuthorPage(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateAuthorPage(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage = default(ManekiApp.Server.Models.ManekiAppDB.AuthorPage))
        {
            var uri = new Uri(baseUri, $"AuthorPages({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(authorPage), Encoding.UTF8, "application/json");

            OnUpdateAuthorPage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportImagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportImagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetImages(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Image>> GetImages(Query query)
        {
            return await GetImages(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Image>> GetImages(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Images");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetImages(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Image>>(response);
        }

        partial void OnCreateImage(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> CreateImage(ManekiApp.Server.Models.ManekiAppDB.Image image = default(ManekiApp.Server.Models.ManekiAppDB.Image))
        {
            var uri = new Uri(baseUri, $"Images");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(image), Encoding.UTF8, "application/json");

            OnCreateImage(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Image>(response);
        }

        partial void OnDeleteImage(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteImage(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Images({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteImage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetImageById(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> GetImageById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Images({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetImageById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Image>(response);
        }

        partial void OnUpdateImage(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateImage(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.Image image = default(ManekiApp.Server.Models.ManekiAppDB.Image))
        {
            var uri = new Uri(baseUri, $"Images({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(image), Encoding.UTF8, "application/json");

            OnUpdateImage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportPostsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportPostsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetPosts(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Post>> GetPosts(Query query)
        {
            return await GetPosts(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Post>> GetPosts(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Posts");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPosts(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Post>>(response);
        }

        partial void OnCreatePost(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> CreatePost(ManekiApp.Server.Models.ManekiAppDB.Post post = default(ManekiApp.Server.Models.ManekiAppDB.Post))
        {
            var uri = new Uri(baseUri, $"Posts");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(post), Encoding.UTF8, "application/json");

            OnCreatePost(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Post>(response);
        }

        partial void OnDeletePost(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeletePost(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Posts({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeletePost(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetPostById(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> GetPostById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Posts({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPostById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Post>(response);
        }

        partial void OnUpdatePost(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdatePost(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.Post post = default(ManekiApp.Server.Models.ManekiAppDB.Post))
        {
            var uri = new Uri(baseUri, $"Posts({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(post), Encoding.UTF8, "application/json");

            OnUpdatePost(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetSubscriptions(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Subscription>> GetSubscriptions(Query query)
        {
            return await GetSubscriptions(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Subscription>> GetSubscriptions(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Subscriptions");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSubscriptions(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Subscription>>(response);
        }

        partial void OnCreateSubscription(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> CreateSubscription(ManekiApp.Server.Models.ManekiAppDB.Subscription subscription = default(ManekiApp.Server.Models.ManekiAppDB.Subscription))
        {
            var uri = new Uri(baseUri, $"Subscriptions");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(subscription), Encoding.UTF8, "application/json");

            OnCreateSubscription(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Subscription>(response);
        }

        partial void OnDeleteSubscription(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteSubscription(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Subscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetSubscriptionById(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscriptionById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Subscriptions({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSubscriptionById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Subscription>(response);
        }

        partial void OnUpdateSubscription(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateSubscription(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.Subscription subscription = default(ManekiApp.Server.Models.ManekiAppDB.Subscription))
        {
            var uri = new Uri(baseUri, $"Subscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(subscription), Encoding.UTF8, "application/json");

            OnUpdateSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportUserSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportUserSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetUserSubscriptions(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>> GetUserSubscriptions(Query query)
        {
            return await GetUserSubscriptions(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>> GetUserSubscriptions(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserSubscriptions(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>>(response);
        }

        partial void OnCreateUserSubscription(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> CreateUserSubscription(ManekiApp.Server.Models.ManekiAppDB.UserSubscription userSubscription = default(ManekiApp.Server.Models.ManekiAppDB.UserSubscription))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userSubscription), Encoding.UTF8, "application/json");

            OnCreateUserSubscription(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>(response);
        }

        partial void OnDeleteUserSubscription(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteUserSubscription(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetUserSubscriptionById(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscriptionById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserSubscriptionById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>(response);
        }

        partial void OnUpdateUserSubscription(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateUserSubscription(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.UserSubscription userSubscription = default(ManekiApp.Server.Models.ManekiAppDB.UserSubscription))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userSubscription), Encoding.UTF8, "application/json");

            OnUpdateUserSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        public async System.Threading.Tasks.Task ExportUserVerificationCodesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async System.Threading.Tasks.Task ExportUserVerificationCodesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGetUserVerificationCodes(HttpRequestMessage requestMessage);

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>> GetUserVerificationCodes(Query query)
        {
            return await GetUserVerificationCodes(filter:$"{query.Filter}", orderby:$"{query.OrderBy}", top:query.Top, skip:query.Skip, count:query.Top != null && query.Skip != null);
        }

        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>> GetUserVerificationCodes(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:filter, top:top, skip:skip, orderby:orderby, expand:expand, select:select, count:count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserVerificationCodes(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>>(response);
        }

        partial void OnCreateUserVerificationCode(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> CreateUserVerificationCode(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode userVerificationCode = default(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userVerificationCode), Encoding.UTF8, "application/json");

            OnCreateUserVerificationCode(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>(response);
        }

        partial void OnDeleteUserVerificationCode(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteUserVerificationCode(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserVerificationCode(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetUserVerificationCodeById(HttpRequestMessage requestMessage);

        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCodeById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter:null, top:null, skip:null, orderby:null, expand:expand, select:null, count:null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserVerificationCodeById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>(response);
        }

        partial void OnUpdateUserVerificationCode(HttpRequestMessage requestMessage);
        
        public async Task<HttpResponseMessage> UpdateUserVerificationCode(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode userVerificationCode = default(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userVerificationCode), Encoding.UTF8, "application/json");

            OnUpdateUserVerificationCode(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}
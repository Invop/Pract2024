using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Text;
using System.Text.Encodings.Web;

namespace ManekiApp.Client
{
    /// <summary>
    /// Class ManekiAppDBService.
    /// </summary>
    public partial class ManekiAppDBService
    {
        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// The base URI
        /// </summary>
        private readonly Uri baseUri;
        /// <summary>
        /// The navigation manager
        /// </summary>
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManekiAppDBService"/> class.
        /// </summary>
        /// <param name="navigationManager">The navigation manager.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="configuration">The configuration.</param>
        public ManekiAppDBService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/ManekiAppDB/");
        }


        /// <summary>
        /// Exports the author pages to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportAuthorPagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the author pages to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportAuthorPagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/authorpages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get author pages].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetAuthorPages(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the author pages.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPages(Query query)
        {
            return await GetAuthorPages(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the author pages.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPages(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"AuthorPages");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAuthorPages(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>>(response);
        }

        /// <summary>
        /// Called when [create author page].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateAuthorPage(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the author page.
        /// </summary>
        /// <param name="authorPage">The author page.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.AuthorPage.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> CreateAuthorPage(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage = default(ManekiApp.Server.Models.ManekiAppDB.AuthorPage))
        {
            var uri = new Uri(baseUri, $"AuthorPages");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(authorPage), Encoding.UTF8, "application/json");

            OnCreateAuthorPage(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>(response);
        }

        /// <summary>
        /// Called when [delete author page].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteAuthorPage(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the author page.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteAuthorPage(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"AuthorPages({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteAuthorPage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get author page by identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetAuthorPageById(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the author page by identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.AuthorPage.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPageById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"AuthorPages({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetAuthorPageById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>(response);
        }

        /// <summary>
        /// Called when [update author page].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateAuthorPage(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the author page.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="authorPage">The author page.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateAuthorPage(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage = default(ManekiApp.Server.Models.ManekiAppDB.AuthorPage))
        {
            var uri = new Uri(baseUri, $"AuthorPages({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(authorPage), Encoding.UTF8, "application/json");

            OnUpdateAuthorPage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the images to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportImagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the images to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportImagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/images/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get images].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetImages(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.Image&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Image>> GetImages(Query query)
        {
            return await GetImages(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.Image&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Image>> GetImages(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Images");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetImages(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Image>>(response);
        }

        /// <summary>
        /// Called when [create image].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateImage(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Image.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> CreateImage(ManekiApp.Server.Models.ManekiAppDB.Image image = default(ManekiApp.Server.Models.ManekiAppDB.Image))
        {
            var uri = new Uri(baseUri, $"Images");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(image), Encoding.UTF8, "application/json");

            OnCreateImage(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Image>(response);
        }

        /// <summary>
        /// Called when [delete image].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteImage(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteImage(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Images({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteImage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get image by identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetImageById(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the image by identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Image.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.Image> GetImageById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Images({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetImageById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Image>(response);
        }

        /// <summary>
        /// Called when [update image].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateImage(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="image">The image.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateImage(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.Image image = default(ManekiApp.Server.Models.ManekiAppDB.Image))
        {
            var uri = new Uri(baseUri, $"Images({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(image), Encoding.UTF8, "application/json");

            OnUpdateImage(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the posts to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportPostsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the posts to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportPostsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get posts].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetPosts(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the posts.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.Post&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Post>> GetPosts(Query query)
        {
            return await GetPosts(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the posts.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.Post&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Post>> GetPosts(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Posts");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPosts(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Post>>(response);
        }

        /// <summary>
        /// Called when [create post].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreatePost(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the post.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Post.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> CreatePost(ManekiApp.Server.Models.ManekiAppDB.Post post = default(ManekiApp.Server.Models.ManekiAppDB.Post))
        {
            var uri = new Uri(baseUri, $"Posts");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(post), Encoding.UTF8, "application/json");

            OnCreatePost(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Post>(response);
        }

        /// <summary>
        /// Called when [delete post].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeletePost(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the post.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeletePost(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Posts({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeletePost(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get post by identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetPostById(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Post.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.Post> GetPostById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Posts({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetPostById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Post>(response);
        }

        /// <summary>
        /// Called when [update post].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdatePost(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the post.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="post">The post.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdatePost(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.Post post = default(ManekiApp.Server.Models.ManekiAppDB.Post))
        {
            var uri = new Uri(baseUri, $"Posts({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(post), Encoding.UTF8, "application/json");

            OnUpdatePost(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the subscriptions to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the subscriptions to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/subscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get subscriptions].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetSubscriptions(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.Subscription&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Subscription>> GetSubscriptions(Query query)
        {
            return await GetSubscriptions(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.Subscription&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Subscription>> GetSubscriptions(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"Subscriptions");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSubscriptions(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.Subscription>>(response);
        }

        /// <summary>
        /// Called when [create subscription].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateSubscription(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the subscription.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Subscription.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> CreateSubscription(ManekiApp.Server.Models.ManekiAppDB.Subscription subscription = default(ManekiApp.Server.Models.ManekiAppDB.Subscription))
        {
            var uri = new Uri(baseUri, $"Subscriptions");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(subscription), Encoding.UTF8, "application/json");

            OnCreateSubscription(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Subscription>(response);
        }

        /// <summary>
        /// Called when [delete subscription].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteSubscription(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the subscription.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteSubscription(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Subscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get subscription by identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetSubscriptionById(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the subscription by identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.Subscription.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscriptionById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"Subscriptions({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSubscriptionById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.Subscription>(response);
        }

        /// <summary>
        /// Called when [update subscription].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateSubscription(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the subscription.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="subscription">The subscription.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateSubscription(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.Subscription subscription = default(ManekiApp.Server.Models.ManekiAppDB.Subscription))
        {
            var uri = new Uri(baseUri, $"Subscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(subscription), Encoding.UTF8, "application/json");

            OnUpdateSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the user subscriptions to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserSubscriptionsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user subscriptions to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserSubscriptionsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/usersubscriptions/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get user subscriptions].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserSubscriptions(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user subscriptions.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserSubscription&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>> GetUserSubscriptions(Query query)
        {
            return await GetUserSubscriptions(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the user subscriptions.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserSubscription&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>> GetUserSubscriptions(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserSubscriptions(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>>(response);
        }

        /// <summary>
        /// Called when [create user subscription].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateUserSubscription(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the user subscription.
        /// </summary>
        /// <param name="userSubscription">The user subscription.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserSubscription.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> CreateUserSubscription(ManekiApp.Server.Models.ManekiAppDB.UserSubscription userSubscription = default(ManekiApp.Server.Models.ManekiAppDB.UserSubscription))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userSubscription), Encoding.UTF8, "application/json");

            OnCreateUserSubscription(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>(response);
        }

        /// <summary>
        /// Called when [delete user subscription].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteUserSubscription(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the user subscription.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteUserSubscription(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get user subscription by identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserSubscriptionById(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user subscription by identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserSubscription.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscriptionById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserSubscriptionById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>(response);
        }

        /// <summary>
        /// Called when [update user subscription].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateUserSubscription(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the user subscription.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userSubscription">The user subscription.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateUserSubscription(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.UserSubscription userSubscription = default(ManekiApp.Server.Models.ManekiAppDB.UserSubscription))
        {
            var uri = new Uri(baseUri, $"UserSubscriptions({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userSubscription), Encoding.UTF8, "application/json");

            OnUpdateUserSubscription(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the user verification codes to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserVerificationCodesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user verification codes to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserVerificationCodesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get user verification codes].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserVerificationCodes(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user verification codes.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>> GetUserVerificationCodes(Query query)
        {
            return await GetUserVerificationCodes(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the user verification codes.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>> GetUserVerificationCodes(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserVerificationCodes(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>>(response);
        }

        /// <summary>
        /// Called when [create user verification code].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateUserVerificationCode(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the user verification code.
        /// </summary>
        /// <param name="userVerificationCode">The user verification code.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> CreateUserVerificationCode(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode userVerificationCode = default(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userVerificationCode), Encoding.UTF8, "application/json");

            OnCreateUserVerificationCode(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>(response);
        }

        /// <summary>
        /// Called when [delete user verification code].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteUserVerificationCode(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the user verification code.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteUserVerificationCode(Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserVerificationCode(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get user verification code by identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserVerificationCodeById(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user verification code by identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCodeById(string expand = default(string), Guid id = default(Guid))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserVerificationCodeById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>(response);
        }

        /// <summary>
        /// Called when [update user verification code].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateUserVerificationCode(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the user verification code.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userVerificationCode">The user verification code.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateUserVerificationCode(Guid id = default(Guid), ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode userVerificationCode = default(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode))
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userVerificationCode), Encoding.UTF8, "application/json");

            OnUpdateUserVerificationCode(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the user chat payments to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserChatPaymentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatpayments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatpayments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user chat payments to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserChatPaymentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatpayments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatpayments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get user chat payments].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserChatPayments(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user chat payments.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatPayment&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>> GetUserChatPayments(Query query)
        {
            return await GetUserChatPayments(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the user chat payments.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatPayment&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>> GetUserChatPayments(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"UserChatPayments");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserChatPayments(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>>(response);
        }

        /// <summary>
        /// Called when [create user chat payment].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateUserChatPayment(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the user chat payment.
        /// </summary>
        /// <param name="userChatPayment">The user chat payment.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatPayment.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> CreateUserChatPayment(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment userChatPayment = default(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment))
        {
            var uri = new Uri(baseUri, $"UserChatPayments");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userChatPayment), Encoding.UTF8, "application/json");

            OnCreateUserChatPayment(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>(response);
        }

        /// <summary>
        /// Called when [delete user chat payment].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteUserChatPayment(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the user chat payment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteUserChatPayment(string userId = default(string))
        {
            var uri = new Uri(baseUri, $"UserChatPayments('{Uri.EscapeDataString(userId.Trim().Replace("'", "''"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserChatPayment(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get user chat payment by user identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserChatPaymentByUserId(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user chat payment by user identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatPayment.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> GetUserChatPaymentByUserId(string expand = default(string), string userId = default(string))
        {
            var uri = new Uri(baseUri, $"UserChatPayments('{Uri.EscapeDataString(userId.Trim().Replace("'", "''"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserChatPaymentByUserId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>(response);
        }

        /// <summary>
        /// Called when [update user chat payment].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateUserChatPayment(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the user chat payment.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userChatPayment">The user chat payment.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateUserChatPayment(string userId = default(string), ManekiApp.Server.Models.ManekiAppDB.UserChatPayment userChatPayment = default(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment))
        {
            var uri = new Uri(baseUri, $"UserChatPayments('{Uri.EscapeDataString(userId.Trim().Replace("'", "''"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userChatPayment), Encoding.UTF8, "application/json");

            OnUpdateUserChatPayment(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Exports the user chat notifications to excel.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserChatNotificationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatnotifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatnotifications/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Exports the user chat notifications to CSV.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="fileName">Name of the file.</param>
        public async System.Threading.Tasks.Task ExportUserChatNotificationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/manekiappdb/userchatnotifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/manekiappdb/userchatnotifications/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        /// <summary>
        /// Called when [get user chat notifications].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserChatNotifications(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user chat notifications.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatNotification&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>> GetUserChatNotifications(Query query)
        {
            return await GetUserChatNotifications(filter: $"{query.Filter}", orderby: $"{query.OrderBy}", top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        /// <summary>
        /// Gets the user chat notifications.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="expand">The expand.</param>
        /// <param name="top">The top.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="count">if set to <c>true</c> [count].</param>
        /// <param name="format">The format.</param>
        /// <param name="select">The select.</param>
        /// <returns>Radzen.ODataServiceResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatNotification&gt;.</returns>
        public async Task<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>> GetUserChatNotifications(string filter = default(string), string orderby = default(string), string expand = default(string), int? top = default(int?), int? skip = default(int?), bool? count = default(bool?), string format = default(string), string select = default(string))
        {
            var uri = new Uri(baseUri, $"UserChatNotifications");
            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserChatNotifications(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<Radzen.ODataServiceResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>>(response);
        }

        /// <summary>
        /// Called when [create user chat notification].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnCreateUserChatNotification(HttpRequestMessage requestMessage);

        /// <summary>
        /// Creates the user chat notification.
        /// </summary>
        /// <param name="userChatNotification">The user chat notification.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatNotification.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> CreateUserChatNotification(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification userChatNotification = default(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification))
        {
            var uri = new Uri(baseUri, $"UserChatNotifications");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userChatNotification), Encoding.UTF8, "application/json");

            OnCreateUserChatNotification(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>(response);
        }

        /// <summary>
        /// Called when [delete user chat notification].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnDeleteUserChatNotification(HttpRequestMessage requestMessage);

        /// <summary>
        /// Deletes the user chat notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteUserChatNotification(string userId = default(string))
        {
            var uri = new Uri(baseUri, $"UserChatNotifications('{Uri.EscapeDataString(userId.Trim().Replace("'", "''"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserChatNotification(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Called when [get user chat notification by user identifier].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnGetUserChatNotificationByUserId(HttpRequestMessage requestMessage);

        /// <summary>
        /// Gets the user chat notification by user identifier.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ManekiApp.Server.Models.ManekiAppDB.UserChatNotification.</returns>
        public async Task<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> GetUserChatNotificationByUserId(string expand = default(string), string userId = default(string))
        {
            var uri = new Uri(baseUri, $"UserChatNotifications('{Uri.EscapeDataString(userId.Trim().Replace("'", "''"))}')");

            uri = Radzen.ODataExtensions.GetODataUri(uri: uri, filter: null, top: null, skip: null, orderby: null, expand: expand, select: null, count: null);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserChatNotificationByUserId(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await Radzen.HttpResponseMessageExtensions.ReadAsync<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>(response);
        }

        /// <summary>
        /// Called when [update user chat notification].
        /// </summary>
        /// <param name="requestMessage">The request message.</param>
        partial void OnUpdateUserChatNotification(HttpRequestMessage requestMessage);

        /// <summary>
        /// Updates the user chat notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userChatNotification">The user chat notification.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> UpdateUserChatNotification(string userId = default(string), ManekiApp.Server.Models.ManekiAppDB.UserChatNotification userChatNotification = default(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification))
        {
            var uri = new Uri(baseUri, $"UserChatNotifications('{Uri.EscapeDataString(userId.Trim().Replace("'", "''"))}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(Radzen.ODataJsonSerializer.Serialize(userChatNotification), Encoding.UTF8, "application/json");

            OnUpdateUserChatNotification(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        /// <summary>
        /// Gets the user verification code by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>UserVerificationCode.</returns>
        public async Task<UserVerificationCode> GetUserVerificationCodeByUserId(
            string userId)
        {
            var filterQuery = $"UserId eq '{userId}'";
            var result = await GetUserVerificationCodes(filterQuery, count: true);
            return result.Value.FirstOrDefault(item => item.ExpiryTime >= DateTime.UtcNow);
        }
    }
}
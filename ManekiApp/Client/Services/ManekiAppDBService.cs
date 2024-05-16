using System.Text;
using System.Text.Encodings.Web;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ManekiApp.Client
{
    public partial class ManekiAppDBService
    {
        private readonly HttpClient httpClient;
        private readonly Uri baseUri;
        private readonly NavigationManager navigationManager;

        public ManekiAppDBService(NavigationManager navigationManager, HttpClient httpClient,
            IConfiguration configuration)
        {
            this.httpClient = httpClient;

            this.navigationManager = navigationManager;
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/ManekiAppDB/");
        }


        public async Task ExportUserVerificationCodesToExcel(Query query = null,
            string fileName = null)
        {
            navigationManager.NavigateTo(
                query != null
                    ? query.ToUrl(
                        $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')")
                    : $"export/manekiappdb/userverificationcodes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')",
                true);
        }

        public async Task ExportUserVerificationCodesToCSV(Query query = null,
            string fileName = null)
        {
            navigationManager.NavigateTo(
                query != null
                    ? query.ToUrl(
                        $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')")
                    : $"export/manekiappdb/userverificationcodes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')",
                true);
        }

        partial void OnGetUserVerificationCodes(HttpRequestMessage requestMessage);

        public async Task<ODataServiceResult<UserVerificationCode>>
            GetUserVerificationCodes(Query query)
        {
            return await GetUserVerificationCodes($"{query.Filter}", $"{query.OrderBy}",
                top: query.Top, skip: query.Skip, count: query.Top != null && query.Skip != null);
        }

        public async Task<ODataServiceResult<UserVerificationCode>>
            GetUserVerificationCodes(string filter = default, string orderby = default,
                string expand = default, int? top = default, int? skip = default,
                bool? count = default, string format = default, string select = default)
        {
            var uri = new Uri(baseUri, "UserVerificationCodes");
            uri = uri.GetODataUri(filter, top, skip, orderby,
                expand, select, count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserVerificationCodes(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response
                .ReadAsync<ODataServiceResult<UserVerificationCode>>();
        }

        partial void OnCreateUserVerificationCode(HttpRequestMessage requestMessage);

        public async Task<UserVerificationCode> CreateUserVerificationCode(
            UserVerificationCode userVerificationCode =
                default)
        {
            var uri = new Uri(baseUri, "UserVerificationCodes");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            httpRequestMessage.Content = new StringContent(ODataJsonSerializer.Serialize(userVerificationCode),
                Encoding.UTF8, "application/json");

            OnCreateUserVerificationCode(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response
                .ReadAsync<UserVerificationCode>();
        }

        partial void OnDeleteUserVerificationCode(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> DeleteUserVerificationCode(Guid id = default)
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

            OnDeleteUserVerificationCode(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }

        partial void OnGetUserVerificationCodeById(HttpRequestMessage requestMessage);

        public async Task<UserVerificationCode> GetUserVerificationCodeById(
            string expand = default, Guid id = default)
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            uri = uri.GetODataUri(null, null, null, null,
                expand);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetUserVerificationCodeById(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response
                .ReadAsync<UserVerificationCode>();
        }


        public async Task<UserVerificationCode> GetUserVerificationCodeByUserId(
            string userId)
        {
            var uri = new Uri(baseUri, $"UserVerificationCodesByUserId(UserId={userId})");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response
                .ReadAsync<UserVerificationCode>();
        }


        partial void OnUpdateUserVerificationCode(HttpRequestMessage requestMessage);

        public async Task<HttpResponseMessage> UpdateUserVerificationCode(Guid id = default,
            UserVerificationCode userVerificationCode =
                default)
        {
            var uri = new Uri(baseUri, $"UserVerificationCodes({id})");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri);


            httpRequestMessage.Content = new StringContent(ODataJsonSerializer.Serialize(userVerificationCode),
                Encoding.UTF8, "application/json");

            OnUpdateUserVerificationCode(httpRequestMessage);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}
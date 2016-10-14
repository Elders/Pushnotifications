using System;
using System.Threading.Tasks;
using PushNotifications.Api.Client.Logging;
using RestSharp;

namespace PushNotifications.Api.Client
{
    public static class RestClientExtensions
    {
        static ILog log = LogProvider.GetLogger(typeof(RestClientExtensions));

        public static Task<IRestResponse> ExecuteAsync(this IRestClient client, IRestRequest request)
        {
            try
            {
                string requestLog = $"{client.BaseUrl}{request.Resource} - {Enum.GetName(typeof(Method), request.Method)}";

                var tcs = new TaskCompletionSource<IRestResponse>();

                client.ExecuteAsync(request, (response) =>
                {
                    try
                    {
                        if (response.ErrorException == null)
                        {
                            tcs.TrySetResult(response);
                            if (response.HasClientError() && log.IsWarnEnabled())
                                log.WarnException($"{requestLog} => {response.StatusCode}", new Exception(response.Content));
                            else
                                log.Debug(() => $"{requestLog} => {response.StatusCode}");
                        }
                        else
                        {
                            log.ErrorException(requestLog, response.ErrorException);
                            tcs.TrySetException(response.ErrorException);
                        }
                    }
                    catch (Exception lambdaEx)
                    {
                        log.ErrorException("RestSharp lambda exception", lambdaEx);
                        tcs.TrySetException(lambdaEx);
                    }
                });

                return tcs.Task;
            }
            catch (Exception ex)
            {
                log.FatalException(ex.Message, ex);
                return null;
            }
        }

        public static Task<IRestResponse<T>> ExecuteAsync<T>(this IRestClient client, IRestRequest request)
             where T : new()
        {
            try
            {
                string requestLog = $"{client.BaseUrl}{request.Resource} - {Enum.GetName(typeof(Method), request.Method)}";

                var tcs = new TaskCompletionSource<IRestResponse<T>>();

                client.ExecuteAsync<T>(request, response =>
                {
                    try
                    {
                        if (response.ErrorException == null)
                        {
                            tcs.SetResult(response);
                            if (response.HasClientError() && log.IsWarnEnabled())
                                log.WarnException($"{requestLog} => {response.StatusCode}", new Exception(response.Content));
                            else
                                log.Debug(() => $"{requestLog} => {response.StatusCode}");
                        }
                        else
                        {
                            log.ErrorException(requestLog, response.ErrorException);
                            tcs.SetException(response.ErrorException);
                        }
                    }
                    catch (Exception lambdaEx)
                    {
                        log.ErrorException("RestSharp lambda exception", lambdaEx);
                        tcs.SetException(lambdaEx);
                    }
                });

                return tcs.Task;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Determines whether the server response status code indicates a client side error (status code >= 400 and &lt; 500).
        /// </summary>
        /// <param name="this">The <see cref="IRestResponse"/> to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the response status code is >= 400 and &lt; 500; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasClientError(this IRestResponse @this)
        {
            var code = (int)@this.StatusCode;

            return (code >= 400 && code < 500) || code == 0;
        }

        /// <summary>
        /// Determines whether the server response status code indicates a server side error (status code >= 500 and &lt; 600).
        /// </summary>
        /// <param name="this">The <see cref="IRestResponse"/> to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the response status code is >= 500 and &lt; 600; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasServerError(this IRestResponse @this)
        {
            var code = (int)@this.StatusCode;

            return code >= 500 && code < 600;
        }

        public static IRestRequest AddAuthorizationBearerHeader(this IRestRequest request, Authenticator auth)
        {
            return request.AddHeader("Authorization", "Bearer " + auth.AccessToken);
        }
    }
}

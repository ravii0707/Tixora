using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;
using System.Threading;


namespace Tixora.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private static int _totalRequests = 0;
        private static int _failedRequests = 0;
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            Interlocked.Increment(ref _totalRequests);

            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;
            var response = context.Response;
            var requestTime = DateTime.UtcNow;

            try
            {
                await _next(context);
                stopwatch.Stop();

                bool isSuccess = response.StatusCode >= 200 && response.StatusCode < 300;
                if (!isSuccess)
                    Interlocked.Increment(ref _failedRequests);

                Log.Information("Request: {method} {url} | Status: {statusCode} | Success: {success} | Timestamp: {timestamp} | Duration: {duration}ms",
                    request.Method,
                    request.Path,
                    response.StatusCode,
                    isSuccess,
                    //response.StatusCode >= 200 && response.StatusCode < 300,
                    requestTime.ToString("o"),
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Interlocked.Increment(ref _failedRequests); // exceptions count as failures

                Log.Error(ex, "Request: {method} {url} | Status: 500 | Success: false | Timestamp: {timestamp} | Error: {error} | Duration: {duration}ms",
                    request.Method,
                    request.Path,
                    requestTime.ToString("o"),
                    ex.Message,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
        // Static method to get overall API success stats
        public static string GetSuccessRate()
        {
            var total = _totalRequests;
            var failed = _failedRequests;
            var success = total - failed;
            double rate = total > 0 ? (success * 100.0) / total : 0;

            return $"Total Requests: {total}, Success: {success}, Failed: {failed}, Success Rate: {rate:F2}%";
        }
    }
}
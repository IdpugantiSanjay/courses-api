using Microsoft.Extensions.Logging;

namespace Courses.CLI;

public class HttpClientLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpClientLoggingHandler> _logger;

    public HttpClientLoggingHandler(ILogger<HttpClientLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        _logger.LogInformation("Request completed: {Route} {Method} {Code} {Headers}", request.RequestUri!.ToString(), request.Method, response.StatusCode,
            request.Headers);
        return response;
    }
}
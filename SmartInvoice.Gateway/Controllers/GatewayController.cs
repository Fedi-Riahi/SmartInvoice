using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dictionary<string, ServiceInfo> _services;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(IHttpClientFactory httpClientFactory, ILogger<GatewayController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            // Define all services with their ports and metadata
            _services = new Dictionary<string, ServiceInfo>
            {
                {
                    "auth",
                    new ServiceInfo
                    {
                        Name = "Authentication",
                        Url = "http://localhost:5001",
                        Description = "User authentication and authorization",
                        Version = "1.0.0",
                        Endpoints = new List<string> { "login", "register", "validate" }
                    }
                },
                {
                    "invoice",
                    new ServiceInfo
                    {
                        Name = "Invoice Management",
                        Url = "http://localhost:5002",
                        Description = "Invoice creation, management, and tracking",
                        Version = "1.0.0",
                        Endpoints = new List<string> { "create", "get", "update", "delete", "list" }
                    }
                },
                {
                    "customer",
                    new ServiceInfo
                    {
                        Name = "Customer Management",
                        Url = "http://localhost:5003",
                        Description = "Customer information and relationship management",
                        Version = "1.0.0",
                        Endpoints = new List<string> { "create", "get", "update", "delete", "invoices" }
                    }
                },
                {
                    "ai",
                    new ServiceInfo
                    {
                        Name = "AI & Analytics",
                        Url = "http://localhost:5004",
                        Description = "AI-powered predictions and analytics",
                        Version = "1.0.0",
                        Endpoints = new List<string> { "predict", "analyze", "generate", "trends" }
                    }
                }
            };

            _logger.LogInformation("Gateway initialized with {ServiceCount} services", _services.Count);
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            _logger.LogInformation("Gateway health check requested");

            var response = new GatewayHealthResponse
            {
                Service = "SmartInvoice Gateway",
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Uptime = TimeSpan.FromMinutes(10), // This would be calculated in production
                Services = _services.Select(s => new ServiceStatus
                {
                    Name = s.Value.Name,
                    ServiceKey = s.Key,
                    Url = s.Value.Url,
                    Status = "Registered",
                    HealthEndpoint = $"{s.Value.Url}/api/{s.Key}/health",
                    Description = s.Value.Description
                }).ToList(),
                SystemInfo = new SystemInfo
                {
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    MachineName = Environment.MachineName,
                    OsVersion = Environment.OSVersion.ToString(),
                    DotNetVersion = Environment.Version.ToString()
                }
            };

            return Ok(new ApiResponse<GatewayHealthResponse>
            {
                Success = true,
                Message = "Gateway is healthy and running",
                Data = response,
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("services")]
        public IActionResult GetAllServices()
        {
            _logger.LogInformation("Get all services requested");

            return Ok(new ApiResponse<List<ServiceInfoResponse>>
            {
                Success = true,
                Message = "List of all registered services",
                Data = _services.Select(s => new ServiceInfoResponse
                {
                    ServiceKey = s.Key,
                    Name = s.Value.Name,
                    Url = s.Value.Url,
                    Description = s.Value.Description,
                    Version = s.Value.Version,
                    Status = "Registered",
                    Endpoints = s.Value.Endpoints,
                    Documentation = $"{s.Value.Url}/swagger"
                }).ToList()
            });
        }

        [HttpGet("services/{serviceName}/health")]
        public async Task<IActionResult> GetServiceHealth(string serviceName)
        {
            if (!_services.ContainsKey(serviceName.ToLower()))
            {
                _logger.LogWarning("Service not found: {ServiceName}", serviceName);

                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Service '{serviceName}' not found",
                    Errors = new List<string> { $"Available services: {string.Join(", ", _services.Keys)}" }
                });
            }

            var service = _services[serviceName.ToLower()];
            var client = _httpClientFactory.CreateClient();

            _logger.LogInformation("Checking health of service: {ServiceName} at {Url}", service.Name, service.Url);

            try
            {
                // Set timeout for health check
                client.Timeout = TimeSpan.FromSeconds(5);

                var response = await client.GetAsync($"{service.Url}/api/{serviceName.ToLower()}/health");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Service {ServiceName} is healthy", service.Name);

                    return Ok(new ApiResponse<ServiceHealthResponse>
                    {
                        Success = true,
                        Message = $"Service '{service.Name}' is available",
                        Data = new ServiceHealthResponse
                        {
                            Service = service.Name,
                            Status = "Available",
                            StatusCode = (int)response.StatusCode,
                            ResponseTime = 0, // Would measure in production
                            Url = service.Url,
                            RoutedThrough = "Gateway",
                            RawResponse = content
                        }
                    });
                }
                else
                {
                    _logger.LogWarning("Service {ServiceName} returned status: {StatusCode}", service.Name, response.StatusCode);

                    return StatusCode((int)response.StatusCode, new ApiResponse<object>
                    {
                        Success = false,
                        Message = $"Service '{service.Name}' returned error status",
                        Data = new
                        {
                            Service = service.Name,
                            StatusCode = (int)response.StatusCode,
                            Status = response.StatusCode.ToString(),
                            Content = content
                        }
                    });
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("Service {ServiceName} health check timed out", service.Name);

                return StatusCode(503, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Service '{service.Name}' health check timed out",
                    Errors = new List<string> { "Service may be down or unresponsive" }
                });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Service {ServiceName} is unreachable", service.Name);

                return StatusCode(503, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Service '{service.Name}' is unreachable",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking health of service {ServiceName}", service.Name);

                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error checking health of service '{service.Name}'",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("services/{serviceName}/endpoints")]
        public IActionResult GetServiceEndpoints(string serviceName)
        {
            if (!_services.ContainsKey(serviceName.ToLower()))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Service '{serviceName}' not found"
                });
            }

            var endpoints = serviceName.ToLower() switch
            {
                "auth" => new[]
                {
                    new ServiceEndpoint { Method = "GET", Path = "/api/auth/health", Description = "Health check" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/auth/login", Description = "User login" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/auth/register", Description = "User registration" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/auth/validate", Description = "Token validation" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/auth/refresh", Description = "Token refresh" }
                },
                "invoice" => new[]
                {
                    new ServiceEndpoint { Method = "GET", Path = "/api/invoice/health", Description = "Health check" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/invoice", Description = "Get all invoices" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/invoice/{id}", Description = "Get invoice by ID" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/invoice", Description = "Create invoice" },
                    new ServiceEndpoint { Method = "PUT", Path = "/api/invoice/{id}", Description = "Update invoice" },
                    new ServiceEndpoint { Method = "DELETE", Path = "/api/invoice/{id}", Description = "Delete invoice" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/invoice/customer/{customerId}", Description = "Get invoices by customer" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/invoice/status/{status}", Description = "Get invoices by status" }
                },
                "customer" => new[]
                {
                    new ServiceEndpoint { Method = "GET", Path = "/api/customer/health", Description = "Health check" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/customer", Description = "Get all customers" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/customer/{id}", Description = "Get customer by ID" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/customer", Description = "Create customer" },
                    new ServiceEndpoint { Method = "PUT", Path = "/api/customer/{id}", Description = "Update customer" },
                    new ServiceEndpoint { Method = "DELETE", Path = "/api/customer/{id}", Description = "Delete customer" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/customer/{id}/invoices", Description = "Get customer invoices" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/customer/search/{query}", Description = "Search customers" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/customer/stats", Description = "Customer statistics" }
                },
                "ai" => new[]
                {
                    new ServiceEndpoint { Method = "GET", Path = "/api/ai/health", Description = "Health check" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/ai/predict/sales", Description = "Predict sales" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/ai/generate/description", Description = "Generate product description" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/ai/analyze/customer", Description = "Analyze customer" },
                    new ServiceEndpoint { Method = "GET", Path = "/api/ai/trends/{period}", Description = "Get business trends" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/ai/recommend/products", Description = "Product recommendations" },
                    new ServiceEndpoint { Method = "POST", Path = "/api/ai/summarize/text", Description = "Text summarization" }
                },
                _ => Array.Empty<ServiceEndpoint>()
            };

            return Ok(new ApiResponse<ServiceEndpointsResponse>
            {
                Success = true,
                Message = $"Endpoints for service '{serviceName}'",
                Data = new ServiceEndpointsResponse
                {
                    Service = serviceName,
                    BaseUrl = _services[serviceName.ToLower()].Url,
                    Endpoints = endpoints,
                    Count = endpoints.Length,
                    Documentation = $"{_services[serviceName.ToLower()].Url}/swagger"
                }
            });
        }

        [HttpPost("route/{serviceName}/{*endpoint}")]
        public async Task<IActionResult> RouteRequest(
            string serviceName,
            [FromRoute] string endpoint,
            [FromBody] object? requestBody = null,
            [FromQuery] string? method = null)
        {
            _logger.LogInformation("Routing request to {ServiceName}/{Endpoint}", serviceName, endpoint);

            if (!_services.ContainsKey(serviceName.ToLower()))
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Service '{serviceName}' not found"
                });
            }

            var service = _services[serviceName.ToLower()];
            var client = _httpClientFactory.CreateClient();
            var url = $"{service.Url}/api/{endpoint}";

            // Preserve query string from original request
            if (HttpContext.Request.QueryString.HasValue)
            {
                url += HttpContext.Request.QueryString.Value;
            }

            // Determine HTTP method
            var httpMethod = method?.ToUpper() ?? HttpContext.Request.Method;

            try
            {
                HttpResponseMessage response;

                switch (httpMethod)
                {
                    case "GET":
                        response = await client.GetAsync(url);
                        break;
                    case "POST":
                        response = await client.PostAsJsonAsync(url, requestBody ?? new { });
                        break;
                    case "PUT":
                        response = await client.PutAsJsonAsync(url, requestBody ?? new { });
                        break;
                    case "DELETE":
                        response = await client.DeleteAsync(url);
                        break;
                    case "PATCH":
                        var patchContent = new StringContent(
                            System.Text.Json.JsonSerializer.Serialize(requestBody ?? new { }),
                            System.Text.Encoding.UTF8,
                            "application/json");
                        response = await client.PatchAsync(url, patchContent);
                        break;
                    default:
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = $"Unsupported HTTP method: {httpMethod}"
                        });
                }

                var content = await response.Content.ReadAsStringAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";

                _logger.LogInformation("Routed request completed with status: {StatusCode}", response.StatusCode);

                // Return the response from the service
                return new ContentResult
                {
                    Content = content,
                    ContentType = contentType,
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to route request to {ServiceName}", serviceName);

                return StatusCode(502, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to connect to service '{serviceName}'",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error routing request to {ServiceName}", serviceName);

                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error routing request to '{serviceName}'",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("batch/health")]
        public async Task<IActionResult> BatchHealthCheck([FromBody] List<string>? serviceNames = null)
        {
            var servicesToCheck = serviceNames?.Select(s => s.ToLower()).ToList() ?? _services.Keys.ToList();
            var results = new List<BatchHealthResult>();

            _logger.LogInformation("Batch health check for {Count} services", servicesToCheck.Count);

            foreach (var serviceKey in servicesToCheck)
            {
                if (_services.TryGetValue(serviceKey, out var service))
                {
                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(3);

                    var startTime = DateTime.UtcNow;

                    try
                    {
                        var response = await client.GetAsync($"{service.Url}/api/{serviceKey}/health");
                        var elapsed = DateTime.UtcNow - startTime;

                        results.Add(new BatchHealthResult
                        {
                            Service = service.Name,
                            ServiceKey = serviceKey,
                            Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                            StatusCode = (int)response.StatusCode,
                            ResponseTime = elapsed.TotalMilliseconds,
                            Url = service.Url,
                            Timestamp = DateTime.UtcNow
                        });
                    }
                    catch (Exception ex)
                    {
                        var elapsed = DateTime.UtcNow - startTime;

                        results.Add(new BatchHealthResult
                        {
                            Service = service.Name,
                            ServiceKey = serviceKey,
                            Status = "Unreachable",
                            StatusCode = 0,
                            ResponseTime = elapsed.TotalMilliseconds,
                            Url = service.Url,
                            Error = ex.Message,
                            Timestamp = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    results.Add(new BatchHealthResult
                    {
                        Service = serviceKey,
                        ServiceKey = serviceKey,
                        Status = "Not Found",
                        StatusCode = 404,
                        ResponseTime = 0,
                        Error = "Service not registered in gateway",
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            var healthyCount = results.Count(r => r.Status == "Healthy");
            var unhealthyCount = results.Count(r => r.Status != "Healthy");

            return Ok(new ApiResponse<BatchHealthResponse>
            {
                Success = unhealthyCount == 0,
                Message = unhealthyCount == 0
                    ? "All services are healthy"
                    : $"{unhealthyCount} service(s) have issues",
                Data = new BatchHealthResponse
                {
                    Results = results,
                    Total = results.Count,
                    Healthy = healthyCount,
                    Unhealthy = unhealthyCount,
                    Timestamp = DateTime.UtcNow,
                    Summary = new HealthSummary
                    {
                        OverallStatus = unhealthyCount == 0 ? "Healthy" : "Degraded",
                        PercentageHealthy = results.Count > 0 ? (healthyCount * 100.0 / results.Count) : 100.0,
                        AverageResponseTime = results.Average(r => r.ResponseTime)
                    }
                }
            });
        }

        [HttpGet("stats")]
        public IActionResult GetGatewayStats()
        {
            return Ok(new ApiResponse<GatewayStats>
            {
                Success = true,
                Message = "Gateway statistics",
                Data = new GatewayStats
                {
                    ServiceCount = _services.Count,
                    Uptime = TimeSpan.FromHours(1), // Would be calculated in production
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    Timestamp = DateTime.UtcNow,
                    Services = _services.Select(s => new ServiceStats
                    {
                        Name = s.Value.Name,
                        Key = s.Key,
                        Url = s.Value.Url,
                        RegisteredAt = DateTime.UtcNow.AddHours(-1) // Would be tracked
                    }).ToList()
                }
            });
        }

        // Helper Classes
        private class ServiceInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public List<string> Endpoints { get; set; } = new();
        }

        // Response Classes
        public class GatewayHealthResponse
        {
            public string Service { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string Version { get; set; } = string.Empty;
            public TimeSpan Uptime { get; set; }
            public List<ServiceStatus> Services { get; set; } = new();
            public SystemInfo SystemInfo { get; set; } = new();
        }

        public class ServiceStatus
        {
            public string Name { get; set; } = string.Empty;
            public string ServiceKey { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string HealthEndpoint { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public class SystemInfo
        {
            public string Environment { get; set; } = string.Empty;
            public string MachineName { get; set; } = string.Empty;
            public string OsVersion { get; set; } = string.Empty;
            public string DotNetVersion { get; set; } = string.Empty;
        }

        public class ServiceInfoResponse
        {
            public string ServiceKey { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public List<string> Endpoints { get; set; } = new();
            public string Documentation { get; set; } = string.Empty;
        }

        public class ServiceHealthResponse
        {
            public string Service { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public int StatusCode { get; set; }
            public double ResponseTime { get; set; } // milliseconds
            public string Url { get; set; } = string.Empty;
            public string RoutedThrough { get; set; } = string.Empty;
            public string RawResponse { get; set; } = string.Empty;
        }

        public class ServiceEndpoint
        {
            public string Method { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public class ServiceEndpointsResponse
        {
            public string Service { get; set; } = string.Empty;
            public string BaseUrl { get; set; } = string.Empty;
            public ServiceEndpoint[] Endpoints { get; set; } = Array.Empty<ServiceEndpoint>();
            public int Count { get; set; }
            public string Documentation { get; set; } = string.Empty;
        }

        public class BatchHealthResult
        {
            public string Service { get; set; } = string.Empty;
            public string ServiceKey { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public int StatusCode { get; set; }
            public double ResponseTime { get; set; } // milliseconds
            public string Url { get; set; } = string.Empty;
            public string? Error { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public class BatchHealthResponse
        {
            public List<BatchHealthResult> Results { get; set; } = new();
            public int Total { get; set; }
            public int Healthy { get; set; }
            public int Unhealthy { get; set; }
            public DateTime Timestamp { get; set; }
            public HealthSummary Summary { get; set; } = new();
        }

        public class HealthSummary
        {
            public string OverallStatus { get; set; } = string.Empty;
            public double PercentageHealthy { get; set; }
            public double AverageResponseTime { get; set; }
        }

        public class GatewayStats
        {
            public int ServiceCount { get; set; }
            public TimeSpan Uptime { get; set; }
            public string Environment { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public List<ServiceStats> Services { get; set; } = new();
        }

        public class ServiceStats
        {
            public string Name { get; set; } = string.Empty;
            public string Key { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public DateTime RegisteredAt { get; set; }
        }
    }
}
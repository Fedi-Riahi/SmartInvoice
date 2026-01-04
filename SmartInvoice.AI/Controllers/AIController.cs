using Microsoft.AspNetCore.Mvc;
using SmartInvoice.Shared.DTOs;

namespace SmartInvoice.AI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new ApiResponse<HealthResponse>
            {
                Success = true,
                Message = "AI service is healthy",
                Data = new HealthResponse
                {
                    Service = "AI & Analytics Service",
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Details = new Dictionary<string, string>
                    {
                        { "Port", "5004" },
                        { "Version", "1.0.0" },
                        { "Features", "Predictions, Recommendations, Gemini AI" }
                    }
                }
            });
        }

        [HttpPost("predict/sales")]
        public IActionResult PredictSales([FromBody] SalesPredictionRequest request)
        {
            var random = new Random();
            var predictions = new List<SalesPrediction>();

            for (int i = 0; i < request.Months; i++)
            {
                var baseAmount = request.HistoricalAverage * (1 + (random.NextDouble() * 0.3 - 0.15));
                var predictedAmount = baseAmount * (1 + (request.GrowthRate / 100));

                predictions.Add(new SalesPrediction
                {
                    Month = DateTime.Now.AddMonths(i + 1).ToString("MMMM yyyy"),
                    PredictedAmount = Math.Round((decimal)predictedAmount, 2),
                    Confidence = Math.Round(random.NextDouble() * 20 + 70, 1), // 70-90% confidence
                    Factors = new List<string> { "Seasonal trends", "Market growth", "Customer expansion" }
                });
            }

            return Ok(new ApiResponse<SalesPredictionResponse>
            {
                Success = true,
                Message = "Sales predictions generated (mock - will integrate Gemini AI)",
                Data = new SalesPredictionResponse
                {
                    Predictions = predictions,
                    TotalPredictedAmount = predictions.Sum(p => p.PredictedAmount),
                    AverageMonthlyGrowth = request.GrowthRate,
                    GeneratedAt = DateTime.UtcNow
                }
            });
        }

        [HttpPost("generate/description")]
        public IActionResult GenerateDescription([FromBody] DescriptionRequest request)
        {
            var descriptions = new List<string>
            {
                $"Professional {request.ProductType} services delivered with excellence and attention to detail.",
                $"Premium {request.ProductType} solution designed for optimal performance and reliability.",
                $"Comprehensive {request.ProductType} package including setup, support, and maintenance.",
                $"Custom {request.ProductType} implementation tailored to your specific business needs."
            };

            var random = new Random();

            return Ok(new ApiResponse<DescriptionResponse>
            {
                Success = true,
                Message = "Descriptions generated (mock - will integrate Gemini AI)",
                Data = new DescriptionResponse
                {
                    ProductName = request.ProductName,
                    ProductType = request.ProductType,
                    GeneratedDescriptions = descriptions.OrderBy(x => random.Next()).Take(3).ToList(),
                    RecommendedDescription = descriptions[0],
                    GenerationMethod = "AI-Powered Description Generation"
                }
            });
        }

        [HttpPost("analyze/customer")]
        public IActionResult AnalyzeCustomer([FromBody] CustomerAnalysisRequest request)
        {
            var analysis = new CustomerAnalysis
            {
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                RiskScore = Math.Round(new Random().NextDouble() * 30 + 60, 1), // 60-90 score
                PotentialValue = request.HistoricalSpending * 1.5m,
                Recommendations = new List<string>
                {
                    "Offer premium support package",
                    "Suggest complementary products",
                    "Schedule quarterly business review",
                    "Provide early payment discount"
                },
                Insights = new List<string>
                {
                    $"Customer shows consistent spending pattern over {request.MonthsAsCustomer} months",
                    "Responds well to personalized offers",
                    "Prefers digital communication channels"
                }
            };

            return Ok(new ApiResponse<CustomerAnalysisResponse>
            {
                Success = true,
                Message = "Customer analysis generated (mock - will integrate Gemini AI)",
                Data = new CustomerAnalysisResponse
                {
                    Analysis = analysis,
                    GeneratedAt = DateTime.UtcNow,
                    ModelVersion = "1.0"
                }
            });
        }

        [HttpGet("trends/{period}")]
        public IActionResult GetTrends(string period = "monthly")
        {
            var trends = new BusinessTrends
            {
                Period = period,
                TopProducts = new List<ProductTrend>
                {
                    new ProductTrend { ProductName = "Web Development", Growth = 25.5, Revenue = 15000 },
                    new ProductTrend { ProductName = "Cloud Hosting", Growth = 18.2, Revenue = 12000 },
                    new ProductTrend { ProductName = "Consulting", Growth = 12.7, Revenue = 8500 }
                },
                CustomerSegments = new List<SegmentTrend>
                {
                    new SegmentTrend { Segment = "Enterprise", Growth = 15.3, Count = 45 },
                    new SegmentTrend { Segment = "SMB", Growth = 22.1, Count = 120 },
                    new SegmentTrend { Segment = "Startup", Growth = 8.5, Count = 75 }
                },
                Seasonality = new List<SeasonalTrend>
                {
                    new SeasonalTrend { Month = "January", Factor = 0.9 },
                    new SeasonalTrend { Month = "February", Factor = 0.85 },
                    new SeasonalTrend { Month = "March", Factor = 1.1 }
                }
            };

            return Ok(new ApiResponse<BusinessTrends>
            {
                Success = true,
                Message = $"Business trends for {period} period (mock)",
                Data = trends
            });
        }

        // DTO Classes
        public class SalesPredictionRequest
        {
            public int Months { get; set; } = 6;
            public decimal HistoricalAverage { get; set; } = 10000;
            public decimal GrowthRate { get; set; } = 5.0m; // percentage
            public List<string>? MarketFactors { get; set; }
        }

        public class SalesPredictionResponse
        {
            public List<SalesPrediction> Predictions { get; set; } = new();
            public decimal TotalPredictedAmount { get; set; }
            public decimal AverageMonthlyGrowth { get; set; }
            public DateTime GeneratedAt { get; set; }
        }

        public class SalesPrediction
        {
            public string Month { get; set; } = string.Empty;
            public decimal PredictedAmount { get; set; }
            public double Confidence { get; set; }
            public List<string> Factors { get; set; } = new();
        }

        public class DescriptionRequest
        {
            public string ProductName { get; set; } = string.Empty;
            public string ProductType { get; set; } = string.Empty;
            public List<string>? Keywords { get; set; }
            public string? Tone { get; set; } = "professional";
        }

        public class DescriptionResponse
        {
            public string ProductName { get; set; } = string.Empty;
            public string ProductType { get; set; } = string.Empty;
            public List<string> GeneratedDescriptions { get; set; } = new();
            public string RecommendedDescription { get; set; } = string.Empty;
            public string GenerationMethod { get; set; } = string.Empty;
        }

        public class CustomerAnalysisRequest
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public decimal HistoricalSpending { get; set; }
            public int MonthsAsCustomer { get; set; }
            public List<string>? PurchaseHistory { get; set; }
        }

        public class CustomerAnalysisResponse
        {
            public CustomerAnalysis Analysis { get; set; } = new();
            public DateTime GeneratedAt { get; set; }
            public string ModelVersion { get; set; } = string.Empty;
        }

        public class CustomerAnalysis
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public double RiskScore { get; set; }
            public decimal PotentialValue { get; set; }
            public List<string> Recommendations { get; set; } = new();
            public List<string> Insights { get; set; } = new();
        }

        public class BusinessTrends
        {
            public string Period { get; set; } = string.Empty;
            public List<ProductTrend> TopProducts { get; set; } = new();
            public List<SegmentTrend> CustomerSegments { get; set; } = new();
            public List<SeasonalTrend> Seasonality { get; set; } = new();
        }

        public class ProductTrend
        {
            public string ProductName { get; set; } = string.Empty;
            public double Growth { get; set; } // percentage
            public decimal Revenue { get; set; }
        }

        public class SegmentTrend
        {
            public string Segment { get; set; } = string.Empty;
            public double Growth { get; set; } // percentage
            public int Count { get; set; }
        }

        public class SeasonalTrend
        {
            public string Month { get; set; } = string.Empty;
            public double Factor { get; set; } // 0.8 = 20% below average, 1.2 = 20% above
        }
    }
}
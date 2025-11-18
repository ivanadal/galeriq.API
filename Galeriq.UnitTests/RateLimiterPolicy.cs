using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

namespace Galeriq.UnitTests
{
    public static class RateLimiterPolicy
    {
        public static PartitionedRateLimiter<string> CreateLimiter()
        {
            return PartitionedRateLimiter.Create<string, string>(key =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: key,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromSeconds(30),
                        QueueLimit = 0
                    }));
        }
    }
}

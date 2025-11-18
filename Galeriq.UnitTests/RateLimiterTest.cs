using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.RateLimiting;

namespace Galeriq.UnitTests
{
    public class RateLimiterTests
    {
        [Fact]
        public async Task RateLimiter_Allows_20_Requests_Then_Blocks()
        {
            var limiter = RateLimiterPolicy.CreateLimiter();

            string key = "test-client";

            // Act: first 20 should succeed
            for (int i = 0; i < 20; i++)
            {
                var lease = await limiter.AcquireAsync(key);
                Assert.True(lease.IsAcquired, $"Request {i + 1} should be acquired");
            }

            // Act: 21st request should fail
            var blockedLease = await limiter.AcquireAsync(key);

            // Assert
            Assert.False(blockedLease.IsAcquired);
        }


    }

}

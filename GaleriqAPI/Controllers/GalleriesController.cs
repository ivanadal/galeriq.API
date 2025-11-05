using CollectionsAPI.Services;
using Galeriq.Data;
using Galeriq.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GaleriqAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalleriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ServiceBusQueueService _queueService;

        public GalleriesController(AppDbContext context, ServiceBusQueueService queueService)
        {
            _context = context;
            _queueService = queueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGallery([FromBody] Gallery gallery)
        {
            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();

            return Ok(gallery);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserGalleries(Guid userId)
        {
            var galleries = _context.Galleries
                .Where(g => g.UserId == userId)
                .ToList();
            return Ok(galleries);
        }


    }

}
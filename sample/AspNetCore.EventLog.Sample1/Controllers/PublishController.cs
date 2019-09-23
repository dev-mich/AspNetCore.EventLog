using System.Threading.Tasks;
using AspNetCore.EventLog.Sample1.Infrastructure;
using AspNetCore.EventLog.Sample1.IntegrationEvents;
using AspNetCore.EventLog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace AspNetCore.EventLog.Sample1.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PublishController : Controller
    {

        private readonly TestDbContext _context;
        private readonly IEventLogService _eventLog;

        public PublishController(TestDbContext context, IEventLogService eventLog)
        {
            _context = context;
            _eventLog = eventLog;
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var transaction = _context.Database.BeginTransaction();

            await _eventLog.SaveEventAsync(transaction.TransactionId, "test.event", new TestIntegrationEvent(), transaction.GetDbTransaction());

            transaction.Commit();

            await _eventLog.DispatchByPublisher(transaction.GetDbTransaction().Connection, transaction.TransactionId);

            return Ok();

        }


    }
}
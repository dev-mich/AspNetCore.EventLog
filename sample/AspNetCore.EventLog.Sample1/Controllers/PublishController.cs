﻿using System.Threading.Tasks;
using AspNetCore.EventLog.Interfaces;
using AspNetCore.EventLog.PostgreSQL.Extensions;
using AspNetCore.EventLog.Sample1.Entities;
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
        private readonly IPublisherService _publisherService;

        public PublishController(TestDbContext context, IPublisherService publisherService)
        {
            _context = context;
            _publisherService = publisherService;
        }


        [HttpPost]
        public async Task<IActionResult> Post()
        {

            using (var transaction = _context.Database.BeginTransaction(_publisherService))
            {
                var integrationEvt = new TestIntegrationEvent();

                await _publisherService.Publish("test.event", integrationEvt, "test_reply", integrationEvt.Id.ToString());

                _context.TestEntities.Add(new TestEntity());

                _context.SaveChanges();

                transaction.Commit();

            }

            return Ok();
        }


    }
}
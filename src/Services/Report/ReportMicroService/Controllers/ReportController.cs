using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AutoMapper;

using EventBusRabbitMQ.Contact;
using EventBusRabbitMQ.Core;
using EventBusRabbitMQ.Events;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using ReportMicroService.Entities;
using ReportMicroService.Repositories.Interfaces;


namespace ReportMicroService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repository;
        private readonly EventBusRabbitMQContact _eventBus;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportRepository repository, EventBusRabbitMQContact eventBus,
            IMapper mapper,
            ILogger<ReportController> logger)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
            _logger = logger;
        }

        #region Crud_Actions

        #region Report

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Report>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Report>>> GetReports()
        {
            var contacts = await _repository.GetReports();
            return Ok(contacts.OrderByDescending(m => m.CreationDate).ToList());
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public ActionResult GetReportCreate()
        {

            try
            {
                var model = new Report
                {
                    CreationDate = DateTime.Now,
                    ReportStatus = ReportStatus.Preparing
                };
                _repository.Create(model);

                var eventMessage = _mapper.Map<ReportCreateEvent>(model);
                _eventBus.Publish(EventBusConstants.ReportCreateQueue, eventMessage);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Publishing integration from {AppName}", "Report");
                throw;
            }

            return Ok();
        }

        #endregion

        #endregion
    }
}
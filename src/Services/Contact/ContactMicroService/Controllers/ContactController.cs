using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ContactMicroService.Entities;
using ContactMicroService.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactMicroService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class ContactController : ControllerBase
    {
        #region Variables

        private readonly IContactRepository _repository;
        private readonly ILogger<ContactController> _logger;

        #endregion

        #region Constructor

        public ContactController(IContactRepository repository, ILogger<ContactController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        #endregion

        #region Crud_Actions

        #region Contact

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Contact>), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var contacts = await _repository.GetContacts();
            return Ok(contacts);
        }

        [HttpGet("{id:length(24)}", Name = "GetContact")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Contact), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Contact>> GetContact(string id)
        {
            var contact = await _repository.GetContact(id);
            if (contact == null)
            {
                _logger.LogError($"Contact with id:{id},hasn't been found in database");
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Contact), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Contact>> CreateContact([FromBody] Contact contact)
        {
            await _repository.Create(contact);
            return CreatedAtRoute("GetContact", new { id = contact.Id }, contact);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Contact), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateContact([FromBody] Contact contact)
        {
            return Ok(await _repository.Update(contact));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Contact), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteContactById(string id)
        {
            return Ok(await _repository.Delete(id));
        }

        #endregion

        #region Contact_Detail

        [HttpPost("{contactId:length(24)}")]
        [ProducesResponseType(typeof(ContactDetail), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ContactDetail>> CreateContactDetail([FromBody] ContactDetail contactDetail,
            string contactId)
        {
            await _repository.CreateDetail(contactDetail, contactId);
            return CreatedAtRoute("GetContact", new { id = contactId }, contactDetail);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ContactDetail), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateContactDetail([FromBody] ContactDetail contactDetail)
        {
            return Ok(await _repository.UpdateDetail(contactDetail));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Contact), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteContactDetailById(string id)
        {
            return Ok(await _repository.DeleteDetail(id));
        }

        #endregion

        #endregion
    }
}
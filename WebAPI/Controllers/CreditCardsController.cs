using Business.Abstract;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardsController : ControllerBase
    {
        private readonly ICustomerCreditCardService _customerCreditCardService;

        public CreditCardsController(ICustomerCreditCardService customerCreditCardService)
        {
            _customerCreditCardService = customerCreditCardService;
        }

        [HttpPost("getcreditcardsbycustomerid")]
        public IActionResult GetCreditCardsByCustomerId([FromBody] int customerId)
        {
            var result = _customerCreditCardService.GetSavedCreditCardsByCustomerId(customerId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("savecreditcard")]
        public IActionResult SaveCreditCard(CustomerCreditCardModel customerCreditCardModel)
        {
            var result = _customerCreditCardService.SaveCustomerCreditCard(customerCreditCardModel);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("deletecreditcard")]
        public IActionResult DeleteCreditCard(CustomerCreditCardModel customerCreditCardModel)
        {
            var result = _customerCreditCardService.DeleteCustomerCreditCard(customerCreditCardModel);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}

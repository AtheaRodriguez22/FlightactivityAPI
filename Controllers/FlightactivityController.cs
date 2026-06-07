using FLIGHTLoyaltyCardDataService;
using FLIGHTLoyaltyCardModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightactivityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightactivityController : ControllerBase
    {
        private readonly LoyaltyDataService _dataService;
        public FlightactivityController()
        {
            _dataService = new LoyaltyDataService();
        }

        [HttpGet]
        public ActionResult<IEnumerable<LoyaltyAccount>> GetAllAccounts()
        {
            var accounts = _dataService.GetAccounts();
            return Ok(accounts);
        }
    }
}

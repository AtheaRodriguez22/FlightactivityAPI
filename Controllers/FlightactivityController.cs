using FLIGHTLoyaltyCardAppService;
using FLIGHTLoyaltyCardDataService;
using FLIGHTLoyaltyCardModels;
using Microsoft.AspNetCore.Http;    
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FlightactivityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    public class FlightactivityController : ControllerBase
    {
        private readonly LoyaltyAppService _appService;
        public FlightactivityController()
        {
            LoyaltyDataService dataService = new LoyaltyDataService();
            _appService = new LoyaltyAppService(dataService);
        }

        [HttpGet]
        public IEnumerable<LoyaltyAccount> GetAllAccounts()
        {
            return _appService.GetAll();
        }

        [HttpPost]
        public ActionResult AddAccount([FromBody] LoyaltyAccount newAccount)
        {
            if (newAccount == null)
            {
                return BadRequest(new { message = "No Data Receive." });
            }
            _appService.Add(newAccount);
            return Ok(new { message = "Account successfully added!" });
        }

        [HttpPut]
        public ActionResult UpdateAccount([FromBody] LoyaltyAccount updatedAccount)
        {
            if (updatedAccount == null)
            {
                return BadRequest(new { message = "No Data Receive." });
            }
            _appService.Update(updatedAccount);
            return Ok(new { message = "Account successfully updated!" });
        }

        [HttpGet("rewards")]
        public IEnumerable<RewardOption> GetAllRewards()
        {
            return _appService.GetRewards();
        }
 
        [HttpGet("rewards/{rewardId}")]
        public ActionResult<RewardOption> GetRewardById(int rewardId)
        {
            var reward = _appService.GetRewardById(rewardId);
            if (reward == null)
            {
                return NotFound(new { message = "Reward not found." });
            }
            return Ok(reward);
        }

        [HttpGet("vouchers/{code}")]
        public ActionResult<VoucherCode> GetVoucherByCode(string code)
        {
            var voucher = _appService.GetVoucherByCode(code);
            if (voucher == null)
            {
                return NotFound(new { message = "Invalid voucher code." });
            }
            return Ok(voucher);
        }

        [HttpPost("{flightNumber}/redeem/{rewardId}")]
        public ActionResult RedeemReward(string flightNumber, int rewardId)
        {
            try
            {
                var allAccounts = _appService.GetAll();
                var account = allAccounts.FirstOrDefault(a => a.FlightNumber.Trim() == flightNumber.Trim());
                if (account == null) return NotFound(new { message = "Account not found." });

                var reward = _appService.GetRewardById(rewardId);
                if (reward == null) return NotFound(new { message = "Invalid Reward ID." });

                if (account.Points >= reward.Cost)
                {
                    account.Points -= reward.Cost;
                    account.PointsHistory.Add($"[REDEEM] {reward.Name} redeemed for {reward.Cost} pts.");
                    _appService.Update(account);
                    return Ok(new { message = $"Successfully redeemed {reward.Name}! Remaining points: {account.Points}" });
                }

                return BadRequest(new { message = "Not enough points." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost("{flightNumber}/voucher/{code}")]
        public ActionResult ApplyVoucher(string flightNumber, string code)
        {
            try
            {
                var allAccounts = _appService.GetAll();
                var account = allAccounts.FirstOrDefault(a => a.FlightNumber.Trim() == flightNumber.Trim());
                if (account == null) return NotFound(new { message = "Account not found." });

                code = code.Trim().ToUpper();

                if (account.UsedVouchers.Contains(code))
                {
                    return BadRequest(new { message = "This voucher has already been used!" });
                }

                var voucher = _appService.GetVoucherByCode(code);
                if (voucher == null)
                {
                    return NotFound(new { message = "Invalid voucher code. Hint: Try FLY50, BONUS100, or WELCOME200" });
                }

                account.Points += voucher.Points;
                account.UsedVouchers.Add(code);
                account.PointsHistory.Add($"[VOUCHER] Code '{code}' applied +{voucher.Points} pts.");
                _appService.Update(account);

                return Ok(new { message = $"Voucher '{code}' applied! Total: {account.Points} pts." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }
 
        [HttpDelete("{flightNumber}")]
        public ActionResult DeleteAccount(string flightNumber)
        {
            try
            {
                var allAccounts = _appService.GetAll();
                var existingAccounts = allAccounts.Where(a => a.FlightNumber.Trim() == flightNumber.Trim()).ToList();
                if (!existingAccounts.Any())
                {
                    return NotFound(new { message = "Account not found." });
                }

                foreach (var acc in existingAccounts)
                {
                    _appService.Delete(acc.AccountID);
                }

                return Ok(new { message = $"Account(s) with flight number '{flightNumber}' successfully deleted!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }
    }
}
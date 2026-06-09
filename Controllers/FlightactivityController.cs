using FLIGHTLoyaltyCardAppService;
using FLIGHTLoyaltyCardDataService;
using FLIGHTLoyaltyCardModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Collections.Generic;
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

        // (GET)Retri, (POST) Add, Create accounts -> api/flightactivity 
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

            // (PUT) Modifies or Update Acc -> api/flightactivity
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

            // (GET) rewards -> api/flightactivity/rewards or api/flightactivity/rewards/1,2,3...
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

            // (GET) view voucher by code -> api/flightactivity/vouchers/FLY50
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

            // (POST) apply voucher to custom deduct and add -> api/flightactivity/{id}/redeem/{rewardId}
        [HttpPost("{id}/redeem/{rewardId}")]
            public ActionResult RedeemReward(Guid id, int rewardId)
            {
                var account = _appService.GetById(id);
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

                return BadRequest(new { message = "Not enough points!" });
            }

            // (POST) apply voucher codes and update -> api/flightactivity/{id}/voucher/{code}
        [HttpPost("{id}/voucher/{code}")]
            public ActionResult ApplyVoucher(Guid id, string code)
            {
                var account = _appService.GetById(id);
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

             // DELETE (DELETE) Remove Acc-> api/flightactivity/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteAccount(Guid id)
        {
            var existingAccount = _appService.GetById(id);
            if (existingAccount == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            _appService.Delete(id);
            return Ok(new { message = "Account successfully deleted!" });
        }
    }
    }
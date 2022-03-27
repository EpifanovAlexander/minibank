using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Web.Controllers.BankAccounts.Dto;

namespace Minibank.Web.Controllers.BankAccounts
{
    [ApiController]
    [Route("[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }


        [HttpGet("/{accountId}")]
        public async Task<BankAccountDto> GetBankAccountById(int accountId)
        {
            var model = await _bankAccountService.GetById(accountId);
            return new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum);
        }


        [HttpGet("user/{userId}")]
        public async IAsyncEnumerable<BankAccountDto> GetUserBankAccounts(int userId)
        {
            var userAccounts = _bankAccountService.GetUserBankAccounts(userId);

            await foreach (var account in userAccounts)
            {
                yield return new BankAccountDto(account.Id, account.UserId, account.Currency, account.Sum);
            }
        }


        [HttpPost]
        public async Task CreateBankAccount(CreateBankAccountDto model)
        {
            await _bankAccountService.Create(new CreateBankAccount(model.UserId, model.Currency, model.Sum));
        }


        [HttpPost("transfer")]
        public async Task TransferMoney(double sum, int fromAccountId, int toAccountId)
        {
            await _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId);
        }


        [HttpDelete("/{accountId}")]
        public async Task DeleteBankAccountById(int accountId)
        {
            await _bankAccountService.DeleteById(accountId);
        }

    }
}

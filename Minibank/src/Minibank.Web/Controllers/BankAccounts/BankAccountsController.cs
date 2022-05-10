using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Web.Controllers.BankAccounts.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Minibank.Web.Controllers.BankAccounts
{
    [ApiController]
   // [Authorize]
    [Route("[controller]")]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }


        [HttpGet("/{accountId}")]
        public async Task<BankAccountDto> GetBankAccountById(int accountId, CancellationToken cancellationToken)
        {
            var model = await _bankAccountService.GetById(accountId, cancellationToken);
            return new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum);
        }


        [HttpGet("user/{userId}")]
        public async Task<List<BankAccountDto>> GetUserBankAccounts(int userId, CancellationToken cancellationToken)
        {
            return (await _bankAccountService.GetUserBankAccounts(userId, cancellationToken))
                .Select(account => new BankAccountDto(account.Id, account.UserId, account.Currency, account.Sum))
                .ToList();
        }


        [HttpPost]
        public async Task CreateBankAccount(CreateBankAccountDto model, CancellationToken cancellationToken)
        {
            await _bankAccountService.Create(new CreateBankAccount(model.UserId, model.Currency, model.Sum), cancellationToken);
        }


        [HttpPost("transfer")]
        public async Task TransferMoney(double sum, int fromAccountId, int toAccountId, CancellationToken cancellationToken)
        {
            await _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId, cancellationToken);
        }


        [HttpDelete("/{accountId}")]
        public async Task DeleteBankAccountById(int accountId, CancellationToken cancellationToken)
        {
            await _bankAccountService.DeleteById(accountId, cancellationToken);
        }

    }
}

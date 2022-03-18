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
        public BankAccountDto GetBankAccountById(int accountId)
        {
            var model = _bankAccountService.GetById(accountId);
            return new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum);
        }


        [HttpGet("User/{userId}")]
        public IEnumerable<BankAccountDto> GetUserBankAccounts(int userId)
        {
            return _bankAccountService.GetUserBankAccounts(userId)
                .Select(model => new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum));
        }


        [HttpPost]
        public void CreateBankAccount(CreateBankAccountDto model)
        {
            _bankAccountService.Create(new CreateBankAccount(model.UserId, model.Currency, model.Sum));
        }


        [HttpPost("Transfer")]
        public void TransferMoney(double sum, int fromAccountId, int toAccountId)
        {
            _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId);
        }


        [HttpDelete("/{accountId}")]
        public void DeleteBankAccountById(int accountId)
        {
            _bankAccountService.DeleteById(accountId);
        }

    }
}

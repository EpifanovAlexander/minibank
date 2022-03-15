using Microsoft.AspNetCore.Mvc;
using Minibank.Web.Dtos;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Web.Controllers.TransferHistories;

namespace Minibank.Web.Controllers.BankAccounts
{
    [ApiController]
    [Route("[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }


        [HttpGet("Account/{accountId}")]
        public BankAccountDto GetBankAccountById(int accountId)
        {
            var model = _bankAccountService.Get(accountId);
            return new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum);
        }


        [HttpGet("UserAccounts/{userId}")]
        public IEnumerable<BankAccountDto> GetUserBankAccounts(int userId)
        {
            return _bankAccountService.GetUserBankAccounts(userId)
                .Select(model => new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum));
        }


        [HttpGet("TransferHistory/{userId}")]
        public IEnumerable<TransferHistoryDto> GetUserTransferHistory(int userId)
        {
            return _bankAccountService.GetUserTransferHistory(userId)
                .Select(model => new TransferHistoryDto(model.Id, model.Sum, model.FromAccountId, model.ToAccountId));
        }


        [HttpPost]
        public void CreateBankAccount(CreateBankAccountDto model)
        {
            _bankAccountService.Create(new BankAccount(model.UserId, model.Currency, model.Sum));
        }


        [HttpPost("Transfer")]
        public void TransferMoney(double sum, int fromAccountId, int toAccountId)
        {
            _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId);
        }


        [HttpDelete("Account/{accountId}")]
        public void DeleteBankAccountById(int accountId)
        {
            _bankAccountService.Delete(accountId);
        }

    }
}

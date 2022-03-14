using Microsoft.AspNetCore.Mvc;
using Minibank.Web.Dtos;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts;


namespace Minibank.Web.Controllers
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


        [HttpGet("GetAccount/{accountId}")]
        public BankAccountDto Get(int accountId)
        {
            var model = _bankAccountService.Get(accountId);
            return new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum);
        }


        [HttpGet("GetUserBankAccounts/{userId}")]
        public IEnumerable<BankAccountDto> GetUserBankAccounts(int userId)
        {
            return _bankAccountService.GetUserBankAccounts(userId)
                .Select(model => new BankAccountDto(model.Id, model.UserId, model.Currency, model.Sum));
        }


        [HttpGet("GetUserTransferHistory/{userId}")]
        public IEnumerable<TransferHistoryDto> GetUserTransferHistory(int userId)
        {
            return _bankAccountService.GetUserTransferHistory(userId)
                .Select(model => new TransferHistoryDto(model.Id, model.Sum, model.FromAccountId, model.ToAccountId));
        }


        [HttpPost("CreateBankAccount")]
        public void Create(BankAccountDto model)
        {
            _bankAccountService.Create(new BankAccount(model.UserId, model.Currency, model.Sum));
        }


        [HttpPost("TransferMoney")]
        public void TransferMoney(double sum, int fromAccountId, int toAccountId)
        {
            _bankAccountService.TransferMoney(sum, fromAccountId, toAccountId);
        }


        [HttpDelete("DeleteBankAccount/{id}")]
        public void Delete(int id)
        {
            _bankAccountService.Delete(id);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.BankTransferHistories.Services;
using Minibank.Web.Controllers.TransferHistories.Dto;

namespace Minibank.Web.Controllers.TransferHistories
{
    [ApiController]
    [Route("[controller]")]
    public class BankTransferHistoryController : ControllerBase
    {
        private readonly IBankTransferHistoryService _bankTransferHistoryService;

        public BankTransferHistoryController(IBankTransferHistoryService bankTransferHistoryService)
        {
            _bankTransferHistoryService = bankTransferHistoryService;
        }


        [HttpGet("{userId}")]
        public async IAsyncEnumerable<TransferHistoryDto> GetUserTransferHistory(int userId)
        {
            var userHistories = _bankTransferHistoryService.GetUserTransferHistory(userId);

            await foreach (var history in userHistories)
            {
                yield return new TransferHistoryDto(history.Id, history.Sum, history.FromAccountId, history.ToAccountId);
            }
        }

    }
}

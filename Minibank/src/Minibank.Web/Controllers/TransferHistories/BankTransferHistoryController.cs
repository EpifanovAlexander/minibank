using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.BankTransferHistories.Services;
using Minibank.Web.Controllers.TransferHistories.Dto;

namespace Minibank.Web.Controllers.TransferHistories
{
    [ApiController]
   // [Authorize]
    [Route("[controller]")]
    public class BankTransferHistoryController : ControllerBase
    {
        private readonly IBankTransferHistoryService _bankTransferHistoryService;

        public BankTransferHistoryController(IBankTransferHistoryService bankTransferHistoryService)
        {
            _bankTransferHistoryService = bankTransferHistoryService;
        }


        [HttpGet("{userId}")]
        public async Task<List<TransferHistoryDto>> GetUserTransferHistory(int userId, CancellationToken cancellationToken)
        {
            return (await _bankTransferHistoryService.GetUserTransferHistory(userId, cancellationToken))
                .Select(history => new TransferHistoryDto(history.Id, history.Sum, history.FromAccountId, history.ToAccountId))
                .ToList();
        }

    }
}

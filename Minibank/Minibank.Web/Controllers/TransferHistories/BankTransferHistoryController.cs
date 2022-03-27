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
        public IEnumerable<TransferHistoryDto> GetUserTransferHistory(int userId)
        {
            return _bankTransferHistoryService.GetUserTransferHistory(userId)
                .Select(model => new TransferHistoryDto(model.Id, model.Sum, model.FromAccountId, model.ToAccountId));
        }

    }
}

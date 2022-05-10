using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Currencies.Services;
using Minibank.Core.Domains.Currencies;
using Microsoft.AspNetCore.Authorization;

namespace Minibank.Web.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("[controller]")]
    public class ConvertController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConverter;
        public ConvertController(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }


        [HttpGet]
        public async Task<double> Convert(int amount, Currency fromCurrency, Currency toCurrency, CancellationToken cancellationToken)
        {
            return await _currencyConverter.Convert(amount, fromCurrency, toCurrency, cancellationToken);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Interfaces;

namespace Minibank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;
        public ConvertController(ICurrencyConverter converter)
        {
            _converter = converter;
        }


        [HttpGet(Name = "GetConvertResult")]
        public double Get(int sum, string currency)
        {
            return _converter.Convert(sum, currency);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Minibank.Core;

namespace Minibank.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertController : ControllerBase
    {
        private readonly IConverter _converter;
        public ConvertController(IConverter converter)
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

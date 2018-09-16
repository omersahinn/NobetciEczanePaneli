using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pharmacy.Extensions;

namespace pharmacy.Controllers
{
    [Produces("application/json")]
    [Route("api/pharmacy")]
    public class PharmacyController : Controller
    {
        [Route("{city}")]
        public async Task<IActionResult> Get(string city)
        {

            var result = await Services.PharmacyService.Instance.GetList(city.ClearText());
            if (result.Count == 0) return NotFound();
            else return Ok(result);
        }
    }
}
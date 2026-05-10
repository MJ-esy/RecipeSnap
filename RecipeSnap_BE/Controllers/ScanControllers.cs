using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeSnap_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanControllers : ControllerBase
    {
        //// GET: api/<ScanControllers>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<ScanControllers>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<ScanControllers>
        [HttpPost ("scan")]
        public async Task<IActionResult> UploadScannedImage([FromBody] IFormFile file, Boolean isMetric)
        {
            if (file.Length == 0) BadRequest("No file uploaded.");


            if (isMetric == true) {

            }

            else if (isMetric == false) {

            }

        }

        //// PUT api/<ScanControllers>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<ScanControllers>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}

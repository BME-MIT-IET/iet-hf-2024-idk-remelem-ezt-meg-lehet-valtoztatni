using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using WebShop.Bll.Dtos;
using WebShop.Bll.Interfaces;
using WebShop.Dal.Entities;

namespace WebShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        

        /// <summary>
        /// Feltölti a json fájlban lévő termékeket
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <returns>Semmi</returns>
        /// <exception cref="Exception">Hibás formátumú fájl</exception>
        /// <response code="200">Sikeres feltöltés</response>
        /// <response code="400">Helytelen formátumú json</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpPost("json")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> UploadJsonFilePost()
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Count != 1) {
                throw new IOException("Pontosan egy fájlt lehet csak feltölteni");
            }
            var jsonStream = files[0].OpenReadStream();
            StreamReader streamReader = new StreamReader(jsonStream);
            string json = streamReader.ReadToEnd();
            List<ProductIn>? result = JsonSerializer.Deserialize<List<ProductIn>>(json);

            if (result == null)
            {
                return BadRequest("A JSON feldolgozása sikertelen.");
            }

            await _productService.InsertProductsAsync(result);
            return NoContent();
        }

        /// <summary>
        /// Kiírja a termékeket egy json fájlba
        /// </summary>
        /// <returns>Json fájl</returns>
        /// <respones code="200">Sikeres exportálás</respones>
        [HttpPost("export")]
        [ProducesResponseType(200)]
        public async Task<ActionResult> ExportPost()
        {
            var products = _productService.GetProductsAsync(null, null, null);
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            // TODO make json from array
            await writer.WriteAsync(products.ToString());
            await writer.FlushAsync();
            stream.Position = 0;
            return File(stream, "application/octet-stream", "{{filename.ext}}");
        }

        // GET api/<Product>/5
        /// <summary>
        /// Kikeres egy terméket
        /// </summary>
        /// <param name="id">A keresett termék id-je</param>
        /// <returns>A keresett termék</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        /// <response code="404">A termék nem található</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductOut>> Get(int id)
        {
            var product = await _productService.GetProductAsync(id);
            return Ok(product);
        }

        // GET: api/<Product>
        /// <summary>
        /// Visszaadja a szűrő feltételeknek megfelelő termékeket
        /// </summary>
        /// <param name="categoryId">A termékek kategóriája</param>
        /// <param name="minPrice">A termékek minimum ára</param>
        /// <param name="maxPrice">A termékek maximum ára</param>
        /// <returns>A kritériumoknak megfelelő termékek</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<ProductOut>>> Get(
            [FromQuery]int? categoryId = null,
            [FromQuery]int? minPrice = null,
            [FromQuery]int? maxPrice = null
            )
        {
            var product = await _productService.GetProductsAsync(categoryId, minPrice, maxPrice);
            return Ok(product);
        }

        // POST api/<Product>
        /// <summary>
        /// Létrehoz egy új terméket
        /// </summary>
        /// <param name="products">Az új termékek listája</param>
        /// <returns>Semmi</returns>
        /// <response code="201">Sikeres feltöltés</response>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<ActionResult<IEnumerable<ProductOut>>> Post([FromBody] List<ProductIn> products)
        {
            var created = await _productService.InsertProductsAsync(products);
            return CreatedAtAction(nameof(Get), created);
        }

        // PUT api/<Product>/5
        /// <summary>
        /// Frissít egy terméket
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A frissítendő termék id-je</param>
        /// <param name="product">A termék új értéke</param>
        /// <returns>Semmi</returns>
        /// <response code="204">Sikeres frissítés</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Put(int id, [FromBody] ProductIn product)
        {
            await _productService.UpdateProductAsync(id, product);
            return NoContent();
        }

        // DELETE api/<Product>/5
        /// <summary>
        /// Töröl egy terméket
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A törlendő termék id-je</param>
        /// <returns>Semmi</returns>
        /// <response code="202">Sikeres törlés</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(202)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Accepted();
        }
    }
}

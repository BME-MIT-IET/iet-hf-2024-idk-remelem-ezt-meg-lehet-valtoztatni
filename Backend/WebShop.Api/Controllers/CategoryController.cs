using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.Bll.Dtos;
using WebShop.Bll.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Visszaadja az összes kategóriát
        /// </summary>
        /// <returns>Az összes kategória</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<Category>>> GetAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        // GET api/<CategoryController>/5
        /// <summary>
        /// Visszaadja a keresett id-vel rendelkező kategóriát
        /// </summary>
        /// <param name="id">A keresett kategória id-je</param>
        /// <returns>A keresett kategória</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        /// <response code="404">A kategória nem található</response>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);
            return Ok(category);
        }

        // POST api/<CategoryController>
        /// <summary>
        /// Létrehoz egy új kategóriát
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="category">Az új kategória</param>
        /// <returns>Semmit</returns>
        /// <response code="204">Kategória sikeresen létrehozva</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Post([FromBody] Category category)
        {
            await _categoryService.InsertCategoryAsync(category);
            return NoContent();
        }

        // PUT api/<CategoryController>/5
        /// <summary>
        /// Frissít egy kategóriát
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A frissítendő kategória id-je</param>
        /// <param name="category">A kategória új értéke</param>
        /// <returns>Az adatbázisban elmentett kategória</returns>
        /// <response code="200">Sikeres frissítés</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category category)
        {
            var newCategory = await _categoryService.UpdateCategoryAsync(id, category);
            return Ok(newCategory);
        }

        // DELETE api/<CategoryController>/5
        /// <summary>
        /// Töröl egy kategóriát
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A törlendő kategória id-je</param>
        /// <returns>Semmi</returns>
        /// <response code="202">Sikeres törlés</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(202)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Accepted();
        }
    }
}

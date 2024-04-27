using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/<UserController>/5
        /// <summary>
        /// Kikeres egy felhasználót
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A keresett felhasználó id-je</param>
        /// <returns>A keresett felhasználó</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        /// <response code="404">A keresett felhasználó nem található</response>
        /// <response code="405">Nem admin felhasználó</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(405)]
        public async Task<ActionResult<UserOut>> Get(int id)
        {
            var user = await _userService.GetUserAsync(id);
            return Ok(user);
        }

        // POST api/<UserController>/login
        /// <summary>
        /// Bejelentkezteti a felhasználót
        /// </summary>
        /// <param name="user">A felhasználó adatai</param>
        /// <returns>A bejelentkezett felhasználó</returns>
        /// <response code="200">Sikeres bejelentkezés</response>
        /// <response code="404">A keresett felhasználó nem található</response>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<UserOut>> LoginPost([FromBody] UserIn user)
        {
            var foundUser = await _userService.FindUserAsync(user);

            await HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()),
                        new Claim(ClaimTypes.Role, foundUser.IsAdmin ? "Admin" : "User")
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme
                )
            ));
            return Ok(foundUser);
        }

        // POST api/<UserController>/register
        /// <summary>
        /// Beregisztrálja a felhasználót
        /// </summary>
        /// <param name="userIn">Az új felhasználó adatai</param>
        /// <returns>Az elmentett felhasználó</returns>
        /// <response code="201">Sikeres regisztrálás</response>
        [HttpPost("register")]
        [ProducesResponseType(201)]
        public async Task<ActionResult<UserOut>> RegisterPost([FromBody] UserIn userIn)
        {
            if (userIn.Name == null) {
                throw new FieldIsRequiredException("A mező megadása kötelező");
            }

            var user = await _userService.InsertUserAsync(userIn);
            return CreatedAtAction(nameof(RegisterPost), user);
        }

        // POST api/<UserController>/logout
        /// <summary>
        /// Kijelentkezteti a felhasználót
        /// </summary>
        /// <returns>Semmi</returns>
        /// <response code="200">Sikeres kejelentkezés</response>
        /// <response code="401">A felhasználó nincs bejelentkezve</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}

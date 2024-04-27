using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using System.Collections;
using System.IO;
using System.Security.Claims;
using WebShop.Bll.Dtos;
using WebShop.Bll.Interfaces;
using WebShop.Bll.Services;
using WebShop.Dal;
using WebShop.Dal.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET api/<Order>
        /// <summary>
        /// Visszaadja a felhasználóhoz tartozó rendeléseket
        /// Admin felhasználóknak minden rendelést visszaad
        /// </summary>
        /// <returns>A rendelések</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        /// <response code="401">A felhasználó nincs bejelentkezve</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<Order>> Get()
        {
            if (ClaimsHelper.IsAdmin(HttpContext.User))
            {
                var orders = await _orderService.GetOrdersAsync();
                return Ok(orders);
            }

            var userId = ClaimsHelper.GetUserId(HttpContext.User);
            var order = await _orderService.GetUserOrdersAsync(userId);
            return Ok(order);
        }

        private static Stream GeneratePDFReceipt(OrderOut order)
        {
            var document = new PdfDocument();
            var page = document.AddPage();

            var gfx = XGraphics.FromPdfPage(page);
            var arial = new XFont("Arial", 15);
            var arialB = new XFont("Arial", 20, XFontStyle.Bold);
            var arialSmall = new XFont("Arial", 10);

            var black = XBrushes.Black;
            var fullPage = new XRect(0, 20, page.Width, page.Height);

            gfx.DrawString("Számla", arialB, black, fullPage, XStringFormats.TopCenter);
            gfx.DrawString("Vásárolt termékek:", arialB, black, new XRect(10, 40, 0, 0), XStringFormats.TopLeft);
            int index = 0;
            foreach (var orderItem in order.OrderItems)
            {
                var box = new XRect(10, 70 + index * 30, page.Width - 20, 25);
                var product = orderItem.Product;
                gfx.DrawString(product.Name, arial, black, box, XStringFormats.TopLeft);
                gfx.DrawString($"(${orderItem.Quantity}db*{product.Price}$)={orderItem.Quantity * product.Price}$", arial, black, box, XStringFormats.TopRight);
                gfx.DrawString(product?.Description ?? "", arialSmall, black, box, XStringFormats.BottomLeft);
                index++;
            }
            Stream pdf = new MemoryStream();
            document.Save(pdf);
            return pdf;
        }

        /// <summary>
        /// Készít egy pdf-et a rendelésről
        /// </summary>
        /// <param name="id">A rendelés id-je</param>
        /// <returns>A rendelés számlája pdf formátumban</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        /// <response code="401">A felhasználó nincs bejelentkezve</response>
        /// <response code="404">A megrendelés nem található</response>
        [HttpGet("{id}/pdf")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [Produces("application/pdf")]
        public async Task<IActionResult> GetPDFReciept(int id)
        {
            OrderOut order;
            if (ClaimsHelper.IsAdmin(HttpContext.User))
            {
                order = await _orderService.GetOrderAsync(id);
            }
            else
            {
                var userId = ClaimsHelper.GetUserId(HttpContext.User);
                var userOrders = await _orderService.GetUserOrdersAsync(userId);
                order = userOrders.Single(o => o.Id == id);
            }
            
            var pdf = GeneratePDFReceipt(order);
            return File(pdf, "application/pdf", "Számla.pdf"); ;
        }

        // GET api/<Order>/5
        /// <summary>
        /// Kikeres egy rendelést
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A rendelés id-je</param>
        /// <returns>A keresett rendelés</returns>
        /// <response code="200">Sikeres lekérdezés</response>
        /// <response code="401">Nem admin felhasználó</response>
        /// <response code="404">A megrendelés nem található</response>
        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            return Ok(order);
        }

        // POST api/<Order>
        /// <summary>
        /// Létrehoz egy új rendelést a bejelentkezett felhasználó részéről
        /// </summary>
        /// <param name="order">Az új rendelés</param>
        /// <returns>Az elmentett rendelés</returns>
        /// <response code="200">Sikeres frissítés</response>
        /// <response code="401">A felhasználó nincs bejelentkezve</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<OrderOut>> Post([FromBody] OrderIn order)
        {
            var userId = ClaimsHelper.GetUserId(HttpContext.User);
            var savedOrder = await _orderService.InsertOrderAsync(userId, order);
            return Ok(savedOrder);
        }

        // PUT api/<Order>/5
        /// <summary>
        /// Frissít egy rendelést
        /// Csak admin felhasználó éri el
        /// </summary>
        /// <param name="id">A frissítendő rendelés id-je</param>
        /// <param name="value">Az új érték</param>
        /// <returns>Semmi</returns>
        /// <response code="204">Sikeres frissítés</response>
        /// <response code="401">Nem admin felhasználó</response>
        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> Put(int id, [FromBody] OrderIn value)
        {
            await _orderService.UpdateOrderAsync(id, value);
            return NoContent();
        }
    }
}

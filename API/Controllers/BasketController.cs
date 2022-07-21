using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController:BaseApiController
    {
        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            this._context = context;
        }

       [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            Basket? basket = await RetriveBasket();

            if (basket == null) return NotFound();

            return MapBaketToDto(basket);
        }

        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            var basket = await RetriveBasket();
            if(basket == null) basket = AddBasket();
            var product = await _context.Products.FindAsync(productId);
            if(product == null) return NotFound();

            basket.AddItem(product, quantity);

            var result = await _context.SaveChangesAsync() > 0;

            if(result) return CreatedAtRoute("GetBasket",MapBaketToDto(basket));

            return BadRequest(new ProblemDetails{Title ="Problem saving item to basket"});
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            var basket = await RetriveBasket();

            if(basket == null) return NotFound();

            basket.RemoveItem(productId, quantity);

            var result = await _context.SaveChangesAsync() > 0;

            if(result) return Ok();

            return BadRequest(new ProblemDetails{ Title = "Problem removing item from the basket" });
        }

        private async Task<Basket> RetriveBasket()
        {
            return await _context.Baskets.Include(i => i.Items).ThenInclude(p => p.Product).FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
        }

        private Basket AddBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions{ IsEssential= true, Expires=DateTime.Now.AddDays(30)};
            Response.Cookies.Append("buyerId",buyerId,cookieOptions);
            var basket = new Basket {BuyerId = buyerId};
            _context.Baskets.Add(basket);

            return basket;
        }

        private BasketDto MapBaketToDto(Basket basket)
        {
            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                  ProductId = item.ProductId,
                  Name = item.Product.Name,
                  Price = item.Product.Price,
                  PictureUrl = item.Product.PictureUrl,
                  Brand = item.Product.Brand,
                  Type = item.Product.Type,
                  Quantity= item.Quantity
                }).ToList()
            };
        }
    }
}
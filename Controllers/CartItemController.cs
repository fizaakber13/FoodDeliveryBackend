using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryBackend.Models;
using FoodDeliveryBackend.Data;

namespace FoodDeliveryBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartItemController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            return await _context.CartItems
                                 .Include(c => c.MenuItem)
                                 .Include(c => c.Restaurant)
                                 .Include(c => c.User)
                                 .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems
                                         .Include(c => c.MenuItem)
                                         .Include(c => c.Restaurant)
                                         .Include(c => c.User)
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (cartItem == null)
                return NotFound();

            return cartItem;
        }

        
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByUserId(int userId)
        {
            var cartItems = await _context.CartItems
                                          .Where(c => c.UserId == userId)
                                          .Include(c => c.MenuItem)
                                          .Include(c => c.Restaurant)
                                          .ToListAsync();
            return cartItems;
        }

        
        [HttpGet("summary/{userId}")]
        public async Task<ActionResult> GetCartSummaryByUser(int userId)
        {
            var groupedCart = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Restaurant)
                .Include(ci => ci.MenuItem)
                .GroupBy(ci => ci.Restaurant)
                .Select(group => new
                {
                    RestaurantId = group.Key.Id,
                    RestaurantName = group.Key.Name,
                    TotalPrice = group.Sum(ci => ci.MenuItem.Price * ci.Quantity),
                    Items = group.Select(ci => new
                    {
                        ci.Id,
                        ci.MenuItem.Name,
                        ci.Quantity,
                        UnitPrice = ci.MenuItem.Price,
                        Subtotal = ci.MenuItem.Price * ci.Quantity
                    }).ToList()
                })
                .ToListAsync();

            return Ok(groupedCart);
        }

       
        [HttpGet("user/{userId}/restaurant/{restaurantId}/total")]
        public async Task<ActionResult<decimal>> GetCartTotal(int userId, int restaurantId)
        {
            var total = await _context.CartItems
                .Where(c => c.UserId == userId && c.RestaurantId == restaurantId)
                .Include(c => c.MenuItem)
                .SumAsync(c => c.MenuItem.Price * c.Quantity);

            return Ok(total);
        }

      
        [HttpGet("user/{userId}/restaurant/{restaurantId}/total-with-coupon")]
        public async Task<ActionResult> GetCartTotalWithCoupon(int userId, int restaurantId, [FromQuery] string couponCode)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId && ci.RestaurantId == restaurantId)
                .Include(ci => ci.MenuItem)
                .ToListAsync();

            if (!cartItems.Any())
                return Ok(new { Total = 0, Discount = 0, FinalAmount = 0, Message = "Cart is empty." });

            var total = cartItems.Sum(ci => ci.MenuItem.Price * ci.Quantity);

           
            var coupon = await _context.RestaurantCoupons
                .Where(rc => rc.RestaurantId == restaurantId && rc.Coupon.Code == couponCode && rc.Coupon.IsActive)
                .Select(rc => rc.Coupon)
                .FirstOrDefaultAsync();

            if (coupon == null)
                return Ok(new { Total = total, Discount = 0, FinalAmount = total, Message = "Invalid or inactive coupon." });

            
            if (total < (double)coupon.MinOrderAmount)
                return Ok(new
                {
                    Total = total,
                    Discount = 0,
                    FinalAmount = total,
                    Message = $"Minimum order ₹{coupon.MinOrderAmount} required to apply this coupon."
                });

            double discount = 0;

            if (coupon.DiscountType == "Flat")
                discount = (double)coupon.DiscountAmount;
            else if (coupon.DiscountType == "Percentage")
                discount = total * ((double)coupon.DiscountAmount / 100);



            var finalAmount = total - discount;

            return Ok(new
            {
                Total = total,
                Discount = discount,
                FinalAmount = finalAmount,
                Message = "Coupon applied successfully."
            });
        }

        
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddCartItem(CartItem cartItem)
        {
            try
            {
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci =>
                        ci.UserId == cartItem.UserId &&
                        ci.MenuItemId == cartItem.MenuItemId &&
                        ci.RestaurantId == cartItem.RestaurantId);

                if (existingItem != null)
                {
                    existingItem.Quantity += cartItem.Quantity;
                    _context.Entry(existingItem).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok(existingItem);
                }

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCartItem), new { id = cartItem.Id }, cartItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.Id)
                return BadRequest();

            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpPut("{id}/quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity(int id, [FromBody] int newQuantity)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
                return NotFound();

            if (newQuantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = newQuantity;
                _context.Entry(cartItem).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }


}

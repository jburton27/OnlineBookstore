using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace OnlineBookstore.Pages
{
    public class CartModel : PageModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public decimal GrandTotal => CartItems.Sum(x => x.Price * x.Quantity);

        public void OnGet()
        {
            CartItems = GetCart();
        }

        public IActionResult OnPostUpdateQuantity(int bookId, int quantity)
        {
            List<CartItem> cart = GetCart();

            CartItem? item = cart.FirstOrDefault(x => x.BookID == bookId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
            }

            SaveCart(cart);
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveItem(int bookId)
        {
            List<CartItem> cart = GetCart();

            CartItem? item = cart.FirstOrDefault(x => x.BookID == bookId);
            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);
            return RedirectToPage();
        }

        private List<CartItem> GetCart()
        {
            string? cartJson = HttpContext.Session.GetString("Cart");

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
        }
    }
}

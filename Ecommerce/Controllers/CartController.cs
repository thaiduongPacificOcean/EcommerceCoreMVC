﻿using Ecommerce.Data;
using Ecommerce.ViewModels;
using Ecommerce.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;
        public CartController(Hshop2023Context context) => db = context;

        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.MaHh == id);
            if (item == null) {
                var product = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (product == null) {
                    TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                    return Redirect("/404");
                }
                // có 
                item = new CartItem
                {
                    MaHh = product.MaHh,
                    TenHH = product.TenHh,
                    DonGia = product.DonGia ?? 0,
                    Hinh = product.Hinh ?? string.Empty,
                    SoLuong = quantity,
                };
                cart.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(CART_KEY, cart);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }

            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                }

                var hoadon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GRAB",
                    MaTrangThai = 0,
                    GhiChu = model.GhiChu
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Database.CommitTransaction();
                    db.Add(hoadon);
                    db.SaveChanges();

                    var cthds = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            MaHh = item.MaHh,
                            GiamGia = 0
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }

            return View(Cart);
        }
        
    }
}
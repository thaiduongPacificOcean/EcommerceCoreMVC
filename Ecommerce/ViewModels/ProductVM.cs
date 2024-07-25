﻿namespace Ecommerce.ViewModels
{
    public class ProductVM
    {
        public int MaHH { get; set; }
        public string TenHH { get; set; }
        public double DonGia { get; set; }
        public string HinhAnh { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
    }
    public class ProductDetailVM
    {
        public int MaHh { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
        public string ChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTon { get; set; }
    }
}

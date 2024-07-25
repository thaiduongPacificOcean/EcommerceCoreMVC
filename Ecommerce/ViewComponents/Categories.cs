using Ecommerce.Data;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.ViewComponents
{
    public class Categories : ViewComponent
    {
        private readonly Hshop2023Context db;

        public Categories(Hshop2023Context context)
        {
            db = context;
        }
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new CategoriesVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.TenLoai);
            return View(data);
        }
    }
}

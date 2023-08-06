using BStore.DataAccess.Data;
using BStore.DataAccess.Repository.IRepository;
using BStore.Models;
using BStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll().ToList();
            // Projections in EF Core_en


            return View(objProductsList);
        }
        public IActionResult Upsert(int? id)
        {

            ProductVM viewModel = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.CategorName,
                    Value = u.CategoryId.ToString()
                }),
                GetProduct = new Product()
            };
            if (id == null || id == 0)
            {
                return View(viewModel);
                

            }
            else
            {
                viewModel.GetProduct = _unitOfWork.Product.Get(u => u.ProductId == id);
                return View(viewModel);

            }

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string productPath=Path.Combine(wwwRootPath, @"images/product");

                    if(!string.IsNullOrEmpty(obj.GetProduct.ImgUrl))
                    {
                        var oldImagePath=Path.Combine(wwwRootPath,obj.GetProduct.ImgUrl.TrimStart('/'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using(var fileStream=new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.GetProduct.ImgUrl = @"images/product/" + fileName; 
                }
                if (obj.GetProduct.ProductId == 0)
                {
                    _unitOfWork.Product.Add(obj.GetProduct);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.GetProduct);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {

                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.CategorName,
                    Value = u.CategoryId.ToString()
                });

                return View(obj);
            }

        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product product = _unitOfWork.Product.Get(u => u.ProductId == id);
        //    if (product == null) { return NotFound(); }
        //    return View(product);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();

        //}
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }
            Product product = _unitOfWork.Product.Get(u => u.ProductId == id);
            if (product == null) { return NotFound(); }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product product = _unitOfWork.Product.Get(u => u.ProductId == id);
            if (product == null) { return NotFound(); }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";

            return RedirectToAction("Index");


        }

    }
}

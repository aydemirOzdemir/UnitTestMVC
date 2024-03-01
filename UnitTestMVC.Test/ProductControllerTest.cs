using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestMVC.Web.Controllers;
using UnitTestMVC.Web.Models;
using UnitTestMVC.Web.Repository;

namespace UnitTestMVC.Test;

public class ProductControllerTest
{
    private readonly Mock<IRepository<Product>> moqProductRepo;
    private readonly ProductController productController;
    private List<Product> products; 
    public ProductControllerTest()
    {
        moqProductRepo = new Mock<IRepository<Product>>();
        productController=new ProductController(moqProductRepo.Object);
        products = new List<Product>() { new Product { Id = 2, Name = "Kalem", Price = 100, Stock = 50, Color = "Blue" }, new Product { Id = 3, Name = "Defter", Price = 20, Stock = 150, Color = "Red" } };
    }
    [Fact]
    public async Task Index_ActionExecute_ReturnView()
    {
        var result=await productController.Index();

        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public async Task Index_ActionExecute_ReturnProductList()
    {
       moqProductRepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);
        var result=await productController.Index();
        var viewResult= Assert.IsType<ViewResult>(result);
        var productList=Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Equal<int>(2, productList.Count());
      
    }
    [Fact]
    public async Task Details_IdIsNull_ReturnRedirectToIndexAction()
    {
        var result = await productController.Details(null);
       var redirect= Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index",redirect.ActionName);
    }

    [Fact]

    public async Task Details_IdInValid_ReturnNotFound()
    {
        Product product = null;
        moqProductRepo.Setup(repo => repo.GetById(0)).ReturnsAsync(product);
        var result = await productController.Details(0);
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal<int>(404,notFoundResult.StatusCode);
    }




    [Theory]
    [InlineData(2)]
    public async Task Details_IdValid_ReturnProduct(int id)
    {
        moqProductRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(products.FirstOrDefault(x=>x.Id==id));
        var result = await productController.Details(id);
      var viewresult=  Assert.IsType<ViewResult>(result);
        var product=Assert.IsAssignableFrom<Product>(viewresult.Model);
        Assert.Equal(id,product.Id);
    }
    [Fact]
    public async Task Create_ActionExecute_ReturnView()
    {
        var result=productController.Create();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_InValidModelState_ReturnViewWithProduct()
    {
        productController.ModelState.AddModelError("Name", "Name alanı boş bırakılamaz.");
        var result= await productController.Create(products.First());
      var viewResult=  Assert.IsType<ViewResult>(result);
        Assert.IsType<Product>(viewResult.Model);
    }
    [Fact]
    public async Task Create_ValidModelState_ReturnRedirectToIndex()
    {
       var result=await productController.Create(products.First());
        var redirectResult=Assert.IsType<RedirectToActionResult>(result);

        Assert.Equal("Index",redirectResult.ActionName);

    }
    [Fact]
    public async Task Create_ValidModelState_CreateMethodExecute()
    {
        Product product = null;
        moqProductRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x=>product=x);
        var result=productController.Create(products.First());
        moqProductRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
        Assert.Equal(products.First().Id, product.Id);
    }

    [Fact]
    public async Task Create_InValidModelState_NeverCreateExecut()
    {
        productController.ModelState.AddModelError("Name","Name alanı boş bırakılamaz");
        var result= await productController.Create(products.First());
        moqProductRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Edit_IdIsNull_ReturnRediretToIndex()
    {
        var result = await productController.Edit(null);
        var redirectResult=Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
    [Theory]
    [InlineData(0)]
    public async Task Edit_IdInValid_ReturnNotFound(int productId)
    {
        Product product = null;
        moqProductRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
        var result=await productController.Edit(productId);
        var notFoundResult=Assert.IsType<NotFoundResult>(result);

        Assert.Equal<int>(404, notFoundResult.StatusCode);
    }
    [Theory]
    [InlineData(2)]
    public async Task Edit_IdValid_ReturnProduct(int id)
    {
        moqProductRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(products.FirstOrDefault(c=>c.Id==id));
        var result= await productController.Edit(id);
        var viewResult = Assert.IsType<ViewResult>(result);
        var resultProduct=Assert.IsAssignableFrom<Product>(viewResult.Model);
        Assert.Equal(products.FirstOrDefault(c => c.Id == id), viewResult.Model);
    }
    [Theory]
    [InlineData(2)]
    public void EditPost_IdIsNotEqualProduct_ReturnNotFound(int id)
    {
        var result= productController.Edit(3,products.First(x=>x.Id==id));
        var notFound=Assert.IsType<NotFoundResult>(result);
    }
    [Theory]
    [InlineData(2)]
    public void EditPost_InValidModelState_ReturnView(int id)
    {
        productController.ModelState.AddModelError("Name","");
        var result= productController.Edit(id,products.First(x=>x.Id==id));
        var viewResult=Assert.IsType<ViewResult>(result);
        Assert.IsType<Product>(viewResult.Model);
    }

    [Theory]
    [InlineData(2)]
    public void EditPost_ValidModelState_ReturnRedirectToIndex(int id)
    {
        Product product=products.First(x=>x.Id==id);
        var result = productController.Edit(id, product);
        var redirect=Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index",redirect.ActionName);
    }
    [Theory]
    [InlineData(2)]
    public void EditPost_ValidModelState_UpdateExecute(int id)
    {
        Product product= products.First(x => x.Id == id);
        moqProductRepo.Setup(x => x.Update(product));
        productController.Edit(id,product);
        moqProductRepo.Verify(repo=>repo.Update(product),Times.Once);
    
    
    }


    [Fact]
    public async Task Delete_IdIsNull_ReturnNotFound()
    {
        var result = await productController.Delete(null);
     Assert.IsType<NotFoundResult>(result);
    }
    [Theory]
    [InlineData(0)]
    public async Task Delete_IdIsNotEqualProduct_ReturnNotFound(int id)
    {
        Product product = null;
        moqProductRepo.Setup(x=>x.GetById(id)).ReturnsAsync(product);
        var result= await productController.Delete(id);
        var notFound= Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData(2)]
    public async Task Delete_ActionExecute_ReturnView(int id)
    {
        Product product=products.First(x => x.Id==id);
        moqProductRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
      var result=await productController.Delete(id);
        var viewResult=Assert.IsType<ViewResult>(result);
        Assert.Equal(product, viewResult.Model);
    }
    [Theory]
    [InlineData(2)]
    public async Task DeleteConfirm_ActionResult_ReturnRedirectToIndex(int id)
    {
        var result=await productController.DeleteConfirmed(id);
        var redirectToAction=Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToAction.ActionName);

    }

    [Theory]
    [InlineData(2)]
    public async Task DeleteConfirmed_ActionExecute_ReturnDeleteMethod(int id)
    {
        Product product=products.First(x=>x.Id==id);
        moqProductRepo.Setup(repo => repo.Delete(product));
        var result=await productController.DeleteConfirmed(id);
        moqProductRepo.Verify(repo=>repo.Delete(It.IsAny<Product>()),Times.Once);
    }


}

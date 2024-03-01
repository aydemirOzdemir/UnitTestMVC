using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestMVC.Web.Controllers;
using UnitTestMVC.Web.Helpers;
using UnitTestMVC.Web.Models;
using UnitTestMVC.Web.Repository;

namespace UnitTestMVC.Test;

public class ProductApiControllerTest
{
    private readonly Mock<IRepository<Product>> mockRepo;
    private readonly ProductsApiController controller;
    private List<Product> products;
    private readonly Helper helper;
    public ProductApiControllerTest()
    {
        mockRepo = new Mock<IRepository<Product>>();
        controller = new ProductsApiController(mockRepo.Object);
        products = new List<Product>() { new Product { Id = 1, Name = "Kalem", Price = 100, Stock = 50, Color = "Blue" }, new Product { Id = 2, Name = "Defter", Price = 20, Stock = 150, Color = "Red" } };
        helper=new Helper();

    }

    [Fact]
    public async Task GetProduct_ActionExecute_ReturnOkWithProduct()
    {
        mockRepo.Setup(x => x.GetAll()).ReturnsAsync(products);
        var result= await controller.GetProduct();
        var OkResult=Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<IEnumerable<Product>>(OkResult.Value);
        Assert.Equal(products, OkResult.Value);
    }

    [Theory]
    [InlineData(3)]
    public async Task GetProduct_ProductIsNull_ReturnNotFound(int id)
    {
        Product product = null;
        mockRepo.Setup(repo=>repo.GetById(id)).ReturnsAsync(product);
        var result= await controller.GetProduct(id);
        var notfoundResult=Assert.IsType<NotFoundResult>(result);
    }
    [Theory]
    [InlineData(1)]
    public async Task GetProduct_IdIsValid_ReturnOkWithProduct(int id)
    {
        Product product = products.First(x=>x.Id==id);
        mockRepo.Setup(repo => repo.GetById(id)).ReturnsAsync(product);
        var result = await controller.GetProduct(id);
        var Okresult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<Product>(Okresult.Value);
        Assert.Equal(product, Okresult.Value);
    }

    [Theory]
    [InlineData(1)]
    public  void PutProduct_IdIsNotEqualProduct_ReturnBadRequest(int id)
    {
        Product product = products.First(x=>x.Id == 2);
        var result = controller.PutProduct(id, product);
        Assert.IsType<BadRequestResult>(result);
    }

    [Theory]
    [InlineData(1)]
    public void PutProduct_ActionExecute_ReturnNoContent(int id)
    {
        Product product = products.First(x => x.Id == id);
        mockRepo.Setup(repo => repo.Update(product));
        var result= controller.PutProduct(id, product);
        mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }
    [Theory]
    [InlineData(1)]
    public async Task PostProduct_ActionExecute_ReturnCreateAtAction(int id)
    {
        Product product = products.First(x=> x.Id == id);
        mockRepo.Setup(repo => repo.Create(product)).Returns(Task.CompletedTask);
        var result = await controller.PostProduct(product);
        mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
        var createResult=Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetProduct", createResult.ActionName);
    }
    [Theory]
    [InlineData(3)]
    public async Task DeleteProduct_IdInValid_ReturnNotFound(int id)
    {
        Product product = null;
        mockRepo.Setup(repo=>repo.GetById(id)).ReturnsAsync(product);
        var result = await controller.DeleteProduct(id);
        Assert.IsType<NotFoundResult>(result.Result);//Burada Iactionresult Değil actionresult kontrol et ondan .Result dedik.

    }

    [Theory]
    [InlineData(2)]
    public async Task DeleteProduct_IdValid_ReturnNoContent(int id)
    {
        Product product = products.First(x=>x.Id == id);
        mockRepo.Setup(repo=>repo.GetById(id)).ReturnsAsync(product);
        mockRepo.Setup(repo => repo.Delete(product));
        var result = await controller.DeleteProduct(id);
        mockRepo.Verify(repo => repo.GetById(It.IsAny<int>()), Times.Once);
        mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);
        Assert.IsType<NoContentResult>(result.Result);

    }

    [Theory]
    [InlineData(1,2,3)]
     public void Add_SampleValues_ReturnTotal(int a,int b,int total)
    {
        var result = helper.add(a, b);
        Assert.Equal(total,result);
    }



}

﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestMVC.Web.Controllers;
using UnitTestMVC.Web.Models;

namespace UnitTestMVC.Test;

public class ProductsControllerTestWithSqllite:ProductsControllerTest
{
    public ProductsControllerTestWithSqllite()
    {

        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        SetDbContextOptions(new DbContextOptionsBuilder<TestContext>().UseSqlite(connection).Options);
    }
    [Fact]
    public async Task Create_ValidStateProduct_ReturnRedirectToactionWithProduct()
    {
        var newProduct = new Product() { Name = "Kalem30", Price = 200, Stock = 50,Color="Red" };
        using (var context = new TestContext(contextOptions))
        {
            var category = context.Categories.First();
            newProduct.CategoryId = category.Id;
            var controller = new ProductsController(context);
            var result = await controller.Create(newProduct);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToAction.ActionName);

        }
        using (var context = new TestContext(contextOptions))
        {
            var product = context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
            Assert.Equal(product.Name, newProduct.Name);

        }
    }


    [Theory]
    [InlineData(1)]
    public async Task DeleteCategory_ExistCategoryId_DeleteAllProducts(int categoryId)
    {
        using (var context = new TestContext(contextOptions))
        {
            var category = await context.Categories.FindAsync(categoryId);
            context.Categories.Remove(category);
            context.SaveChanges();
        }
        using (var context = new TestContext(contextOptions))
        {
            var products = await context.Products.Where(x => x.CategoryId == categoryId).ToListAsync();
            Assert.Empty(products);//Sqllite İlişkisel bir veri tabanı onun için boş olmasını bekliyorum


        }


    }
}

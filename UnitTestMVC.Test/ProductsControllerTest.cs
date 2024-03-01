using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestMVC.Web.Models;

namespace UnitTestMVC.Test;

public class ProductsControllerTest
{
    protected DbContextOptions<TestContext> contextOptions { get; private set; }
    public void SetDbContextOptions(DbContextOptions<TestContext> contextOptions)
    {
        this.contextOptions = contextOptions;
        Seeds();
    }
    public void Seeds()
    {
        using (TestContext context=new TestContext(contextOptions))
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Categories.AddRange(new Category {  Name = "Kalemler" }, new Category { Name = "Defterler" });
            context.SaveChanges();

            context.Products.Add(new Product { CategoryId = 1, Name = "Kalem10", Price = 100, Color = "red", Stock = 100 });
            context.Products.Add(new Product { CategoryId = 1, Name = "Kalem20", Price = 100, Color = "blue", Stock = 100 });
            context.SaveChanges();
        }
    }
}

using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace UnitTestMVC.Web.Models;

public partial class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public decimal? Price { get; set; }
    [Required]
    public int? Stock { get; set; }
    [Required]
    public string? Color { get; set; }
    public Category Category { get; set; }

}

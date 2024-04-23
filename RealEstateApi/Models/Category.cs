using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.Models;

public class Category
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Category name can't be null or empthy")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Image url name can't be null or empthy")]
    public string ImageUrl{ get; set; }
    public ICollection<Property> Properties { get; set; }
}

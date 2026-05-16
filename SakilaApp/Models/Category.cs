using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SakilaApp.Models;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryId { get; set; } 
    public string Name { get; set; } = string.Empty;
    public DateTime LastUpdate { get; set; } = DateTime.Now;
    public bool Active { get; set; } = true;
}
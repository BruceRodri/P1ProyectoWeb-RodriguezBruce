using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Inventory
{
    public int InventoryId { get; set; }
    public int FilmId { get; set; }
    public int StoreId { get; set; }
    public DateTime LastUpdate { get; set; }
    public virtual Film? Film { get; set; }
    public bool Active { get; set; } = true;
}
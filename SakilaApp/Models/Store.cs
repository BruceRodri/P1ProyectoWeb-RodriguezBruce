using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Store
{
    public int StoreId { get; set; }
    public int ManagerStaffId { get; set; }
    public int AddressId { get; set; }
    public DateTime LastUpdate { get; set; }
    public bool Active { get; set; } = true;
}
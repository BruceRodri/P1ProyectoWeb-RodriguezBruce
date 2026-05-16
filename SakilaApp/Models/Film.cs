using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class Film
{
    public int FilmId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ReleaseYear { get; set; } = string.Empty;
    public byte RentalDuration { get; set; }
    public decimal RentalRate { get; set; }
    public short? Length { get; set; }
    public decimal ReplacementCost { get; set; }
    public string Rating { get; set; } = string.Empty;
    public byte LanguageId { get; set; }
    public byte? OriginalLanguageId { get; set; }
    public DateTime LastUpdate { get; set; }
    public bool Active { get; set; } = true;
    public virtual ICollection<FilmActor> FilmActors { get; set; } = new
   List<FilmActor>();
}

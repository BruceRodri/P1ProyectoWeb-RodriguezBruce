using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models
{
    public class Actor
    {
        public int ActorId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime LastUpdate { get; set; }
        public bool Active { get; set; } = true;

        public virtual ICollection<FilmActor> FilmActors { get; set; } = new List<FilmActor>();
    }
}

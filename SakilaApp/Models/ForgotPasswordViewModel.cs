using System.ComponentModel.DataAnnotations;
namespace SakilaApp.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;
}
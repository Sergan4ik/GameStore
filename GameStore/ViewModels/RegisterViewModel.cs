using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GameStore.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "E-mail")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [Display(Name = "Нікнейм")]
    public string UserName { get; set; }
    [Required]
    [Display(Name = "Дата народження")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
    [Required]
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [Display(Name = "Підтвердження пароля")]
    [Compare("Password" , ErrorMessage = "Паролі не співпадають")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }
}
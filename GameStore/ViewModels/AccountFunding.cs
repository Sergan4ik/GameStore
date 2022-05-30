using System.ComponentModel.DataAnnotations;

namespace GameStore.ViewModels;

public class AccountFunding
{
    [Microsoft.Build.Framework.Required]
    public decimal Amount { get; set; }
    public string Email { get; set; }
}
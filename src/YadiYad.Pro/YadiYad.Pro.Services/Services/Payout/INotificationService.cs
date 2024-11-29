using System.Threading.Tasks;

namespace YadiYad.Pro.Services.Services.Payout
{
    // Services/INotificationService.cs
    public interface INotificationService
    {
        Task SendNotificationAsync(string recipient, string subject, string message);
    }
}

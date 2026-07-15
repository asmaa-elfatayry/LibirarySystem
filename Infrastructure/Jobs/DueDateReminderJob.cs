
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Jobs;

public class DueDateReminderJob
{
    private readonly ILoanService _loanService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    public DueDateReminderJob(
        ILoanService loanService,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender)
    {
        _loanService = loanService;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async Task SendDueDateRemindersAsync()
    {
        var loans = await _loanService.GetAllAsync();
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        var dueSoon = loans
            .Where(l => l.Status == "Active" && l.DueDate.Date == tomorrow)
            .ToList();

        foreach (var loan in dueSoon)
        {
            var user = await _userManager.FindByIdAsync(loan.MemberId.ToString());
            if (user?.Email == null) continue;

            await _emailSender.SendEmailAsync(
                user.Email,
                "تذكير: موعد إرجاع كتاب غدًا",
                $"<p>مرحبًا {user.FullName}،</p>" +
                $"<p>نود تذكيرك بأن موعد إرجاع كتاب \"<strong>{loan.BookTitle}</strong>\" هو غدًا ({loan.DueDate:dd/MM/yyyy}).</p>" +
                $"<p>يرجى إعادته في الموعد المحدد لتجنب أي غرامات تأخير.</p>");
        }
    }
}
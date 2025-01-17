namespace CityInfo.API.Services;

public class LocalMailService : IMailService
{
    private readonly string _mailFrom;
    private readonly string _mailTo;

    public LocalMailService(IConfiguration configuration)
    {
        _mailTo = configuration["mailSettings:mailToAddress"] ?? string.Empty;
        _mailFrom = configuration["mailSettings:mailFromAddress"] ?? string.Empty;
    }
    
    public void Send(string subject, string message)
    {
        // send email - output to console window
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}");
        Console.WriteLine($"\t Subject: {subject}");
        Console.WriteLine($"\t Message: {message}");
    }
}
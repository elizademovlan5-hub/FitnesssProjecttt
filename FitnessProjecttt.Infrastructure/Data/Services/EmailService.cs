using FitnessProjecttt.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FitnessProjecttt.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOtpAsync(string email, string otp)
    {
        var from = _configuration["Email:From"];
        var password = _configuration["Email:Password"];
        var host = _configuration["Email:Host"];
        var port = int.Parse(_configuration["Email:Port"]!);

        using var client = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(from, password),
            EnableSsl = true
        };

        using var message = new MailMessage(
            from!.Trim(),
            email.Trim());

        message.Subject = "FitnessProjecttt Email Verification";

        message.Body =
            $"OTP kodunuz: {otp}\n\n" +
            "Bu kod 5 deqiqe erzinde kecerlidir.";

        await client.SendMailAsync(message);
    }
}
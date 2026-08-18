using CleanMadeira.Application.Contract;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /*public async Task SendEmailAsync(
    string to,
    string subject,
    string body)
    {
        var senderEmail =
            _configuration["EmailSettings:SenderEmail"];

        var password =
            _configuration["EmailSettings:Password"];

        if (string.IsNullOrWhiteSpace(senderEmail))
        {
            _logger.LogError(
                "EmailSettings:SenderEmail não está configurado.");

            throw new InvalidOperationException(
                "EmailSettings:SenderEmail não está configurado.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogError(
                "EmailSettings:Password não está configurado.");

            throw new InvalidOperationException(
                "EmailSettings:Password não está configurado.");
        }

        try
        {
            _logger.LogInformation(
                "A tentar enviar email para {Email} através de smtp.gmail.com:587",
                to);

            using var message = new MailMessage();

            message.From = new MailAddress(
                senderEmail,
                "CleanMadeira");

            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using var smtpClient =
                new SmtpClient("smtp.gmail.com", 587);

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials =
                new NetworkCredential(
                    senderEmail,
                    password);

            await smtpClient.SendMailAsync(message);

            _logger.LogInformation(
                "Email enviado com sucesso para {Email}",
                to);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(
                ex,
                "Erro SMTP. StatusCode={StatusCode}. Destinatário={Email}",
                ex.StatusCode,
                to);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado ao enviar email para {Email}",
                to);

            throw;
        }
    }*/

    
    public async Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        var host = _configuration["Smtp:Host"]
            ?? throw new InvalidOperationException("SMTP Host em falta.");

        var portText = _configuration["Smtp:Port"]
            ?? throw new InvalidOperationException("SMTP Port em falta.");

        var fromEmail = _configuration["Smtp:FromEmail"]
            ?? throw new InvalidOperationException("SMTP FromEmail em falta.");

        var fromName = _configuration["Smtp:FromName"] ?? "CleanMadeira";

        var enableSsl =
            bool.TryParse(_configuration["Smtp:EnableSsl"], out var ssl)
                && ssl;

        var username = _configuration["Smtp:Username"];
        var password = _configuration["Smtp:Password"];

        using var client = new SmtpClient(host, int.Parse(portText))
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = string.IsNullOrWhiteSpace(username)
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            client.Credentials = new NetworkCredential(
                username,
                password);
        }

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await client.SendMailAsync(message);
    }

    public async Task SendConfirmationEmailAsync(
    ApplicationUser user,
    string confirmationLink)
    {
        var safeName = WebUtility.HtmlEncode(
            user.PrimeiroNome ?? "utilizador");

        var safeLink = WebUtility.HtmlEncode(confirmationLink);

        var roleContent = user.Role == UserRole.Limpador || user.Role == UserRole.GestorELimpador
            ? """
          <li>📅 Consultar as limpezas que lhe forem atribuídas</li>
          <li>🏠 Ver os detalhes das propriedades</li>
          <li>✅ Atualizar o estado das tarefas</li>
          <li>📷 Adicionar fotografias após a limpeza</li>
          """
            : """
          <li>🏠 Gerir os seus alojamentos</li>
          <li>📅 Criar e acompanhar limpezas</li>
          <li>👥 Atribuir tarefas a limpadores</li>
          <li>📦 Controlar o inventário das propriedades</li>
          """;

        var body = $"""
    <!DOCTYPE html>
    <html lang="pt">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport"
              content="width=device-width, initial-scale=1.0">
        <title>Confirmar conta</title>
    </head>

    <body style="
        margin:0;
        background:#eef6ff;
        font-family:Arial,Helvetica,sans-serif;">

        <table role="presentation"
               width="100%"
               cellpadding="0"
               cellspacing="0"
               style="padding:40px 15px;background:#eef6ff;">

            <tr>
                <td align="center">

                    <table role="presentation"
                           width="100%"
                           cellpadding="0"
                           cellspacing="0"
                           style="
                               max-width:600px;
                               background:#ffffff;
                               border-radius:18px;
                               overflow:hidden;
                               box-shadow:0 10px 30px rgba(0,0,0,.08);">

                        <tr>
                            <td style="
                                background:#2563eb;
                                padding:38px;
                                text-align:center;">

                                <div style="font-size:48px;">🏠</div>

                                <h1 style="
                                    margin:12px 0 0;
                                    color:#ffffff;">
                                    CleanMadeira
                                </h1>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:38px;">

                                <h2 style="color:#1e293b;">
                                    Olá, {safeName}!
                                </h2>

                                <p style="
                                    color:#475569;
                                    font-size:16px;
                                    line-height:1.7;">
                                    Obrigado por criar a sua conta no
                                    CleanMadeira. Para começar, confirme o seu
                                    endereço de email.
                                </p>

                                <div style="
                                    text-align:center;
                                    margin:34px 0;">

                                    <a href="{safeLink}"
                                       style="
                                           display:inline-block;
                                           background:#2563eb;
                                           color:#ffffff;
                                           padding:15px 30px;
                                           border-radius:9px;
                                           text-decoration:none;
                                           font-weight:bold;">
                                        Confirmar conta
                                    </a>

                                </div>

                                <div style="
                                    background:#f8fafc;
                                    border-left:4px solid #2563eb;
                                    padding:18px;
                                    border-radius:8px;">

                                    <p style="
                                        margin-top:0;
                                        color:#334155;
                                        font-weight:bold;">
                                        Depois de confirmar poderá:
                                    </p>

                                    <ul style="
                                        margin-bottom:0;
                                        color:#475569;
                                        line-height:2;">
                                        {roleContent}
                                    </ul>

                                </div>

                                <p style="
                                    margin-top:28px;
                                    color:#64748b;
                                    font-size:13px;">
                                    Caso não tenha criado esta conta, pode
                                    ignorar esta mensagem.
                                </p>

                            </td>
                        </tr>

                    </table>

                </td>
            </tr>

        </table>
    </body>
    </html>
    """;

        await SendEmailAsync(
            user.Email!,
            "Confirme a sua conta CleanMadeira",
            body);
    }

    public async Task SendWelcomeEmailAsync(
    ApplicationUser user,
    string loginLink)
    {
        var safeName = WebUtility.HtmlEncode(
            user.PrimeiroNome ?? "utilizador");

        var safeLink = WebUtility.HtmlEncode(loginLink);

        var title = user.Role == UserRole.Limpador || user.Role == UserRole.GestorELimpador
            ? "A sua área de trabalho está pronta"
            : "A gestão dos seus alojamentos começa aqui";

        var description = user.Role == UserRole.Limpador || user.Role == UserRole.GestorELimpador
            ? """
          Já pode consultar as limpezas que lhe forem atribuídas,
          verificar os detalhes das propriedades e atualizar o progresso
          das tarefas.
          """
            : """
          Já pode adicionar propriedades, criar limpezas, atribuir
          tarefas e acompanhar a operação dos seus alojamentos.
          """;

        var body = $"""
    <!DOCTYPE html>
    <html lang="pt">
    <body style="
        margin:0;
        background:#ecfdf5;
        font-family:Arial,Helvetica,sans-serif;">

        <table role="presentation"
               width="100%"
               cellpadding="0"
               cellspacing="0"
               style="padding:40px 15px;">

            <tr>
                <td align="center">

                    <table role="presentation"
                           width="100%"
                           cellpadding="0"

                           cellspacing="0"
                           style="
                               max-width:600px;
                               background:#ffffff;
                               border-radius:16px;
                               overflow:hidden;">

                        <tr>
                            <td style="
                                background:#047857;
                                color:#ffffff;
                                text-align:center;
                                padding:36px;">

                                <div style="font-size:48px;">🎉</div>

                                <h1 style="margin-bottom:6px;">
                                    Bem-vindo ao CleanMadeira
                                </h1>

                                <p style="margin:0;color:#d1fae5;">
                                    A sua conta foi confirmada
                                </p>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:36px;">

                                <h2 style="color:#065f46;">
                                    Olá, {safeName}
                                </h2>

                                <h3 style="color:#1f2937;">
                                    {title}
                                </h3>

                                <p style="
                                    color:#4b5563;
                                    font-size:16px;
                                    line-height:1.7;">
                                    {description}
                                </p>

                                <div style="
                                    text-align:center;
                                    margin-top:30px;">

                                    <a href="{safeLink}"
                                       style="
                                           display:inline-block;
                                           background:#059669;
                                           color:#ffffff;
                                           padding:14px 28px;
                                           border-radius:999px;
                                           text-decoration:none;
                                           font-weight:bold;">
                                        Entrar no CleanMadeira
                                    </a>
                                </div>

                            </td>
                        </tr>

                    </table>

                </td>
            </tr>

        </table>
    </body>
    </html>
    """;

        await SendEmailAsync(
            user.Email!,
            "Bem-vindo ao CleanMadeira",
            body);
    }

    public async Task SendResetPasswordEmailAsync(
    ApplicationUser user,
    string resetLink)
    {
        var safeName = System.Net.WebUtility.HtmlEncode(
            user.PrimeiroNome ?? "utilizador");

        var safeLink = System.Net.WebUtility.HtmlEncode(resetLink);

        var body = $"""
    <!DOCTYPE html>
    <html lang="pt">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Redefinir palavra-passe</title>
    </head>

    <body style="
        margin:0;
        padding:0;
        background-color:#eef2f7;
        font-family:Arial,Helvetica,sans-serif;
        color:#1f2937;">

        <table role="presentation"
               width="100%"
               cellspacing="0"
               cellpadding="0"
               border="0"
               style="background-color:#eef2f7;padding:32px 12px;">

            <tr>
                <td align="center">

                    <table role="presentation"
                           width="100%"
                           cellspacing="0"
                           cellpadding="0"
                           border="0"
                           style="
                               max-width:600px;
                               background-color:#ffffff;
                               border-radius:14px;
                               overflow:hidden;
                               box-shadow:0 8px 24px rgba(15,23,42,0.08);">

                        <tr>
                            <td style="
                                background-color:#172554;
                                padding:28px 32px;
                                text-align:center;">

                                <div style="
                                    font-size:14px;
                                    letter-spacing:2px;
                                    text-transform:uppercase;
                                    color:#bfdbfe;
                                    margin-bottom:8px;">
                                    CleanMadeira
                                </div>

                                <h1 style="
                                    margin:0;
                                    color:#ffffff;
                                    font-size:26px;
                                    line-height:1.3;">
                                    Segurança da conta
                                </h1>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:36px 32px;">

                                <div style="
                                    width:58px;
                                    height:58px;
                                    line-height:58px;
                                    text-align:center;
                                    border-radius:50%;
                                    background-color:#dbeafe;
                                    font-size:28px;
                                    margin:0 auto 22px;">
                                    🔐
                                </div>

                                <h2 style="
                                    margin:0 0 16px;
                                    text-align:center;
                                    color:#0f172a;
                                    font-size:24px;">
                                    Redefina a sua palavra-passe
                                </h2>

                                <p style="
                                    margin:0 0 14px;
                                    font-size:16px;
                                    line-height:1.7;">
                                    Olá <strong>{safeName}</strong>,
                                </p>

                                <p style="
                                    margin:0 0 24px;
                                    font-size:16px;
                                    line-height:1.7;
                                    color:#475569;">
                                    Recebemos um pedido para alterar a
                                    palavra-passe da sua conta CleanMadeira.
                                </p>

                                <div style="text-align:center;margin:30px 0;">

                                    <a href="{safeLink}"
                                       style="
                                           display:inline-block;
                                           background-color:#2563eb;
                                           color:#ffffff;
                                           padding:14px 26px;
                                           border-radius:8px;
                                           text-decoration:none;
                                           font-size:16px;
                                           font-weight:700;">
                                        Criar nova palavra-passe
                                    </a>

                                </div>

                                <div style="
                                    background-color:#f8fafc;
                                    border-left:4px solid #3b82f6;
                                    padding:16px 18px;
                                    border-radius:6px;
                                    margin-top:26px;">

                                    <p style="
                                        margin:0;
                                        color:#475569;
                                        font-size:14px;
                                        line-height:1.6;">
                                        Não pediu esta alteração? Pode ignorar
                                        este email. A sua palavra-passe atual
                                        continuará válida.
                                    </p>

                                </div>

                                <p style="
                                    margin:28px 0 0;
                                    color:#64748b;
                                    font-size:13px;
                                    line-height:1.6;">
                                    Por motivos de segurança, não partilhe este
                                    link com outras pessoas.
                                </p>

                            </td>
                        </tr>

                        <tr>
                            <td style="
                                background-color:#f8fafc;
                                padding:20px 32px;
                                text-align:center;
                                color:#94a3b8;
                                font-size:12px;">
                                Email automático do CleanMadeira.<br>
                                Não responda a esta mensagem.
                            </td>
                        </tr>

                    </table>

                </td>
            </tr>
        </table>
    </body>
    </html>
    """;

        await SendEmailAsync(
            user.Email!,
            "Redefinir a sua palavra-passe",
            body);
    }

    public async Task SendCleaningAssignedEmailAsync(
    ApplicationUser cleaner,
    CleaningTask task,
    Property property,
    string taskDetailsLink)
    {
        var safeCleanerName = System.Net.WebUtility.HtmlEncode(
            cleaner.PrimeiroNome ?? "utilizador");

        var safePropertyName = System.Net.WebUtility.HtmlEncode(
            property.Name ?? "Propriedade");

        var safeAddress = System.Net.WebUtility.HtmlEncode(
            property.Address ?? "Morada não indicada");

        var safePriority = System.Net.WebUtility.HtmlEncode(
            task.Priority.ToString());

        var safeLink = System.Net.WebUtility.HtmlEncode(taskDetailsLink);

        var body = $"""
    <!DOCTYPE html>
    <html lang="pt">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Nova limpeza atribuída</title>
    </head>

    <body style="
        margin:0;
        padding:0;
        background-color:#fff7ed;
        font-family:Arial,Helvetica,sans-serif;
        color:#1f2937;">

        <table role="presentation"
               width="100%"
               cellspacing="0"
               cellpadding="0"
               border="0"
               style="background-color:#fff7ed;padding:32px 12px;">

            <tr>
                <td align="center">

                    <table role="presentation"
                           width="100%"
                           cellspacing="0"
                           cellpadding="0"
                           border="0"
                           style="
                               max-width:620px;
                               background-color:#ffffff;
                               border-radius:14px;
                               overflow:hidden;
                               box-shadow:0 8px 24px rgba(154,52,18,0.10);">

                        <tr>
                            <td style="
                                background-color:#c2410c;
                                padding:26px 32px;">

                                <table role="presentation"
                                       width="100%"
                                       cellspacing="0"
                                       cellpadding="0"
                                       border="0">

                                    <tr>
                                        <td>
                                            <div style="
                                                color:#fed7aa;
                                                font-size:13px;
                                                text-transform:uppercase;
                                                letter-spacing:1.5px;">
                                                Nova atribuição
                                            </div>

                                            <h1 style="
                                                margin:6px 0 0;
                                                color:#ffffff;
                                                font-size:26px;">
                                                Limpeza agendada
                                            </h1>
                                        </td>

                                        <td align="right"
                                            style="font-size:42px;">
                                            🧹
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding:32px;">

                                <p style="
                                    margin:0 0 20px;
                                    font-size:16px;
                                    line-height:1.7;">
                                    Olá <strong>{safeCleanerName}</strong>,
                                    foi-lhe atribuída uma nova tarefa de limpeza.
                                </p>

                                <table role="presentation"
                                       width="100%"
                                       cellspacing="0"
                                       cellpadding="0"
                                       border="0"
                                       style="
                                           border:1px solid #fed7aa;
                                           border-radius:10px;
                                           overflow:hidden;">

                                    <tr>
                                        <td style="
                                            width:38%;
                                            padding:14px 16px;
                                            background-color:#fff7ed;
                                            color:#9a3412;
                                            font-size:14px;
                                            font-weight:700;
                                            border-bottom:1px solid #fed7aa;">
                                            🏠 Propriedade
                                        </td>

                                        <td style="
                                            padding:14px 16px;
                                            font-size:15px;
                                            border-bottom:1px solid #fed7aa;">
                                            {safePropertyName}
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="
                                            padding:14px 16px;
                                            background-color:#fff7ed;
                                            color:#9a3412;
                                            font-size:14px;
                                            font-weight:700;
                                            border-bottom:1px solid #fed7aa;">
                                            📍 Morada
                                        </td>

                                        <td style="
                                            padding:14px 16px;
                                            font-size:15px;
                                            border-bottom:1px solid #fed7aa;">
                                            {safeAddress}
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="
                                            padding:14px 16px;
                                            background-color:#fff7ed;
                                            color:#9a3412;
                                            font-size:14px;
                                            font-weight:700;
                                            border-bottom:1px solid #fed7aa;">
                                            📅 Data
                                        </td>

                                        <td style="
                                            padding:14px 16px;
                                            font-size:15px;
                                            border-bottom:1px solid #fed7aa;">
                                            {task.ScheduledDate:dd/MM/yyyy}
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="
                                            padding:14px 16px;
                                            background-color:#fff7ed;
                                            color:#9a3412;
                                            font-size:14px;
                                            font-weight:700;
                                            border-bottom:1px solid #fed7aa;">
                                            ⏰ Hora
                                        </td>

                                        <td style="
                                            padding:14px 16px;
                                            font-size:15px;
                                            border-bottom:1px solid #fed7aa;">
                                            {task.ScheduledDate:HH:mm}
                                        </td>
                                    </tr>

                                    <tr>
                                        <td style="
                                            padding:14px 16px;
                                            background-color:#fff7ed;
                                            color:#9a3412;
                                            font-size:14px;
                                            font-weight:700;">
                                            ⚑ Prioridade
                                        </td>

                                        <td style="
                                            padding:14px 16px;
                                            font-size:15px;">
                                            {safePriority}
                                        </td>
                                    </tr>

                                </table>

                                <div style="
                                    margin:24px 0;
                                    padding:15px 18px;
                                    background-color:#fffbeb;
                                    border-radius:8px;
                                    color:#92400e;
                                    font-size:14px;
                                    line-height:1.6;">
                                    Confirme os detalhes da tarefa antes da
                                    deslocação à propriedade.
                                </div>

                                <div style="text-align:center;">

                                    <a href="{safeLink}"
                                       style="
                                           display:inline-block;
                                           background-color:#ea580c;
                                           color:#ffffff;
                                           padding:14px 28px;
                                           border-radius:8px;
                                           text-decoration:none;
                                           font-size:16px;
                                           font-weight:700;">
                                        Ver detalhes da limpeza
                                    </a>

                                </div>

                            </td>
                        </tr>

                        <tr>
                            <td style="
                                background-color:#fff7ed;
                                padding:20px 30px;
                                text-align:center;
                                color:#9a3412;
                                font-size:12px;">
                                Notificação automática do CleanMadeira
                            </td>
                        </tr>

                    </table>

                </td>
            </tr>
        </table>
    </body>
    </html>
    """;

        await SendEmailAsync(
            cleaner.Email!,
            $"Nova limpeza: {safePropertyName}",
            body);
    }



}
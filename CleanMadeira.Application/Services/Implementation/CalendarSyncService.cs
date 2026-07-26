using CleanMadeira.Application.Common.Interfaces;
using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Models;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.Extensions.Configuration;
using IcalCalendar = Ical.Net.Calendar;

namespace CleanMadeira.Application.Services;

public class CalendarSyncService : ICalendarSyncService
{
    private readonly HttpClient _httpClient;

    private readonly ICalendarIntegrationRepository
        _calendarIntegrationRepository;

    private readonly IReservationRepository
        _reservationRepository;

    private readonly ICleaningTaskRepository
        _cleaningTaskRepository;

    private readonly IPropertyRepository
        _propertyRepository;

    private readonly IEmailService
        _emailService;

    private readonly IConfiguration 
        _configuration;

    public CalendarSyncService(
        HttpClient httpClient,
        ICalendarIntegrationRepository calendarIntegrationRepository,
        IReservationRepository reservationRepository,
        ICleaningTaskRepository cleaningTaskRepository,
        IPropertyRepository propertyRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _calendarIntegrationRepository = calendarIntegrationRepository;
        _reservationRepository = reservationRepository;
        _cleaningTaskRepository = cleaningTaskRepository;
        _propertyRepository = propertyRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<CalendarSyncResult> SyncAsync(
        Guid calendarIntegrationId,
        CancellationToken cancellationToken = default)
    {
        var result = new CalendarSyncResult();

        try
        {
            var integration = await _calendarIntegrationRepository
                .GetByIdAsync(calendarIntegrationId);

            if (integration is null)
            {
                result.Message = "A integração de calendário não foi encontrada.";
                return result;
            }

            if (!integration.IsEnabled)
            {
                result.Message = "A integração de calendário está desativada.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(integration.CalendarUrl))
            {
                result.Message = "A integração não possui um URL iCal.";
                return result;
            }

            if (!Uri.TryCreate(
                    integration.CalendarUrl,
                    UriKind.Absolute,
                    out var calendarUri))
            {
                result.Message = "O URL do calendário não é válido.";
                return result;
            }

            if (calendarUri.Scheme != Uri.UriSchemeHttp &&
                calendarUri.Scheme != Uri.UriSchemeHttps)
            {
                result.Message = "O URL deve utilizar HTTP ou HTTPS.";
                return result;
            }

            var icalContent = await DownloadCalendarAsync(
                calendarUri,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(icalContent))
            {
                result.Message = "O calendário descarregado está vazio.";
                return result;
            }

            IcalCalendar calendar;

            try
            {
                calendar = IcalCalendar.Load(icalContent);
            }
            catch (Exception ex)
            {
                result.Message =
                    $"Não foi possível interpretar o calendário iCal: {ex.Message}";

                return result;
            }

            if (calendar.Events is null || calendar.Events.Count == 0)
            {
                integration.LastSync = DateTime.UtcNow;

                await _calendarIntegrationRepository.SaveChangesAsync();

                result.Success = true;
                result.Message = "O calendário não contém reservas.";

                return result;
            }

            foreach (var calendarEvent in calendar.Events)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await ProcessEventAsync(
                    integration,
                    calendarEvent,
                    result,
                    cancellationToken);
            }

            integration.LastSync = DateTime.UtcNow;

            /*
             * Este SaveChanges guarda as reservas modificadas.
             *
             * Se os três repositórios utilizarem a mesma instância scoped do
             * ApplicationDbContext, também guarda as CleaningTasks adicionadas
             * ao contexto.
             */
            await _reservationRepository.SaveChangesAsync();

            /*
             * Mantém esta chamada caso o teu repositório de integração utilize
             * um SaveChanges próprio.
             */
            await _calendarIntegrationRepository.SaveChangesAsync();

            result.Success = true;

            result.Message =
                $"Sincronização concluída. " +
                $"{result.ReservationsCreated} reserva(s) criada(s), " +
                $"{result.ReservationsUpdated} atualizada(s) e " +
                $"{result.CleaningTasksCreated} limpeza(s) criada(s).";

            return result;
        }
        catch (OperationCanceledException)
        {
            result.Message = "A sincronização foi cancelada.";
            return result;
        }
        catch (HttpRequestException ex)
        {
            result.Message =
                $"Não foi possível descarregar o calendário: {ex.Message}";

            return result;
        }
        catch (Exception ex)
        {
            result.Message =
                $"Ocorreu um erro durante a sincronização: {ex.Message}";

            return result;
        }
    }

    private async Task ProcessEventAsync(
    CalendarIntegration integration,
    CalendarEvent calendarEvent,
    CalendarSyncResult result,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(calendarEvent.Uid))
            return;

        if (calendarEvent.DtStart is null ||
            calendarEvent.DtEnd is null)
        {
            return;
        }

        var checkIn = ConvertCalendarDate(calendarEvent.DtStart);
        var checkOut = ConvertCalendarDate(calendarEvent.DtEnd); ;

        if (checkOut <= checkIn)
            return;

        var reservation = await _reservationRepository
            .GetByExternalUidAsync(
                integration.Id,
                calendarEvent.Uid);

        if (reservation is null)
        {
            reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                PropertyId = integration.PropertyId,
                CalendarIntegrationId = integration.Id,
                ExternalUid = calendarEvent.Uid,
                Summary = calendarEvent.Summary,
                CheckIn = checkIn,
                CheckOut = checkOut,
                IsCancelled = IsCancelled(calendarEvent),
                LastSyncedAt = DateTime.UtcNow
            };

            await _reservationRepository.AddAsync(
                reservation);

            result.ReservationsCreated++;
        }
        else
        {
            reservation.Summary = calendarEvent.Summary;
            reservation.CheckIn = checkIn;
            reservation.CheckOut = checkOut;
            reservation.IsCancelled = IsCancelled(calendarEvent);
            reservation.LastSyncedAt = DateTime.UtcNow;

            result.ReservationsUpdated++;
        }

        if (reservation.IsCancelled)
            return;

        await CreateOrUpdateCleaningTaskAsync(
            reservation,
            checkOut,
            result,
            cancellationToken);
    }

    private static DateTime ConvertCalendarDate(
    Ical.Net.DataTypes.CalDateTime date)
    {
        if (!date.HasTime)
            return date.Value.Date;

        if (date.IsUtc)
            return date.AsUtc.ToLocalTime();

        return date.Value;
    }

    private async Task CreateOrUpdateCleaningTaskAsync(
        Reservation reservation,
        DateTime checkOut,
        CalendarSyncResult result,
        CancellationToken cancellationToken)
    {
        /*
         * Já existe uma limpeza associada à reserva.
         */
        if (reservation.CleaningTaskId.HasValue)
        {
            var existingCleaningTask =
                reservation.CleaningTask;

            /*
             * O Include pode não ter carregado a CleaningTask.
             * Nesse caso, procuramos diretamente pelo ID.
             */
            if (existingCleaningTask is null)
            {
                existingCleaningTask =
                    await _cleaningTaskRepository.GetByIdAsync(
                        reservation.CleaningTaskId.Value);
            }

            if (existingCleaningTask is null)
                return;

            /*
             * Se o checkout mudar no calendário externo,
             * atualiza a data da limpeza enquanto ela ainda
             * não estiver concluída.
             */
            if (existingCleaningTask.Status !=
                CleaningStatus.Completo)
            {
                existingCleaningTask.ScheduledDate = checkOut;
            }

            return;
        }

        var cleaningTask = new CleaningTask
        {
            Id = Guid.NewGuid(),

            PropertyId = reservation.PropertyId,

            ScheduledDate = checkOut,

            Status = CleaningStatus.Pendente,

            Priority = TaskPriority.Normal,
        };

        await _cleaningTaskRepository.AddAsync(
            cleaningTask);

        try
        {
            await EnviarEmailLimpezaAutomaticaAsync(
                cleaningTask,
                reservation);
        }
        catch (Exception ex)
        {
            // O erro no email não deve apagar nem impedir a limpeza.
            // Idealmente usa ILogger.
            Console.WriteLine(
                $"Erro ao enviar email da limpeza automática: {ex.Message}");
        }

        reservation.CleaningTaskId = cleaningTask.Id;
        reservation.CleaningTask = cleaningTask;

        result.CleaningTasksCreated++;
    }

    private async Task<string> DownloadCalendarAsync(
        Uri calendarUri,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            calendarUri);

        request.Headers.UserAgent.ParseAdd(
            "CleanMadeira-CalendarSync/1.0");

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(
            cancellationToken);
    }

    private static bool IsCancelled(
        CalendarEvent calendarEvent)
    {
        /*
         * Alguns fornecedores marcam eventos cancelados com STATUS:CANCELLED.
         */
        var status = calendarEvent.Status?.ToString();

        if (string.Equals(
                status,
                "CANCELLED",
                StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        /*
         * Alguns feeds colocam a informação no título.
         */
        var summary = calendarEvent.Summary ?? string.Empty;

        return summary.Contains(
                   "cancelled",
                   StringComparison.OrdinalIgnoreCase)
               ||
               summary.Contains(
                   "canceled",
                   StringComparison.OrdinalIgnoreCase)
               ||
               summary.Contains(
                   "cancelada",
                   StringComparison.OrdinalIgnoreCase);
    }

    private async Task EnviarEmailLimpezaAutomaticaAsync(
    CleaningTask cleaningTask,
    Reservation reservation)
    {
        var propriedade = await _propertyRepository
            .GetByIdWithOwnerAsync(cleaningTask.PropertyId);

        if (propriedade == null)
            return;

        var emailDestinatario = propriedade.ApplicationUser?.Email;

        if (string.IsNullOrWhiteSpace(emailDestinatario))
            return;

        var nomeUtilizador =
            propriedade.ApplicationUser?.PrimeiroNome ?? "utilizador";
        
        var nomePropriedade =
            propriedade.Name ?? "Propriedade";

        var baseUrl = _configuration["Application:BaseUrl"];

        var linkTarefa = string.IsNullOrWhiteSpace(baseUrl)
            ? string.Empty
            : $"{baseUrl.TrimEnd('/')}/CleaningTask/Edit/{cleaningTask.Id}";

        var assunto =
            $"Nova limpeza automática — {nomePropriedade}";

        var corpoHtml = $"""
        <!DOCTYPE html>
        <html lang="pt">
        <head>
            <meta charset="UTF-8">
        </head>

        <body style="
            margin:0;
            padding:0;
            background-color:#f5f7fa;
            font-family:Arial, Helvetica, sans-serif;
            color:#212529;">

            <div style="
                max-width:600px;
                margin:30px auto;
                background-color:#ffffff;
                border-radius:12px;
                overflow:hidden;
                box-shadow:0 4px 16px rgba(0,0,0,0.08);">

                <div style="
                    background-color:#198754;
                    color:#ffffff;
                    padding:24px 30px;">

                    <h1 style="
                        margin:0;
                        font-size:24px;">

                        CleanMadeira
                    </h1>

                    <p style="
                        margin:8px 0 0;
                        opacity:0.9;">

                        Nova limpeza criada automaticamente
                    </p>
                </div>

                <div style="padding:30px;">

                    <p style="font-size:16px;">
                        Olá, {System.Net.WebUtility.HtmlEncode(nomeUtilizador)}.
                    </p>

                    <p style="
                        font-size:16px;
                        line-height:1.6;">

                        Foi criada automaticamente uma tarefa de limpeza
                        após a deteção de uma nova reserva no calendário.
                    </p>

                    <div style="
                        background-color:#f8f9fa;
                        border-left:4px solid #198754;
                        border-radius:6px;
                        padding:18px;
                        margin:24px 0;">

                        <p style="margin:0 0 10px;">
                            <strong>Propriedade:</strong>
                            {System.Net.WebUtility.HtmlEncode(nomePropriedade)}
                        </p>

                        <p style="margin:0 0 10px;">
                            <strong>Check-in:</strong>
                            {reservation.CheckIn:dd/MM/yyyy HH:mm}
                        </p>

                        <p style="margin:0 0 10px;">
                            <strong>Check-out:</strong>
                            {reservation.CheckOut:dd/MM/yyyy HH:mm}
                        </p>

                        <p style="margin:0 0 10px;">
                            <strong>Data da limpeza:</strong>
                            {cleaningTask.ScheduledDate:dd/MM/yyyy HH:mm}
                        </p>

                        <p style="margin:0;">
                            <strong>Estado:</strong>
                            Pendente
                        </p>
                    </div>

                    <p style="
                        font-size:16px;
                        line-height:1.6;">

                        Esta tarefa ainda não tem um limpador atribuído.
                        Gostaria de escolher um limpador para realizar esta limpeza?
                    </p>

                    {CriarBotaoEmail(linkTarefa)}

                    <p style="
                        margin-top:28px;
                        font-size:13px;
                        color:#6c757d;">

                        Este email foi enviado automaticamente pelo CleanMadeira.
                    </p>
                </div>
            </div>
        </body>
        </html>
        """;

        await _emailService.SendEmailAsync(
            emailDestinatario,
            assunto,
            corpoHtml);
    }

    private static string CriarBotaoEmail(string linkTarefa)
    {
        if (string.IsNullOrWhiteSpace(linkTarefa))
        {
            return """
            <p style="
                font-size:14px;
                color:#6c757d;
                margin-top:22px;">

                Entre no CleanMadeira para atribuir um limpador à tarefa.
            </p>
            """;
        }

        var linkSeguro =
            System.Net.WebUtility.HtmlEncode(linkTarefa);

        return $"""
        <div style="
            text-align:center;
            margin-top:28px;">

            <a href="{linkSeguro}"
               style="
                   display:inline-block;
                   background-color:#198754;
                   color:#ffffff;
                   text-decoration:none;
                   padding:13px 24px;
                   border-radius:7px;
                   font-size:16px;
                   font-weight:bold;">

                Atribuir limpador
            </a>
        </div>
        """;
    }
}
using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Enums;
using CleanMadeira.Web.ViewModels.CleaningTask;
using ImageMagick;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CleanMadeira.Web.Controllers;

public class CleaningTaskController : Controller
{
    private readonly ICleaningTaskService _cleaningTaskService;
    private readonly IPropertyService _propertyService;
    private readonly IUtilizadorService _utilizadorService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailService _emailService;

    public CleaningTaskController(
        ICleaningTaskService cleaningTaskService,
        IPropertyService propertyService,
        IUtilizadorService utilizadorService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment webHostEnvironment,
        IEmailService emailService)
    {
        _cleaningTaskService = cleaningTaskService;
        _propertyService = propertyService;
        _utilizadorService = utilizadorService;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {

        await LoadDropdowns();

        return View(new CreateCleaningTaskVM());
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _userManager.GetUserAsync(User);

        var task = await _cleaningTaskService.GetByIdAsync(id);

        if (user.Role == UserRole.Dono)
        {
            task = await _cleaningTaskService.GetByIdAndOwnerIdAsync(id, userId);
        }
        else
        {
            task = await _cleaningTaskService.GetByIdAndCleanerIdAsync(id, userId);
        }


        if (task == null)
            return NotFound();

        var vm = new CleaningTaskVM
        {
            Id = task.Id,

            PropriedadeNome = task.Property?.Name ?? "",
            TipoServico = task.CleaningType,
            Morada = task.Property?.Address ?? "",
            Freguesia = task.Property?.Freguesia ?? "",
            GestorNome = task.Property?.ApplicationUser != null
                 ? $"{task.Property.ApplicationUser.PrimeiroNome} {task.Property.ApplicationUser.UltimoNome}"
                  : "",

            GestorTelefone = task.Property?.ApplicationUser.PhoneNumber,

            AssignedUserName = task.AssignedUser != null
                ? $"{task.AssignedUser.PrimeiroNome} {task.AssignedUser.UltimoNome}"
                : "Não atribuído",
            AssignedUserPhone = task.AssignedUser?.PhoneNumber,
            AssignedUserCode = task.AssignedUser?.CleanerCode,

            Latitude = task.Property?.Latitude,
            Longitude = task.Property?.Longitude,

            ScheduledDate = task.ScheduledDate,

            Status = task.Status,
            Prioridade = task.Priority,

            EstimatedMinutes = task.EstimatedMinutes,

            Notas = task.Notes,

            CleanerNotes = task.CleanerNotes,

            Photos = task.Photos
                .Select(p => new CleaningPhotoVM
                {
                    Id = p.Id,
                    FileUrl = p.FileUrl,
                    FileName = p.FileName
                })
                .ToList(),

            StartTime = task.StartTime
        };

        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var propriedades = await _propertyService.GetByUserAsync(user.Id);

        ViewBag.Propriedades = new SelectList(
            propriedades,
            "Id",
            "Name");

        var limpadores = await _userManager.Users
        .Where(u => u.Role == UserRole.Limpador)
        .OrderBy(u => u.PrimeiroNome)
        .ToListAsync();

        ViewBag.Limpadores = new SelectList(
            limpadores,
            "Id",
            "FirstName");



        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCleaningTaskVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = await _userManager.GetUserAsync(User);

        var property = await _propertyService.GetByIdAndOwnerIdAsync(vm.PropriedadeId, user.Id);

        var task = new CleaningTask
        {
            Id = Guid.NewGuid(),
            PropertyId = vm.PropriedadeId,
            CleaningType = vm.TipoServico,
            AssignedUserId = vm.AssignedUserId,
            ScheduledDate = vm.ScheduledDate,
            Priority = vm.Prioridade,
            Status = CleaningStatus.Pendente,
            EstimatedMinutes = vm.EstimatedMinutes,
            CleaningCompanyId = property?.CleaningCompanyId,
            Notes = vm.Notas
        };

        await _cleaningTaskService.CreateAsync(task);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService.GetByIdAndOwnerIdAsync(id, userId);

        if (task == null)
            return NotFound();

        await LoadDropdowns();

        var vm = new EditCleaningTaskVM
        {
            Id = task.Id,
            PropriedadeId = task.PropertyId,
            TipoServico = task.CleaningType,
            AssignedUserId = task.AssignedUserId,
            ScheduledDate = task.ScheduledDate,
            Prioridade = task.Priority,
            Status = (TaskStatus)task.Status,
            EstimatedMinutes = task.EstimatedMinutes,
            Notas = task.Notes
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCleaningTaskVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService.GetByIdAndOwnerIdAsync(vm.Id, userId);

        if (task == null)
            return NotFound();

        task.PropertyId = vm.PropriedadeId;
        task.CleaningType = vm.TipoServico;
        task.ScheduledDate = vm.ScheduledDate;
        task.Priority = vm.Prioridade;
        task.Status = (CleaningStatus)vm.Status;
        task.EstimatedMinutes = vm.EstimatedMinutes;
        task.Notes = vm.Notas;

        await _cleaningTaskService.UpdateAsync(task);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService.GetByIdAndOwnerIdAsync(id, userId);

        if (task == null)
            return NotFound();

        var vm = new CleaningTaskVM
        {
            Id = task.Id,
            PropriedadeNome = task.Property?.Name ?? "",
            AssignedUserName = task.AssignedUser != null
                ? $"{task.AssignedUser.PrimeiroNome} {task.AssignedUser.UltimoNome}"
                : "Não atribuído",
            ScheduledDate = task.ScheduledDate,
            Status = task.Status,
            Prioridade = task.Priority
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService.GetByIdAndOwnerIdAsync(id, userId);

        if (task == null)
            return NotFound();

        if (task.Status != CleaningStatus.Pendente)
        {
            TempData["Error"] = "Só pode eliminar limpezas pendentes. Limpezas em progresso ou concluídas devem ser preservadas.";
            return RedirectToAction(nameof(Index));
        }

        await _cleaningTaskService.DeleteAsync(id);

        TempData["Success"] = "Limpeza eliminada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MyTasks(
    string status = "todas",
    string periodo = "mes")
    {
        var user = await _userManager.GetUserAsync(User);


        var tasks = await _cleaningTaskService
            .GetByLimpadorUserIdAsync(user.Id);

        var query = tasks.AsQueryable();

        if (status == "emprogresso")
            query = query.Where(t => t.Status == CleaningStatus.EmProgresso);

        if (status == "pendentes")
            query = query.Where(t => t.Status == CleaningStatus.Pendente);

        if (status == "concluidas")
            query = query.Where(t => t.Status == CleaningStatus.Completo);

        var hoje = DateTime.Today;

        if (periodo == "hoje")
            query = query.Where(t => t.ScheduledDate.Date == hoje);

        if (periodo == "semana")
            query = query.Where(t => t.ScheduledDate >= hoje &&
                                     t.ScheduledDate < hoje.AddDays(7));

        if (periodo == "mes")
            query = query.Where(t => t.ScheduledDate >= hoje &&
                                     t.ScheduledDate < hoje.AddMonths(1));

        var vm = query
            .OrderBy(t => t.Status == CleaningStatus.EmProgresso ? 0 :
                          t.Status == CleaningStatus.Pendente ? 1 :
                          t.Status == CleaningStatus.Completo ? 2 : 3)
            .ThenBy(t => t.ScheduledDate)
            .Select(t => new CleaningTaskVM
            {
                Id = t.Id,
                PropriedadeNome = t.Property.Name,
                ScheduledDate = t.ScheduledDate,
                Status = t.Status,
                Prioridade = t.Priority
            })
            .ToList();

        ViewBag.Status = status;
        ViewBag.Periodo = periodo;

        return View(vm);
    }

    public async Task<IActionResult> Start(Guid id)
    {
        var task = await _cleaningTaskService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        task.Status = CleaningStatus.EmProgresso;
        task.StartedAt = DateTime.Now;
        task.StartTime = DateTime.Now;

        await _cleaningTaskService.UpdateAsync(task);

        return RedirectToAction(nameof(Details), new { id = task.Id });
    }

    public async Task<IActionResult> Finish(Guid id)
    {
        var task = await _cleaningTaskService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        task.Status = CleaningStatus.Completo;
        task.EndTime = DateTime.Now;

        if (task.StartTime.HasValue)
        {
            task.ActualMinutes =
                (int)(task.EndTime.Value - task.StartTime.Value).TotalMinutes;
        }
        else
        {
            task.ActualMinutes = null;
        }

        await _cleaningTaskService.UpdateAsync(task);

        return RedirectToAction(nameof(MyTasks));
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents(string status)
    {
        var user = await _userManager.GetUserAsync(User);

        var tasks = await _cleaningTaskService.GetAllAsync();

        if (user.Role == UserRole.Dono)
        {
            tasks = await _cleaningTaskService.GetByOwnerIdAsync(user.Id);
        }
        else
        {
            tasks = await _cleaningTaskService.GetByCompanyIdAsync((Guid)user?.CompanyId);
        }

        var rawStatus = Request.Query["status"].ToString();

        if (status == "Geral")
            tasks = tasks.Where(t => t.CleaningType == CleaningType.Geral);

        if (status == "Exterior")
            tasks = tasks.Where(t => t.CleaningType == CleaningType.Exterior);

        if (status == "Janelas")
            tasks = tasks.Where(t => t.CleaningType == CleaningType.Janelas);

        if (status == "Intermediário")
            tasks = tasks.Where(t => t.CleaningType == CleaningType.Intermediário);

        if (status == "PosConstrucao")
            tasks = tasks.Where(t => t.CleaningType == CleaningType.PosConstrução);


        var events = tasks.Select(t => new
        {
            id = t.Id,
            title = t.Property?.Name ?? "Limpeza",
            start = t.ScheduledDate,

            backgroundColor = t.Priority switch
            {
                TaskPriority.Baixa => "#198754",
                TaskPriority.Normal => "#0d6efd",
                TaskPriority.Alta => "#fd7e14",
                TaskPriority.Urgente => "#dc3545",
                _ => "#0d6efd"
            },

            borderColor = t.Priority switch
            {
                TaskPriority.Baixa => "#198754",
                TaskPriority.Normal => "#0d6efd",
                TaskPriority.Alta => "#fd7e14",
                TaskPriority.Urgente => "#dc3545",
                _ => "#0d6efd"
            },

            textColor = "#ffffff",

            extendedProps = new
            {
                estado = t.Status.ToString(),
                prioridade = t.Priority.ToString(),
                funcionaria = t.AssignedUser != null
                    ? $"{t.AssignedUser.PrimeiroNome} {t.AssignedUser.UltimoNome}"
                    : "Não atribuída",

                telefone = t.AssignedUser?.PhoneNumber,

                semLimpador = t.AssignedUserId == null,

                codigoLimpador = t.AssignedUser?.CleanerCode,
            }
        });

        return Json(events);
    }

    private async Task LoadDropdowns()
    {
        var user = await _userManager.GetUserAsync(User);

        var properties = await _propertyService.GetByUserAsync(user.Id);

        ViewBag.Propriedades = new SelectList(
            properties,
            "Id",
            "Name");

        var cleaners = _userManager.Users
            .Where(u => u.Role == UserRole.Limpador && u.Active)
            .Select(u => new
            {
                u.Id,
                Nome = u.PrimeiroNome + " " + u.UltimoNome
            })
            .ToList();

        ViewBag.Cleaners = new SelectList(
            cleaners,
            "Id",
            "Name");
    }


    public async Task<IActionResult> CleanerUpdate(Guid id)
    {
        var task = await _cleaningTaskService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        var vm = new CleaningTaskVM
        {
            Id = task.Id,
            PropriedadeNome = task.Property?.Name ?? "",
            CleanerNotes = task.CleanerNotes
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCleanerUpdate(
     Guid taskId,
     string? cleanerNotes,
     List<IFormFile>? photos)
    {
        var task = await _cleaningTaskService.GetByIdAsync(taskId);

        if (task == null)
            return NotFound();

        photos ??= new List<IFormFile>();

        const int maxPhotoCount = 10;

        // Tamanho máximo permitido no upload original.
        const long maxUploadSize = 30 * 1024 * 1024;

        // Tamanho máximo do ficheiro guardado.
        const long maxFinalSize = 10 * 1024 * 1024;

        const uint maxDimension = 1920;
        const uint initialJpegQuality = 82;
        const uint minimumJpegQuality = 45;

        var allowedExtensions = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".heic",
        ".heif"
    };

        var allowedContentTypes = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic",
        "image/heif",
        "image/heic-sequence",
        "image/heif-sequence"
    };

        if (photos.Count > maxPhotoCount)
        {
            TempData["Error"] =
                $"Só pode enviar no máximo {maxPhotoCount} fotografias.";

            return RedirectToAction(
                nameof(CleanerUpdate),
                new { id = taskId });
        }

        foreach (var photo in photos)
        {
            if (photo == null || photo.Length == 0)
                continue;

            if (photo.Length > maxUploadSize)
            {
                TempData["Error"] =
                    $"A fotografia \"{photo.FileName}\" excede o limite de 30 MB.";

                return RedirectToAction(
                    nameof(CleanerUpdate),
                    new { id = taskId });
            }

            var extension = Path.GetExtension(photo.FileName);

            if (string.IsNullOrWhiteSpace(extension) ||
                !allowedExtensions.Contains(extension))
            {
                TempData["Error"] =
                    $"O formato de \"{photo.FileName}\" não é permitido.";

                return RedirectToAction(
                    nameof(CleanerUpdate),
                    new { id = taskId });
            }

            if (!string.IsNullOrWhiteSpace(photo.ContentType) &&
                !allowedContentTypes.Contains(photo.ContentType))
            {
                TempData["Error"] =
                    $"O tipo do ficheiro \"{photo.FileName}\" não é válido.";

                return RedirectToAction(
                    nameof(CleanerUpdate),
                    new { id = taskId });
            }
        }

        var uploadsFolder = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "cleaningtasks");

        Directory.CreateDirectory(uploadsFolder);

        var newPhotos = new List<TaskPhoto>();
        var savedFiles = new List<string>(); 

        try
        {
            foreach (var photo in photos)
            {
                if (photo == null || photo.Length == 0)
                    continue;

                await using var inputStream = photo.OpenReadStream();

                using var image = new MagickImage(inputStream);

                // Corrige orientação EXIF de fotos tiradas no telemóvel.
                image.AutoOrient();

                // Remove metadata desnecessária.
                image.Strip();

                // Redimensionar mantendo proporção.
                if (image.Width > maxDimension ||
                    image.Height > maxDimension)
                {
                    var geometry = new MagickGeometry(
                        maxDimension,
                        maxDimension)
                    {
                        IgnoreAspectRatio = false
                    };

                    image.Resize(geometry);
                }

                // Tudo é guardado em JPEG.
                image.Format = MagickFormat.Jpeg;

                uint quality = initialJpegQuality;

                byte[]? finalBytes = null;

                while (quality >= minimumJpegQuality)
                {
                    image.Quality = quality;

                    await using var output = new MemoryStream();

                    await image.WriteAsync(output);

                    if (output.Length <= maxFinalSize)
                    {
                        finalBytes = output.ToArray();
                        break;
                    }

                    if (quality < 5)
                        break;

                    quality -= 5;
                }

                // Se baixar qualidade não chegar, reduzimos resolução.
                while (finalBytes == null &&
                       (image.Width > 800 || image.Height > 800))
                {
                    var newWidth =
                        (uint)Math.Max(
                            800,
                            (int)(image.Width * 0.85));

                    var newHeight =
                        (uint)Math.Max(
                            800,
                            (int)(image.Height * 0.85));

                    image.Resize(
                        new MagickGeometry(
                            newWidth,
                            newHeight)
                        {
                            IgnoreAspectRatio = false
                        });

                    image.Quality = minimumJpegQuality;

                    await using var output =
                        new MemoryStream();

                    await image.WriteAsync(output);

                    if (output.Length <= maxFinalSize)
                    {
                        finalBytes = output.ToArray();
                        break;
                    }
                }

                if (finalBytes == null)
                {
                    TempData["Error"] =
                        $"Não foi possível reduzir \"{photo.FileName}\" para menos de 10 MB.";

                    DeleteSavedFiles(savedFiles);

                    return RedirectToAction(
                        nameof(CleanerUpdate),
                        new { id = taskId });
                }

                var fileName =
                    $"{Guid.NewGuid():N}.jpg";

                var physicalPath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                await System.IO.File.WriteAllBytesAsync(
                    physicalPath,
                    finalBytes);

                savedFiles.Add(physicalPath);

                newPhotos.Add(new TaskPhoto
                {
                    Id = Guid.NewGuid(),
                    CleaningTaskId = taskId,
                    FileName = fileName,
                    FileUrl =
                        $"/uploads/cleaningtasks/{fileName}",
                    Type = PhotoType.Depois,
                    UploadedAt = DateTime.UtcNow
                });
            }

            await _cleaningTaskService
                .AddCleanerUpdateAsync(
                    taskId,
                    cleanerNotes,
                    newPhotos);

            TempData["Success"] =
                "Notas e fotografias guardadas com sucesso.";

            return RedirectToAction(
                nameof(Details),
                new { id = taskId });
        }
        catch (MagickException)
        {
            DeleteSavedFiles(savedFiles);

            TempData["Error"] =
                "Uma das fotografias não pôde ser processada.";

            return RedirectToAction(
                nameof(CleanerUpdate),
                new { id = taskId });
        }
        catch (Exception)
        {
            DeleteSavedFiles(savedFiles);

            TempData["Error"] =
                "Não foi possível guardar as fotografias.";

            return RedirectToAction(
                nameof(CleanerUpdate),
                new { id = taskId });
        }
    }

    private static SKSizeI CalculateTargetSize(
    int width,
    int height,
    int maxDimension)
    {
        if (width <= maxDimension && height <= maxDimension)
        {
            return new SKSizeI(width, height);
        }

        var scale = Math.Min(
            (double)maxDimension / width,
            (double)maxDimension / height);

        return new SKSizeI(
            Math.Max(1, (int)Math.Round(width * scale)),
            Math.Max(1, (int)Math.Round(height * scale)));
    }

    private static SKBitmap ApplyExifOrientation(
    SKBitmap source,
    SKEncodedOrigin origin)
    {
        if (origin == SKEncodedOrigin.TopLeft)
            return source.Copy();

        var swapDimensions =
            origin == SKEncodedOrigin.LeftTop ||
            origin == SKEncodedOrigin.RightTop ||
            origin == SKEncodedOrigin.RightBottom ||
            origin == SKEncodedOrigin.LeftBottom;

        var width = swapDimensions ? source.Height : source.Width;
        var height = swapDimensions ? source.Width : source.Height;

        var result = new SKBitmap(
            width,
            height,
            source.ColorType,
            source.AlphaType);

        using var canvas = new SKCanvas(result);

        switch (origin)
        {
            case SKEncodedOrigin.TopRight:
                canvas.Translate(width, 0);
                canvas.Scale(-1, 1);
                break;

            case SKEncodedOrigin.BottomRight:
                canvas.Translate(width, height);
                canvas.RotateDegrees(180);
                break;

            case SKEncodedOrigin.BottomLeft:
                canvas.Translate(0, height);
                canvas.Scale(1, -1);
                break;

            case SKEncodedOrigin.LeftTop:
                canvas.RotateDegrees(90);
                canvas.Scale(1, -1);
                break;

            case SKEncodedOrigin.RightTop:
                canvas.Translate(width, 0);
                canvas.RotateDegrees(90);
                break;

            case SKEncodedOrigin.RightBottom:
                canvas.Translate(width, height);
                canvas.RotateDegrees(-90);
                canvas.Scale(1, -1);
                break;

            case SKEncodedOrigin.LeftBottom:
                canvas.Translate(0, height);
                canvas.RotateDegrees(-90);
                break;
        }

        canvas.DrawBitmap(source, 0, 0);
        canvas.Flush();

        return result;
    }

    private static void DeleteSavedFiles(
    IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            try
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            catch
            {
                // Não esconder o erro principal por falhar a limpeza.
            }
        }
    }

    public async Task<IActionResult> AssignCleaner(
    Guid id,
    string? search)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var task = await _cleaningTaskService
            .GetByIdAsync(id);

        if (task == null)
            return NotFound();

        var query = _userManager.Users
            .Where(u =>
                (u.Role == UserRole.Limpador || u.Role == UserRole.GestorELimpador) &&
                u.EmailConfirmed == true &&
                u.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            var numericTerm = term
                .Replace("LMP-", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            var isCleanerNumber = int.TryParse(
                numericTerm,
                out var cleanerNumber);

            query = query.Where(u =>
                (isCleanerNumber &&
                u.CleanerNumber == cleanerNumber) ||
                u.PrimeiroNome.Contains(term) ||
                u.UltimoNome.Contains(term) ||
                u.Email!.Contains(term) ||
                (u.PhoneNumber != null &&
                 u.PhoneNumber.Contains(term)));
        }

        var cleaners = await query
            .Where(u => u.CompanyId == user.CompanyId)
            .OrderBy(u => u.PrimeiroNome)
            .ThenBy(u => u.UltimoNome)
            .Take(30)
            .Select(u => new CleanerSearchItemVM
            {
                Id = u.Id,
                LimpadorCodigo = u.CleanerCode,
                NomeCompleto =
                    u.PrimeiroNome + " " + u.UltimoNome,
                Email = u.Email ?? "",
                Telemovel = u.PhoneNumber,
                Active = u.Active
            })
            .ToListAsync();


        if (user.Role == UserRole.GestorELimpador) {
        
             cleaners = await query
            .Where(u => u.CompanyId == user.CompanyId)
            .OrderBy(u => u.PrimeiroNome)
            .ThenBy(u => u.UltimoNome)
            .Take(30)
            .Select(u => new CleanerSearchItemVM
            {
                Id = u.Id,
                LimpadorCodigo = u.CleanerCode,
                NomeCompleto =
                    u.PrimeiroNome + " " + u.UltimoNome,
                Email = u.Email ?? "",
                Telemovel = u.PhoneNumber,
                Active = u.Active
            })
            .ToListAsync();
        }
        

        var vm = new AssignCleanerVM
        {
            CleaningTaskId = task.Id,
            PropriedadeNome = task.Property?.Name ?? "",
            ScheduledDate = task.ScheduledDate,
            SelectedCleanerId = task.AssignedUserId,
            Search = search,
            Cleaners = cleaners
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignCleaner(AssignCleanerVM vm)
    {
        var owner = await _userManager.GetUserAsync(User);

        if (owner == null)
            return Challenge();
        var id = vm.CleaningTaskId;
        var task = await _cleaningTaskService
            .GetByIdAsync(id);

        if (task == null)
            return NotFound();

        if (!vm.SelectedCleanerId.HasValue)
        {
            ModelState.AddModelError(
                nameof(vm.SelectedCleanerId),
                "Selecione um limpador.");

            return RedirectToAction(
                nameof(AssignCleaner),
                new { id = vm.CleaningTaskId });
        }

        var cleaner = await _userManager.Users
            .FirstOrDefaultAsync(u =>
                u.Id == vm.SelectedCleanerId.Value &&
                (u.Role == UserRole.Limpador || u.Role == UserRole.GestorELimpador) &&
                u.Active);

        if (cleaner == null)
        {
            TempData["Error"] =
                "O limpador selecionado não existe ou está inativo.";

            return RedirectToAction(
                nameof(AssignCleaner),
                new { id = vm.CleaningTaskId });
        }

        task.AssignedUser = cleaner;

        await _cleaningTaskService.UpdateAsync(task);

        var taskDetailsLink = Url.Action(
            "Details",
            "CleaningTask",
            new { id = task.Id },
            protocol: Request.Scheme)!;

        var propriedade = await _propertyService.GetByIdAsync(task.PropertyId);

        await _emailService.SendCleaningAssignedEmailAsync(
            cleaner,
            task,
            propriedade,
            taskDetailsLink);

        TempData["Success"] =
            $"A limpeza foi atribuída a {cleaner.PrimeiroNome} {cleaner.UltimoNome}.";

        return RedirectToAction(
            nameof(Details),
            new { id = task.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(
        Guid photoId,
        Guid taskId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var task = await _cleaningTaskService
            .GetByIdAsync(taskId);

        if (task == null)
            return NotFound();

        var deleted = await _cleaningTaskService
            .DeletePhotoAsync(photoId, taskId);

        if (!deleted)
        {
            TempData["Error"] =
                "Não foi possível eliminar a fotografia.";
        }
        else
        {
            TempData["Success"] =
                "Fotografia eliminada com sucesso.";
        }

        return RedirectToAction(
            nameof(Details),
            new { id = taskId });
    }

    public async Task<IActionResult> Team()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return RedirectToAction("Login", "Account");

        if (user.Role != UserRole.Gestor &&
            user.Role != UserRole.GestorELimpador)
        {
            return Forbid();
        }

        var membros = await _userManager.Users
            .Where(u => u.CompanyId == user.CompanyId)
            .ToListAsync();

        return View(membros);
    }



}
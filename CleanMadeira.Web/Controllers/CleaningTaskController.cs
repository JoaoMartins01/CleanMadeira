using CleanMadeira.Application.Contract;
using CleanMadeira.Application.Services.Interface;
using CleanMadeira.Domain.Entities;
using CleanMadeira.Domain.Entities.Enums;
using CleanMadeira.Web.ViewModels;
using CleanMadeira.Web.ViewModels.CleaningTask;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Security.Claims;

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
            task = await _cleaningTaskService.GetByIdAndOwnerAsync(id, userId);
        }


        if (task == null)
            return NotFound();

        var vm = new CleaningTaskVM
        {
            Id = task.Id,

            PropriedadeNome = task.Property?.Name ?? "",
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

        var task = new CleaningTask
        {
            Id = Guid.NewGuid(),
            PropertyId = vm.PropriedadeId,
            AssignedUserId = vm.AssignedUserId,
            ScheduledDate = vm.ScheduledDate,
            Priority = vm.Prioridade,
            Status = CleaningStatus.Pendente,
            EstimatedMinutes = vm.EstimatedMinutes,
            Notes = vm.Notas
        };

        await _cleaningTaskService.CreateAsync(task);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var task = await _cleaningTaskService.GetByIdAndOwnerAsync(id, userId);

        if (task == null)
            return NotFound();

        await LoadDropdowns();

        var vm = new EditCleaningTaskVM
        {
            Id = task.Id,
            PropriedadeId = task.PropertyId,
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

        var task = await _cleaningTaskService.GetByIdAndOwnerAsync(vm.Id, userId);

        if (task == null)
            return NotFound();

        task.PropertyId = vm.PropriedadeId;
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

        var task = await _cleaningTaskService.GetByIdAndOwnerAsync(id, userId);

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

        var task = await _cleaningTaskService.GetByIdAndOwnerAsync(id, userId);

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
    public async Task<IActionResult> GetEvents()
    {
        var user = await _userManager.GetUserAsync(User);

        var tasks = await _cleaningTaskService.GetByOwnerIdAsync(user.Id);

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
        const long maxPhotoSize = 10 * 1024 * 1024;
        const int maxDimension = 1920;
        const int jpegQuality = 82;

        var allowedExtensions = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

        var allowedContentTypes = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
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

            if (photo.Length > maxPhotoSize)
            {
                TempData["Error"] =
                    $"A fotografia \"{photo.FileName}\" excede o limite de 10 MB.";

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

            if (!allowedContentTypes.Contains(photo.ContentType))
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

                using var managedStream = new SKManagedStream(inputStream);
                using var codec = SKCodec.Create(managedStream);

                if (codec == null)
                {
                    TempData["Error"] =
                        $"O ficheiro \"{photo.FileName}\" não é uma imagem válida.";

                    DeleteSavedFiles(savedFiles);

                    return RedirectToAction(
                        nameof(CleanerUpdate),
                        new { id = taskId });
                }

                var sourceInfo = codec.Info;

                using var sourceBitmap = new SKBitmap(
                    sourceInfo.Width,
                    sourceInfo.Height,
                    sourceInfo.ColorType,
                    sourceInfo.AlphaType);

                var decodeResult = codec.GetPixels(
                    sourceBitmap.Info,
                    sourceBitmap.GetPixels());

                if (decodeResult != SKCodecResult.Success &&
                    decodeResult != SKCodecResult.IncompleteInput)
                {
                    TempData["Error"] =
                        $"Não foi possível processar \"{photo.FileName}\".";

                    DeleteSavedFiles(savedFiles);

                    return RedirectToAction(
                        nameof(CleanerUpdate),
                        new { id = taskId });
                }

                using var orientedBitmap = sourceBitmap.Copy();

                var targetSize = CalculateTargetSize(
                    orientedBitmap.Width,
                    orientedBitmap.Height,
                    maxDimension);

                using var resizedBitmap =
                    targetSize.Width == orientedBitmap.Width &&
                    targetSize.Height == orientedBitmap.Height
                    ? orientedBitmap.Copy()
                    : orientedBitmap.Resize(
                        new SKImageInfo(
                        targetSize.Width,
                        targetSize.Height),
                        SKFilterQuality.Medium);

                if (resizedBitmap == null)
                {
                    TempData["Error"] =
                        $"Não foi possível redimensionar \"{photo.FileName}\".";

                    DeleteSavedFiles(savedFiles);

                    return RedirectToAction(
                        nameof(CleanerUpdate),
                        new { id = taskId });
                }

                var fileName = $"{Guid.NewGuid():N}.jpg";

                var physicalPath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using var image = SKImage.FromBitmap(resizedBitmap);

                using var encodedData = image.Encode(
                    SKEncodedImageFormat.Jpeg,
                    jpegQuality);

                if (encodedData == null)
                {
                    TempData["Error"] =
                        $"Não foi possível guardar \"{photo.FileName}\".";

                    DeleteSavedFiles(savedFiles);

                    return RedirectToAction(
                        nameof(CleanerUpdate),
                        new { id = taskId });
                }

                await using (var outputStream =
                             System.IO.File.Create(physicalPath))
                {
                    encodedData.SaveTo(outputStream);
                }

                savedFiles.Add(physicalPath);

                newPhotos.Add(new TaskPhoto
                {
                    Id = Guid.NewGuid(),
                    CleaningTaskId = taskId,
                    FileName = fileName,
                    FileUrl = $"/uploads/cleaningtasks/{fileName}",
                    Type = PhotoType.Depois,
                    UploadedAt = DateTime.UtcNow
                });
            }

            await _cleaningTaskService.AddCleanerUpdateAsync(
                taskId,
                cleanerNotes,
                newPhotos);

            TempData["Success"] =
                "Notas e fotografias guardadas com sucesso.";

            return RedirectToAction(
                nameof(Details),
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
                u.Role == UserRole.Limpador &&
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
                u.Role == UserRole.Limpador &&
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

        await _emailService.SendCleaningAssignedEmailAsync(
            cleaner,
            task,
            task.Property,
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



}
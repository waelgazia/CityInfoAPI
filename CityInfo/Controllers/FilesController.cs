using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
           ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
    }

    [HttpGet("{fileId}")]
    public ActionResult GetFile(int fileId)
    {
        // demo code, look-up for actual file id
        var pathToFile = "file_path.pdf";
        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound();
        }
        
        // Determine the file content type
        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out string fileContentType))
        {
            fileContentType = "application/octet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, fileContentType, Path.GetFileName(pathToFile));
    }

    [HttpPost]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        // validate the input, put a limit on the files size to avoid large uploads attachment.
        // and only accept certain content type (i.e., pdf)
        if (file.Length == 0 || file.Length > 20971520 || file.ContentType != "application/pdf")
        {
            return BadRequest("No file or an invalid one has been inputted!");
        }
        
        // create the file path, avoid using file.FileName, as an attacker can provide malicious one,
        // include full paths or relative paths, and store the file in a separate disk.
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"uploaded_file_{Guid.NewGuid()}.pdf");

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok("Your file has been uploaded successfully!");
    }
}
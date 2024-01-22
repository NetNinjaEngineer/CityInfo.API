using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
    }

    [HttpGet]
    public IActionResult GetFile(string fileId)
    {
        var pathToFile = @"C:\\Users\\Mohamed ElHelaly\\Downloads\\MohamedElHelaly_Resume.pdf";
        if (!System.IO.File.Exists(pathToFile))
            return NotFound();

        var fileBytes = System.IO.File.ReadAllBytes(pathToFile);
        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out string contenyType))
            contenyType = "application/octet-stream";

        return File(fileBytes, contenyType, Path.GetFileName(pathToFile));
    }
}

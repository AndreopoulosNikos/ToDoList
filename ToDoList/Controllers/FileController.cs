using Microsoft.AspNetCore.Mvc;
using ToDoList.DatabaseAccess.ConnectionFactories.IConnectionFactories;
using ToDoList.DatabaseAccess.Repositories.IRepositories;

namespace ToDoList.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {

        private readonly IFileRepository _fileRepository;
        private readonly IConnectionFactory _connectionFactory;

        // Constructor injection
        public FileController(IFileRepository fileRepository,IConnectionFactory connectionFactory)
        {
            _fileRepository = fileRepository;
            _connectionFactory = connectionFactory;
        }


        /// <summary>
        /// Uploads a single PDF file to a temporary location.
        /// </summary>
        /// <param name="file">The PDF file from the client.</param>
        /// <returns>Returns the temporary file path and name.</returns>
        [HttpPost("upload-temp")]
        public async Task<IActionResult> UploadTempFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (file.ContentType != "application/pdf")
                return BadRequest("Only PDF files are allowed.");

            // Get the system temp folder
            string tempFolder = Path.GetTempPath();

            // Create a unique temp file name
            string tempFilePath = Path.Combine(tempFolder, Path.GetRandomFileName() + ".pdf");

            // Stream file directly to disk
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the temp file info to the client
            return Ok(new
            {
                TempFilePath = tempFilePath,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length
            });
        }

        /// <summary>
        /// Serves an existing PDF file by its ID from the database.
        /// </summary>
        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var file = await _fileRepository.GetFileById(connection, id);
            if (file == null || !System.IO.File.Exists(file.FilePath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
            
            Response.Headers["Content-Disposition"] = $"inline; filename={file.Filename}";
            return File(bytes, "application/pdf");
           
        }
    }

}

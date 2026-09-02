using Microsoft.AspNetCore.Mvc;
using TrainingHelpDeskApi.Services;

namespace TrainingHelpDeskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeBaseController : ControllerBase
    {
        private readonly IRagService _ragService;
        private readonly KnowledgeBaseService _knowledgeBaseService;
        private readonly IDocumentLoaderService _documentLoader;

        public KnowledgeBaseController(
            IRagService ragService,
            KnowledgeBaseService knowledgeBaseService,
            IDocumentLoaderService documentLoader)
        {
            _ragService = ragService;
            _knowledgeBaseService = knowledgeBaseService;
            _documentLoader = documentLoader;
        }

        // GET api/knowledgebase/documents
        // Lists every source document currently available in the Trainee Knowledge Base.
        [HttpGet("documents")]
        public IActionResult GetDocuments()
        {
            return Ok(_documentLoader.GetAllDocumentNames());
        }

        // GET api/knowledgebase/status
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                isBuilt = _knowledgeBaseService.IsBuilt,
                totalChunks = _knowledgeBaseService.GetDocuments().Count,
                lastBuiltAtUtc = _knowledgeBaseService.LastBuiltAtUtc
            });
        }

        // POST api/knowledgebase/build
        // Loads every document, chunks it, generates embeddings and stores them in memory.
        [HttpPost("build")]
        public async Task<IActionResult> BuildKnowledgeBase()
        {
            try
            {
                int totalChunks = await _ragService.BuildKnowledgeBaseAsync();

                return Ok(new
                {
                    message = "Trainee Knowledge Base built successfully.",
                    totalChunks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingHelpDeskApi.Data;
using TrainingHelpDeskApi.Models;
using TrainingHelpDeskApi.Models.Dtos;
using TrainingHelpDeskApi.Services;

namespace TrainingHelpDeskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IRagService _ragService;
        private readonly AppDbContext _dbContext;

        public ChatController(IRagService ragService, AppDbContext dbContext)
        {
            _ragService = ragService;
            _dbContext = dbContext;
        }

        // POST api/chat/ask
        // Main entry point: trainee asks a question, gets a RAG-generated answer back.
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new { message = "Question cannot be empty." });
            }

            try
            {
                AskResponseDto response = await _ragService.AskAsync(request.Question);

                // Persist the interaction to SQL Server via EF Core.
                _dbContext.ChatLogs.Add(new ChatLog
                {
                    Question = response.Question,
                    Answer = response.Answer,
                    SourcesUsed = string.Join(", ", response.Sources),
                    AnswerFoundInKnowledgeBase = response.AnswerFoundInKnowledgeBase,
                    CreatedAtUtc = DateTime.UtcNow
                });
                await _dbContext.SaveChangesAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // GET api/chat/history
        // Returns recent trainee chat history from SQL Server.
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] int take = 20)
        {
            List<ChatHistoryItemDto> history = await _dbContext.ChatLogs
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(take)
                .Select(x => new ChatHistoryItemDto
                {
                    Id = x.Id,
                    Question = x.Question,
                    Answer = x.Answer,
                    CreatedAtUtc = x.CreatedAtUtc
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}

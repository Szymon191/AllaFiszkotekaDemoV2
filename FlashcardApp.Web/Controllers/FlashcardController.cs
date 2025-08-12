using FlashcardApp.Core.DTO;
using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlashcardApp.Web.Controllers
{
    [Route("api/flashcards")]
    [ApiController]
    [Authorize]
    public class FlashcardController : ControllerBase
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IFlashcardCategoryRepository _categoryRepository;

        public FlashcardController(IFlashcardRepository flashcardRepository, ILessonRepository lessonRepository, IFlashcardCategoryRepository categoryRepository)
        {
            _flashcardRepository = flashcardRepository;
            _lessonRepository = lessonRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("lesson/{lessonId}")]
        public async Task<IActionResult> GetFlashcardsByLesson(int lessonId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null)
                return NotFound("Lesson not found.");

            var category = await _categoryRepository.GetByIdAsync(lesson.CategoryId);
            if (category == null || category.UserId != userId)
                return Forbid("You can only access your own flashcards.");

            var flashcards = await _flashcardRepository.GetByLessonIdAsync(lessonId);
            var flashcardDtos = flashcards.Select(f => new FlashcardDto
            {
                Id = f.Id,
                LessonId = f.LessonId,
                Word = f.Word,
                Translation = f.Translation,
                ExampleUsage = f.ExampleUsage,
                Tags = f.Tags,
                Difficulty = f.Difficulty
            }).ToList();

            return Ok(flashcardDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashcard([FromBody] FlashcardRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (string.IsNullOrEmpty(request.Word) || string.IsNullOrEmpty(request.Translation) || request.LessonId <= 0)
                return BadRequest("Word, Translation, and LessonId are required.");

            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
            if (lesson == null)
                return NotFound("Lesson not found.");

            var category = await _categoryRepository.GetByIdAsync(lesson.CategoryId);
            if (category == null || category.UserId != userId)
                return Forbid("You can only add flashcards to your own lessons.");

            var flashcard = new Flashcard
            {
                LessonId = request.LessonId,
                Word = request.Word,
                Translation = request.Translation,
                ExampleUsage = request.ExampleUsage,
                Tags = request.Tags ?? new string[] { },
                Difficulty = request.Difficulty ?? "Medium",
                CreatedAt = DateTime.UtcNow
            };

            await _flashcardRepository.AddAsync(flashcard);
            var flashcardDto = new FlashcardDto
            {
                Id = flashcard.Id,
                LessonId = flashcard.LessonId,
                Word = flashcard.Word,
                Translation = flashcard.Translation,
                ExampleUsage = flashcard.ExampleUsage,
                Tags = flashcard.Tags,
                Difficulty = flashcard.Difficulty
            };

            return Ok(flashcardDto);
        }
    }
}
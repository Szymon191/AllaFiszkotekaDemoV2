using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using FlashcardApp.Infrastructure.Repositories;
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
        private readonly IFlashcardCategoryRepository _categoryRepository;
        private readonly ILessonRepository _lessonRepository;

        public FlashcardController(IFlashcardRepository flashcardRepository, IFlashcardCategoryRepository categoryRepository, ILessonRepository lessonRepository )
        {
            _flashcardRepository = flashcardRepository;
            _categoryRepository = categoryRepository;
            _lessonRepository = lessonRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashcard([FromBody] FlashcardRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (string.IsNullOrEmpty(request.Word) || string.IsNullOrEmpty(request.Translation) || request.LessonId <= 0)
                return BadRequest("Word, Translation, and LessonId are required.");

            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
            if (lesson == null)
                return NotFound("Lesson not found.");

            // Sprawdzenie, czy lekcja należy do użytkownika (przez kategorię)
            var category = await _categoryRepository.GetByIdAsync(lesson.CategoryId); // Zakładając, że masz _categoryRepository wstrzyknięte
            if (category == null || category.UserId != userId)
                return Forbid("You can only add flashcards to your own lessons.");

            var flashcard = new Flashcard
            {
                LessonId = request.LessonId,
                Word = request.Word,
                Translation = request.Translation,
                ExampleUsage = request.ExampleUsage,
                Tags = request.Tags ?? new string[] { },
                Difficulty = request.Difficulty ?? "Medium"
            };

            await _flashcardRepository.AddAsync(flashcard);
            return Ok(flashcard);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlashcard(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var flashcard = await _flashcardRepository.GetByIdAsync(id);
            if (flashcard == null)
                return NotFound();

            var category = await _categoryRepository.GetByIdAsync(flashcard.CategoryId);
            if (category == null)
                return NotFound("Category not found.");

            // Sprawdzenie, czy kategoria należy do użytkownika lub jest publiczna
            if (category.UserId != userId && !category.IsPublic)
                return Forbid("You can only view flashcards in your own categories or public ones.");

            return Ok(flashcard);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetFlashcardsByCategory(int categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                return NotFound("Category not found.");

            // Sprawdzenie, czy kategoria należy do użytkownika lub jest publiczna
            if (category.UserId != userId && !category.IsPublic)
                return Forbid("You can only view flashcards in your own categories or public ones.");

            var flashcards = await _flashcardRepository.GetAllByCategoryIdAsync(categoryId);
            return Ok(flashcards);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFlashcards(string word)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (string.IsNullOrEmpty(word))
                return BadRequest("Search term is required.");

            // Wyszukiwanie tylko w kategoriach użytkownika lub publicznych
            var flashcards = await _flashcardRepository.SearchByWordAsync(word);
            var allowedFlashcards = new List<Flashcard>();

            foreach (var flashcard in flashcards)
            {
                var category = await _categoryRepository.GetByIdAsync(flashcard.CategoryId);
                if (category != null && (category.UserId == userId || category.IsPublic))
                {
                    allowedFlashcards.Add(flashcard);
                }
            }

            return Ok(allowedFlashcards);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlashcard(int id, [FromBody] Flashcard flashcard)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (id != flashcard.Id)
                return BadRequest("Invalid ID.");

            var existingFlashcard = await _flashcardRepository.GetByIdAsync(id);
            if (existingFlashcard == null)
                return NotFound();

            var category = await _categoryRepository.GetByIdAsync(existingFlashcard.CategoryId);
            if (category == null)
                return NotFound("Category not found.");

            // Sprawdzenie, czy kategoria należy do użytkownika
            if (category.UserId != userId)
                return Forbid("You can only update flashcards in your own categories.");

            flashcard.CategoryId = existingFlashcard.CategoryId; // Zapobiega zmianie kategorii
            await _flashcardRepository.UpdateAsync(flashcard);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcard(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var flashcard = await _flashcardRepository.GetByIdAsync(id);
            if (flashcard == null)
                return NotFound();

            var category = await _categoryRepository.GetByIdAsync(flashcard.CategoryId);
            if (category == null)
                return NotFound("Category not found.");

            // Sprawdzenie, czy kategoria należy do użytkownika
            if (category.UserId != userId)
                return Forbid("You can only delete flashcards in your own categories.");

            await _flashcardRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
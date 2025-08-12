using FlashcardApp.Core.DTO;
using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlashcardApp.Web.Controllers
{
    [Route("api/lessons")]
    [ApiController]
    [Authorize]
    public class LessonController : ControllerBase
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IFlashcardCategoryRepository _categoryRepository;

        public LessonController(ILessonRepository lessonRepository, IFlashcardCategoryRepository categoryRepository)
        {
            _lessonRepository = lessonRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLesson([FromBody] LessonRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (string.IsNullOrEmpty(request.Name) || request.CategoryId <= 0)
                return BadRequest("Name and CategoryId are required.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return NotFound("Category not found.");

            if (category.UserId != userId)
                return Forbid("You can only add lessons to your own categories.");

            var lesson = new Lesson
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow
            };

            await _lessonRepository.AddAsync(lesson);
            var lessonDto = new LessonDto
            {
                Id = lesson.Id,
                CategoryId = lesson.CategoryId,
                Name = lesson.Name
            };

            return Ok(lessonDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesson(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                return NotFound("Lesson not found.");
            var lessonDto = new LessonDto
            {
                Id = lesson.Id,
                CategoryId = lesson.CategoryId,
                Name = lesson.Name
            };
            return Ok(lessonDto);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetLessonsByCategory(int categoryId)
        {
            var lessons = await _lessonRepository.GetByCategoryIdAsync(categoryId);
            if (lessons == null || !lessons.Any())
                return NotFound("No lessons found for this category.");
            var lessonDtos = lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                CategoryId = l.CategoryId,
                Name = l.Name
            }).ToList();
            return Ok(lessonDtos);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                return NotFound("Lesson not found.");
            if (lesson.Category.UserId != userId)
                return Forbid("You can only update your own lessons.");
            lesson.Name = request.Name;
            await _lessonRepository.UpdateAsync(lesson);
            var lessonDto = new LessonDto
            {
                Id = lesson.Id,
                CategoryId = lesson.CategoryId,
                Name = lesson.Name
            };
            return Ok(lessonDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null)
                return NotFound("Lesson not found.");
            if (lesson.Category.UserId != userId)
                return Forbid("You can only delete your own lessons.");
            await _lessonRepository.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLessons()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");
            var categories = await _categoryRepository.GetByUserIdAsync(userId);
            if (categories == null || !categories.Any())
                return NotFound("No categories found for this user.");
            var lessons = new List<LessonDto>();
            foreach (var category in categories)
            {
                var categoryLessons = await _lessonRepository.GetByCategoryIdAsync(category.Id);
                lessons.AddRange(categoryLessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    CategoryId = l.CategoryId,
                    Name = l.Name
                }));
            }
            return Ok(lessons);
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetLessonsByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");
            var categories = await _categoryRepository.GetByUserIdAsync(userId);
            if (categories == null || !categories.Any())
                return NotFound("No categories found for this user.");
            var lessons = new List<LessonDto>();
            foreach (var category in categories)
            {
                var categoryLessons = await _lessonRepository.GetByCategoryIdAsync(category.Id);
                lessons.AddRange(categoryLessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    CategoryId = l.CategoryId,
                    Name = l.Name
                }));
            }
            return Ok(lessons);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchLessons([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Search query cannot be empty.");
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");
            var categories = await _categoryRepository.GetByUserIdAsync(userId);
            if (categories == null || !categories.Any())
                return NotFound("No categories found for this user.");
            var lessons = new List<LessonDto>();
            foreach (var category in categories)
            {
                var categoryLessons = await _lessonRepository.GetByCategoryIdAsync(category.Id);
                lessons.AddRange(categoryLessons
                    .Where(l => l.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .Select(l => new LessonDto
                    {
                        Id = l.Id,
                        CategoryId = l.CategoryId,
                        Name = l.Name
                    }));
            }
            return Ok(lessons);
        }
    }
}

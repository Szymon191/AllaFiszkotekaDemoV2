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

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetLessonsByCategory(int categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
                return NotFound("Category not found.");

            if (category.UserId != userId)
                return Forbid("You can only access your own categories.");

            var lessons = await _lessonRepository.GetByCategoryIdAsync(categoryId);
            var lessonDtos = lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                CategoryId = l.CategoryId,
                Name = l.Name
            }).ToList();

            return Ok(lessonDtos);
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
    }
}
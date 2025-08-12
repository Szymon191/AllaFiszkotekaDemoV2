using FlashcardApp.Core.Auth.DTO;
using FlashcardApp.Core.Models;
using FlashcardApp.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlashcardApp.Web.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [Authorize] // Wymaga tokena dla wszystkich endpointów
    public class FlashcardCategoryController : ControllerBase
    {
        private readonly IFlashcardCategoryRepository _categoryRepository;

        public FlashcardCategoryController(IFlashcardCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateFlashcardCategoryDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (string.IsNullOrEmpty(dto.Name))
                return BadRequest("Name is required.");

            var existing = await _categoryRepository.GetByNameAndUserIdAsync(dto.Name, userId);
            if (existing != null)
                return Conflict("Category with this name already exists for the user.");

            var category = new FlashcardCategory
            {
                UserId = userId,
                Name = dto.Name,
                IsPublic = dto.IsPublic
            };

            await _categoryRepository.AddAsync(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            // Sprawdzenie, czy kategoria należy do użytkownika lub jest publiczna
            if (category.UserId != userId && !category.IsPublic)
                return Forbid("You can only view your own categories or public ones.");

            return Ok(category);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCategories(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized("Invalid token.");

            // Użytkownik może zobaczyć tylko swoje kategorie
            if (currentUserId != userId)
                return Forbid("You can only view your own categories.");

            var categories = await _categoryRepository.GetAllByUserIdAsync(userId);
            return Ok(categories);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicCategories()
        {
            var categories = await _categoryRepository.GetPublicCategoriesAsync();
            return Ok(categories);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] FlashcardCategory category)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            if (id != category.Id)
                return BadRequest("Invalid ID.");

            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Sprawdzenie, czy kategoria należy do użytkownika
            if (existing.UserId != userId)
                return Forbid("You can only update your own categories.");

            category.UserId = userId; // Zapobiega zmianie UserId
            await _categoryRepository.UpdateAsync(category);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token.");

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            // Sprawdzenie, czy kategoria należy do użytkownika
            if (category.UserId != userId)
                return Forbid("You can only delete your own categories.");

            await _categoryRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System_API.Models;

namespace Task_Management_System_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskCategoryController : ControllerBase
    {
        private readonly TaskDbContext db;

        public TaskCategoryController(TaskDbContext context)
        {
            db = context;
        }

        // GET: api/Member
        [HttpGet]
        [ProducesResponseType(typeof(TaskCategory[]), 200)]
        public async Task<ActionResult<TaskCategory[]>> GetTaskCategory()
        {
            var data = await db.TaskCategories.ToListAsync();
            return Ok(data);
        }

        // GET: api/Member/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskCategory(int id)
        {
            var taskCategory = await db.TaskCategories.FindAsync(id);
            if (taskCategory == null) return NotFound();
            return Ok(taskCategory);
        }

        // POST: api/Member
        //[HttpPost]
        //public async Task<IActionResult> CreateTaskCategory(TaskCategory taskCategory)
        //{
        //    db.TaskCategories.Add(taskCategory);
        //    await db.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetTaskCategory), new { id = taskCategory.TaskCategoryId }, taskCategory);
        //}


        [HttpPost]
        public async Task<IActionResult> CreateTaskCategory(TaskCategory taskCategory)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                var errors = ModelState.SelectMany(ms => ms.Value.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();

                return BadRequest(new { Errors = errors });
            }

            taskCategory.CreateBy = taskCategory.CreateBy; // Handle logged-in user
            taskCategory.UpdateBy = taskCategory.UpdateBy; // Handle logged-in user
            taskCategory.CreateDate = DateTime.Now; // Set the creation date
            taskCategory.UpdateDate = DateTime.Now; // Set the creation date
            taskCategory.IsActive = true; // Default value

            db.TaskCategories.Add(taskCategory);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskCategory), new { id = taskCategory.TaskCategoryId }, taskCategory);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskCategory(int id, [FromBody] TaskCategory updatedTaskCategory)
        {
            if (id != updatedTaskCategory.TaskCategoryId)
                return BadRequest("TaskCategory ID mismatch.");

            var existingCategory = await db.TaskCategories.FindAsync(id);
            if (existingCategory == null)
                return NotFound("TaskCategory not found.");

            // Update the Category properties
            existingCategory.Name = updatedTaskCategory.Name;
            existingCategory.UpdateBy = updatedTaskCategory.UpdateBy;
            existingCategory.UpdateDate = DateTime.Now; // Set the current date for update
            existingCategory.IsActive = updatedTaskCategory.IsActive;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskCategoryExists(id))
                    return NotFound("TaskCategory not found during update.");
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception (could use a logger here, but for simplicity returning it)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the task category.", details = ex.Message });
            }

            return NoContent();
        }


        private async Task<bool> TaskCategoryExists(int id)
        {
            return await db.TaskCategories.AnyAsync(m => m.TaskCategoryId == id);
        }

        // DELETE: api/Member/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskCategory(int id)
        {
            var category = await db.TaskCategories.FindAsync(id);
            if (category == null) return NotFound();
            db.TaskCategories.Remove(category);
            await db.SaveChangesAsync();
            return NoContent();
        }
    }
}
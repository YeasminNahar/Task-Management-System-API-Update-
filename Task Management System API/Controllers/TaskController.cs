using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = Task_Management_System_API.Models.Task;
using Task_Management_System_API.ViewModels;
using System.Numerics;

namespace Task_Management_System_API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskDbContext _context;

        public TaskController(TaskDbContext context)
        {
            _context = context;
        }
        // Get all tasks
        [HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<Task>), 200)] // Correct the response type to IEnumerable<Task>
        public async Task<IActionResult> GetTasks(string username, int? status)  //status 0=pending, 1=complete, 2=all
        {
            try
            {
                var task = new List<Task>();
                var user = HttpContext.User.Identity.Name;
                var RoleId = await _context.UserDetails.Where(x => x.UserName.ToLower() == username.ToLower()).Select(x => x.RoleId).FirstOrDefaultAsync();
                var userRole = await _context.UserRoles.Where(x => x.RoleId == RoleId).Select(x => x.RoleName).FirstOrDefaultAsync();
                if (userRole == "Member")
                {
                    var memberId = await _context.UserDetails.Where(x => x.UserName == username).Select(x => x.MemberId).FirstOrDefaultAsync();
                    var memberTaskLists = await _context.TaskAssigns.Where(x => x.MemberId == memberId).Select(x => x.TaskId).ToListAsync();
                    task = await _context.Tasks.Include(t => t.TaskCategory).Where(x => (status == 3 ? x.TaskStatus == x.TaskStatus : x.TaskStatus == status)
                    && memberTaskLists.Contains(x.TaskId)).ToListAsync();
                }
                else
                {
                    task = await _context.Tasks.Include(t => t.TaskCategory).Where(x => status == 3 ? x.TaskStatus == x.TaskStatus : x.TaskStatus == status).ToListAsync();
                }


                if (task == null || !task.Any())
                {

                    return NotFound("No tasks found.");
                }
                var result = new List<TaskViewModel>();

                foreach (var item in task)
                {
                    result.Add(new TaskViewModel
                    {
                        CreateBy = item.CreateBy,
                        CreateDate = item.CreateDate,
                        Description = item.Description,
                        DeadLine = item.DeadLine,
                        UpdateBy = item.UpdateBy,
                        UpdateDate = item.UpdateDate,
                        TaskCategoryId = item.TaskCategoryId,
                        TaskStatus = item.TaskStatus,
                        TaskId = item.TaskId,
                        TaskCategory = item.TaskCategory?.Name

                    });
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (use your logger here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetTask()
        {
            try
            {
                // Fetch TaskAssigns with navigation properties included
                var data = await _context.Tasks.Where(x => x.TaskStatus == 0)
                    // Includes Task navigation property
                    .Include(t => t.TaskCategory) // Includes TaskCategory through Task
                    .ToListAsync();

                // Check if data is empty
                if (data == null || !data.Any())
                {
                    return NotFound("No task assignments found.");
                }

                // Map data to TaskAssignVM including related data
                var result = new List<TaskViewModel>();

                foreach (var item in data)
                {
                    result.Add(new TaskViewModel
                    {
                        CreateBy = item.CreateBy,
                        CreateDate = item.CreateDate,
                        Description = item.Description,
                        DeadLine = item.DeadLine,
                        UpdateBy = item.UpdateBy,
                        UpdateDate = item.UpdateDate,
                        TaskCategoryId = item.TaskCategoryId,
                        TaskId = item.TaskId,
                        TaskCategory = item.TaskCategory?.Name

                    });
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging mechanism)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTask(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskCategory) // Include related TaskCategory
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        // Create a new task
        [HttpPost]
        public async Task<ActionResult<TaskViewModel>> PostTask([FromBody] TaskViewModel task)
        {
            if (task == null)
            {
                return BadRequest("Task data is required.");
            }

            // Check if the TaskCategory exists
            var taskCategory = await _context.TaskCategories.FindAsync(task.TaskCategoryId);
            if (taskCategory == null)
            {
                return BadRequest("Invalid TaskCategoryID. The specified category does not exist.");
            }
            // Assign default timestamps
            task.CreateDate = DateTime.Now;
            task.UpdateDate = DateTime.Now;
            var taskInfo = new Task
            {
                TaskId = task.TaskId,
                Description = task.Description,
                DeadLine = task.DeadLine,
                CreateBy = task.CreateBy,
                CreateDate = task.CreateDate,
                UpdateBy = task.UpdateBy,
                UpdateDate = task.UpdateDate,
                IsActive = task.IsActive,
                TaskCategoryId = task.TaskCategoryId,


            };
            // Save the task
            try
            {
                //task.TaskCategory = null; // Avoid circular reference
                _context.Tasks.Add(taskInfo);
                //int? tId=taskInfo.TaskId;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(task);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<TaskViewModel>> PutTask(int id, [FromBody] TaskViewModel task)
        {
            if (task == null)
            {
                return BadRequest("Task data is required.");
            }

            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the task exists
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound($"Task with ID {id} not found.");
            }

            // Check if the TaskCategory exists
            var taskCategory = await _context.TaskCategories.FindAsync(task.TaskCategoryId);
            if (taskCategory == null)
            {
                return BadRequest("Invalid TaskCategoryID. The specified category does not exist.");
            }

            // Update task fields
            existingTask.Description = task.Description;
            existingTask.UpdateBy = task.UpdateBy ?? existingTask.UpdateBy;
            existingTask.UpdateDate = DateTime.UtcNow; // UTC for consistency
            existingTask.IsActive = task.IsActive;
            existingTask.TaskCategoryId = task.TaskCategoryId;

            try
            {
                // Save changes to the database
                _context.Tasks.Update(existingTask);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(409, $"Concurrency error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            // Return updated task
            var updatedTask = new TaskViewModel
            {
                TaskId = existingTask.TaskId,
                Description = existingTask.Description,
                DeadLine = existingTask.DeadLine,
                CreateBy = existingTask.CreateBy,
                CreateDate = existingTask.CreateDate,
                UpdateBy = existingTask.UpdateBy,
                UpdateDate = existingTask.UpdateDate,
                IsActive = existingTask.IsActive,
                TaskCategoryId = existingTask.TaskCategoryId
            };

            return Ok(updatedTask);
        }

        // Delete a task
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success response
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound("The task does not exist.");
            }

            // Remove the task
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok("Delete successfully");
        }
        [HttpGet]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound("The task does not exist.");
            }

            // Remove the task
            task.TaskStatus = 2;
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok("Complete successfully");
        }

        // Helper method to check if a task exists
        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
        [HttpGet]
        public async Task<IActionResult> GetTaskCountByUsername(string username)
        {
            var user = HttpContext.User.Identity.Name;
            var NewTask = 0;
            var PendingTask = 0;
            var CompleteTask = 0;
            var TotalTask = 0;
            var RoleId = await _context.UserDetails.Where(x => x.UserName.ToLower() == username.ToLower()).Select(x => x.RoleId).FirstOrDefaultAsync();
            var userRole = await _context.UserRoles.Where(x => x.RoleId == RoleId).Select(x => x.RoleName).FirstOrDefaultAsync();
            if (userRole == "Member")
            {
                var memberId = await _context.UserDetails.Where(x => x.UserName == username).Select(x => x.MemberId).FirstOrDefaultAsync();
                var memberTaskLists = await _context.TaskAssigns.Where(x => x.MemberId == memberId).Select(x => x.TaskId).ToListAsync();
                //NewTask = await _context.Tasks.Where(x => x.TaskStatus == 0 && memberTaskLists.Contains(x.TaskId)).CountAsync();
                PendingTask = await _context.Tasks.Where(x => x.TaskStatus == 1 && memberTaskLists.Contains(x.TaskId)).CountAsync();
                CompleteTask = await _context.Tasks.Where(x => x.TaskStatus == 2 && memberTaskLists.Contains(x.TaskId)).CountAsync();
                TotalTask = PendingTask + CompleteTask;
                var memberTasks = await _context.Tasks.Where(x => memberTaskLists.Contains(x.TaskId) &&
                                                          (x.TaskStatus == 1 || x.TaskStatus == 2))
                                              .ToListAsync();
                return Ok(new { newTask = NewTask, pendingTask = PendingTask, completeTask = CompleteTask, totalTask = TotalTask, taskList = memberTasks });
            }
            else
            {
                // Admin এর জন্য সব টাস্ক গননা
                NewTask = await _context.Tasks.Where(x => x.TaskStatus == 0).CountAsync();
                PendingTask = await _context.Tasks.Where(x => x.TaskStatus == 1).CountAsync();
                CompleteTask = await _context.Tasks.Where(x => x.TaskStatus == 2).CountAsync();
                TotalTask = NewTask + PendingTask + CompleteTask;

                // Admin এর জন্য সব টাস্ক লিস্ট
                var allTasks = await _context.Tasks.ToListAsync();
                return Ok(new { newTask = NewTask, pendingTask = PendingTask, completeTask = CompleteTask, totalTask = TotalTask, taskList = allTasks });
            }
        }
    }
}
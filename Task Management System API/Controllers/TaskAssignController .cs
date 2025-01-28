using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Management_System_API.Models;
using Task_Management_System_API.ViewModels;

namespace Task_Management_System_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskAssignController : ControllerBase
    {
        private readonly TaskDbContext _context;

        public TaskAssignController(TaskDbContext context)
        {
            _context = context;
        }

        // GET: api/TaskAssign
        [HttpGet]
        public async Task<IActionResult> GetTaskAssignsWithDetails()
        {
            try
            {
                // Fetch TaskAssigns with navigation properties included
                var data = await _context.TaskAssigns
                    .Include(ta => ta.Member) // Includes Member navigation property
                    .Include(ta => ta.Task)  // Includes Task navigation property
                    .ThenInclude(t => t.TaskCategory) // Includes TaskCategory through Task
                    .ToListAsync();

                // Check if data is empty
                if (data == null || !data.Any())
                {
                    return NotFound("No task assignments found.");
                }

                // Map data to TaskAssignVM including related data
                var result = new List<TaskAssignVM>();
                foreach (var item in data) {
                    result.Add(new TaskAssignVM 
                {
                    TaskAssignId = item.TaskAssignId,
                    MemberId = item.MemberId,
                    MemberName = item.Member?.MemberName, // Assuming Member has a Name property
                    TaskId = item.TaskId,
                    Name = item.Task?.Description, // Assuming Task has a Description property
                    CreateBy = item.CreateBy,
                    CreateDate = item.CreateDate,
                    UpdateBy = item.UpdateBy,
                    UpdateDate = item.UpdateDate,
                    IsActive = item.IsActive,
               
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


        // GET: api/TaskAssign/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskAssignVM>> GetTaskAssign(int id)
        {
            var taskAssign = await _context.TaskAssigns
                .Include(ta => ta.Member)
                .FirstOrDefaultAsync(ta => ta.TaskAssignId == id);

            if (taskAssign == null)
            {
                return NotFound($"Task assignment with ID {id} not found.");
            }

            var result = new TaskAssignVM
            {
                TaskAssignId = taskAssign.TaskAssignId,
                MemberId = taskAssign.MemberId,
                TaskId = taskAssign.TaskAssignId,
                CreateBy = taskAssign.CreateBy,
                CreateDate = taskAssign.CreateDate,
                UpdateBy = taskAssign.UpdateBy,
                UpdateDate = taskAssign.UpdateDate,
                IsActive = taskAssign.IsActive,
                

            };

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> PostTaskAssign([FromBody] TaskAssignVM taskAssignVM)
        {
            // Validate input
            if (taskAssignVM == null)
            {
                return BadRequest("Task assignment data is required.");
            }

            // Validate member existence
            var member = await _context.Members.FindAsync(taskAssignVM.MemberId);
            if (member == null)
            {
                return BadRequest("Invalid MemberID. The specified member does not exist.");
            }

            // Validate task existence
            var task = await _context.Tasks.FindAsync(taskAssignVM.TaskId);
            if (task == null)
            {
                return BadRequest("Invalid TaskID. The specified task does not exist.");
            }

            // Map data to entity
            var taskAssign = new TaskAssign
            {
                MemberId = taskAssignVM.MemberId,
                TaskId = taskAssignVM.TaskId,
                CreateBy = taskAssignVM.CreateBy,
                CreateDate = DateTime.UtcNow,
                IsActive = taskAssignVM.IsActive
            };

            try
            {
                // Save task assignment
                _context.TaskAssigns.Add(taskAssign);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok(new
            {
                message = "Task assigned successfully",
                taskAssignId = taskAssign.TaskAssignId,
                memberId = taskAssign.MemberId,
                taskId = taskAssign.TaskId,
                

            });
        }

        // PUT: api/TaskAssign/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskAssignVM>> UpdateTaskAssign(int id, [FromBody] TaskAssignVM taskAssignVM)
        {
            if (taskAssignVM == null)
            {
                return BadRequest("Task assignment data is required.");
            }

            // Find the existing task assignment
            var existingTaskAssign = await _context.TaskAssigns.FindAsync(id);
            if (existingTaskAssign == null)
            {
                return NotFound($"Task assignment with ID {id} not found.");
            }

            // Validate Member ID
            var member = await _context.Members.FindAsync(taskAssignVM.MemberId);
            if (member == null)
            {
                return BadRequest("Invalid MemberID. The specified member does not exist.");
            }

            // Update the task assignment details
            existingTaskAssign.MemberId = taskAssignVM.MemberId;
            existingTaskAssign.TaskId = taskAssignVM.TaskId;
            existingTaskAssign.UpdateBy = taskAssignVM.UpdateBy ?? existingTaskAssign.UpdateBy;
            existingTaskAssign.UpdateDate = DateTime.UtcNow;
            existingTaskAssign.IsActive = taskAssignVM.IsActive;

            try
            {
                _context.TaskAssigns.Update(existingTaskAssign);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            // Get additional details for memberName and task name
            var task = await _context.Tasks.FindAsync(taskAssignVM.TaskId);
            var updatedTaskAssign = new TaskAssignVM
            {
                TaskAssignId = existingTaskAssign.TaskAssignId,
                TaskId = existingTaskAssign.TaskId,
                MemberId = existingTaskAssign.MemberId,
                CreateBy = existingTaskAssign.CreateBy,
                CreateDate = existingTaskAssign.CreateDate,
                UpdateBy = existingTaskAssign.UpdateBy,
                UpdateDate = existingTaskAssign.UpdateDate,
                IsActive = existingTaskAssign.IsActive,
                MemberName = member?.MemberName, // Assuming the Member entity has a "Name" property
                Name = task?.Description          // Assuming the Task entity has a "Name" property
            };

            return Ok(updatedTaskAssign);
        }

        // DELETE: api/TaskAssign/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAssign(int id)
        {
            var taskAssign = await _context.TaskAssigns.FindAsync(id);
            if (taskAssign == null)
            {
                return NotFound($"Task assignment with ID {id} not found.");
            }

            try
            {
                _context.TaskAssigns.Remove(taskAssign);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok("Delete Successfully");
        }
        //[HttpGet("GetTasksByMember/{memberId}")]
        //public IActionResult GetTasksByMember(int memberId)
        //{
        //    var tasks = _context.TaskAssigns
        //        .Where(t => t.MemberId == memberId)
        //        .Select(t => new
        //        {
        //            t.TaskAssignId,
                    
        //            t.Task.Description,
        //            t.Task.DeadLine
        //        }).ToList();

        //    return Ok(tasks);
        //}

    }
}

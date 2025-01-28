using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Task_Management_System_API.Models;

namespace Task_Management_System_API.ViewModels
{
    public class UserPayload
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int TokenExpire { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public  class TaskViewModel

    {
        public int TaskId { get; set; }

        public string Description { get; set; }

        public DateTime DeadLine { get; set; }

        public int TaskCategoryId { get; set; }
        public  string TaskCategory { get; set; }

        public string CreateBy { get; set; }

        public DateTime CreateDate { get; set; }

        public string UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool IsActive { get; set; }

      
    }
}

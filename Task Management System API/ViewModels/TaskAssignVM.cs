using Task_Management_System_API.Models;
using Task = Task_Management_System_API.Models.Task;

namespace Task_Management_System_API.ViewModels
{
    public class TaskAssignVM
    {
        public int TaskAssignId { get; set; }
        public int? MemberId { get; set; }

        public string CreateBy { get; set; }
        public int? TaskId { get; set; }

        public DateTime? CreateDate { get; set; }

        public string UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool? IsActive { get; set; }
        public object MemberName { get; internal set; }
        public string Name { get; internal set; }
    }
}

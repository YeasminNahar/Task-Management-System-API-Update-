using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class TaskDetail
{
    public int TaskDetailId { get; set; }

    public int TaskId { get; set; }

    public string Description { get; set; }

    public DateTime DeadLine { get; set; }

    public bool IsComplete { get; set; }

    public string CompleteBy { get; set; }

    public DateTime? CompleteTime { get; set; }

    public int Priority { get; set; }

    public string CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public string UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<FileAttach> FileAttaches { get; set; } = new List<FileAttach>();

    public virtual Task Task { get; set; }
}

using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class TaskCategory
{
    public int TaskCategoryId { get; set; }

    public string Name { get; set; }

    public string CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public string UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}

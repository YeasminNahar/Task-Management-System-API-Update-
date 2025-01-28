using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class TaskAssign
{
    public int TaskAssignId { get; set; }

    public int? TaskDetailId { get; set; }

    public int? TaskId { get; set; }

    public int? MemberId { get; set; }

    public string CreateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Member Member { get; set; }

    public virtual Task Task { get; set; }
}

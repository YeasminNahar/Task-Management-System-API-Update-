using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public string Description { get; set; }

    public DateTime DeadLine { get; set; }

    public int TaskCategoryId { get; set; }

    public string CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public string UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsActive { get; set; }

    //public virtual ICollection<MemberInvitation> MemberInvitations { get; set; } = new List<MemberInvitation>();

    public virtual ICollection<TaskAssign> TaskAssigns { get; set; } = new List<TaskAssign>();

    public virtual TaskCategory TaskCategory { get; set; }

    //public virtual ICollection<TaskDetail> TaskDetails { get; set; } = new List<TaskDetail>();
}

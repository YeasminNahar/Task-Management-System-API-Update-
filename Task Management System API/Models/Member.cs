using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string MemberName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public string UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TaskAssign> TaskAssigns { get; set; } = new List<TaskAssign>();

    public virtual ICollection<UserDetail> UserDetails { get; set; } = new List<UserDetail>();
}

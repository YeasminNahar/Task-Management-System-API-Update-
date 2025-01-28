using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class UserDetail
{
    public int UserId { get; set; }

    public string UserName { get; set; }

    public string PassWord { get; set; }

    public string RoleId { get; set; }

    public int? MemberId { get; set; }

    public virtual Member Member { get; set; }
}

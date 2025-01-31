using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class MemberInvitation
{
    public int SendId { get; set; }

    public DateTime SendTime { get; set; }

    public string Email { get; set; }

    public string SendBy { get; set; }

    public int TaskId { get; set; }

    public virtual Task Task { get; set; }
}

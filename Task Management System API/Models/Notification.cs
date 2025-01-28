using System;
using System.Collections.Generic;

namespace Task_Management_System_API.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string NotificationEvent { get; set; }
}

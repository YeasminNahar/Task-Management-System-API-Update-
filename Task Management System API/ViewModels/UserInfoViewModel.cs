namespace Task_Management_System_API.ViewModels
{
    public class UserInfoViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Token { get; set; }

        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public int? MemberId { get; set; }

    }
}

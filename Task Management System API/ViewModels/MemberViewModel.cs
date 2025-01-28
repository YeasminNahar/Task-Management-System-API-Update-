namespace Task_Management_System_API.ViewModels
{
    public class MemberViewModel
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
    }
}

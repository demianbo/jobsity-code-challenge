using System.ComponentModel.DataAnnotations;

namespace MrRobotRoomChat.Request
{
    public class UserRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class PasswordDto
    {
        [Required(ErrorMessage = "Nhập mật khẩu hiện tại"), MaxLength(100)]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "Nhập mật khẩu mới"), MaxLength(100)]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Nhập mật khẩu xác nhận")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = "";

    }
}

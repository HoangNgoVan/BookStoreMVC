using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Chưa nhập First Name field is required"), MaxLength(150)]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "Chưa nhập Last Name field is required"), MaxLength(150)]
        public string LastName { get; set; } = "";
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";
        [Phone(ErrorMessage = "Định danh số điện thoại không chính xác"), MaxLength(50)]
        public string? PhoneNumber { get; set; }
        [Required, MaxLength(200)]
        public string Address { get; set; } = "";
        [Required, MaxLength(100)]
        public string Password { get; set; } = "";
        [Required(ErrorMessage = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = "";
    }
}

namespace Limen.Samples.Models;

public class User
{
    [Required(ErrorMessage = "名字不能为空")]
    [MinLength(2, ErrorMessage = "名字不能少于 2 个字符")]
    public string? Name { get; set; }

    [Min(18, ErrorMessage = "年龄不能小于 18 岁")]
    public int Age { get; set; }

    [Required(ErrorMessage = "密码不能为空")]
    [Compare(nameof(ConfirmPassword), ErrorMessage = "两次输入的密码不一致")]
    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }
}
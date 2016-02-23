using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Online.Web.Models
{
   
    public class LoginViewModel
    {
        [Required(ErrorMessage = "请输入邮箱")]
        [Display(Name = "电子邮件")]
        //[EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "验证码")]
        public string Code { get; set; }

        [Display(Name = "")]
        public string Message { get; set; }
    }

     
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        public string Message { get; set; }
    }


    #region 个人中心

    public class UserBaseViewModel
    {
        public string UserName { get; set; }
        public string Avatar { get; set; }

        public string RealName { get; set; }

        public string QQ { get; set; }

        public string Weixin { get; set; }

        public int Sex { get; set; }

        public int InvestmentType { get; set; }
    }

    public class UserTagsViewModel
    {
        public List<Online.DbHelper.Model.UC_Dictionarys> List { get; set; }

        public string Tags { get; set; }
    }

    public class UserAddressViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "手机")]
        [RegularExpression(@"^1[3458][0-9]{9}$", ErrorMessage = "手机号格式不正确")]
        public string Telephone { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        [Required]
        [Display(Name = "具体地址")]
        public string DetailInfo { get; set; }

        public int AddressID { get; set; }
    }

    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "请输入邮箱")]
        [EmailAddress(ErrorMessage = "邮箱格式错误，请重新输入")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
        [Required(ErrorMessage="请输入手机")]
        [Display(Name = "手机")]
        [RegularExpression(@"^1[3458][0-9]{9}$", ErrorMessage = "手机号格式不正确")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "请输入昵称")]
        [Display(Name = "昵称")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "请输入确认密码")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "QQ")]
        public string QQ { get; set; }

        [Required(ErrorMessage = "请输入手机验证码")]
        [Display(Name = "手机验证码")]
        public string VerifyPhoneCode { get; set; }
        [Required]
        public bool RedChecked { get; set; }

        public string Token { get; set; }

        public string Message { get; set; } = string.Empty;
    }


    #endregion
}

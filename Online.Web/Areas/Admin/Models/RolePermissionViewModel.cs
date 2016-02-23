using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online.Web.Areas.Admin.Models
{
    public class RolePermissionViewModel
    {
        public long? RoleID { get; set; }

        public string RoleName { get; set; }

        public List<string> Permissions { get; set; }

    }

    public class RolePermissionEditViewModel
    {
        public long? RoleID { get; set; }
        public List<int?> Permissions { get; set; }
    }

    public class TvScheduleViewModel
    {
        public string LiveRoom { get; set; }

        public DateTime EffectiveStartTime { get; set; }
        public DateTime EffectiveEndTime { get; set; }


        public List<TeacherClass> TeacherClass { get; set; }
    }

    public class TeacherClass
    {
        public string Teacher { get; set; }
     

        public string LiveStartTime { get; set; }
        public string LiveEndTime { get; set; }


        public string NickName { get; set; }

        public int SCId { get; set; }
    }

    public class LoginViewModel
    {
        public LoginViewModel()
        {
            Message = string.Empty;
        }

        [Required(ErrorMessage = "请输入用户名或者邮箱")]
        [Display(Name = "电子邮件")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Message { get; set; }
    }
}
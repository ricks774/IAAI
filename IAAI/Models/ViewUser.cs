using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IAAI.Models
{
    public class ViewUser
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(50)]
        [Display(Name = "帳號")]
        public string Account { get; set; }

        /// <summary>
        /// 密碼, 必須為6-12位英文字母和數字的組合
        /// </summary>
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }
    }
}
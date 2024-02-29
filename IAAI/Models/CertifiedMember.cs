using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IAAI.Models
{
    public class CertifiedMember
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "照片")]
        public string Picture { get; set; }

        [Display(Name = "名稱")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "姓氏")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "國家")]
        public string Country { get; set; }

        [Display(Name = "職稱")]
        public string Title { get; set; }

        [Display(Name = "公司")]
        public string Company { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
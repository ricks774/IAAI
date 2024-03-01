using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IAAI.Models
{
    public class Business
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [AllowHtml]
        [Display(Name = "協會業務")]
        public string Scope { get; set; }

        [AllowHtml]
        [Display(Name = "訓練、認證、發照")]
        public string Accreditation { get; set; }

        [Display(Name = "諮詢、顧問")]
        public string Consult { get; set; }

        [Display(Name = "縱火查詢")]
        public string Investigation { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
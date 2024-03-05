using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IAAI.Models
{
    public class Knowledge
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        [Display(Name = "知識庫名稱")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "知識庫內容")]
        public string Content { get; set; }

        [Display(Name = "發布時間")]
        public DateTime? PubDate { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
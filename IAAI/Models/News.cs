using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IAAI.Models
{
    public class News
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(200)]
        [Display(Name = "新聞名稱")]
        public string Title { get; set; }

        [Display(Name = "新聞內容")]
        public string Content { get; set; }

        [Display(Name = "發布時間")]
        public DateTime? PubDate { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
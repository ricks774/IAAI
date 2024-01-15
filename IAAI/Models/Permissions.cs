using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IAAI.Models
{
    public class Permissions
    {
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(20)]
        [Display(Name = "權限名稱")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [MaxLength(10)]
        [Display(Name = "值")]
        public string Value { get; set; }

        [Display(Name = "上一層")]
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Permissions Parent { get; set; }

        public virtual List<Permissions> Children { get; set; }
    }
}
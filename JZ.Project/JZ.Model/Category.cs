/*
 * 本文件由根据实体插件自动生成，请勿更改
 * =========================== */

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JZ.Manage
{
    public class Category 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name="category_id")]
        public int category_id{ get; set; }

        [StringLength(128)]
        [Display(Name="category_name")]
        public string category_name{ get; set; }

        [StringLength(128)]
        [Display(Name="category_type")]
        public string category_type{ get; set; }

        [Display(Name="remark")]
        public string remark{ get; set; }

        [Display(Name="is_deleted")]
        public bool? is_deleted{ get; set; }

        [Display(Name="insert_date")]
        public DateTime? insert_date{ get; set; }

        [Display(Name="insert_user_id")]
        public int? insert_user_id{ get; set; }

        [Display(Name="update_date")]
        public DateTime? update_date{ get; set; }

        [Display(Name="update_user_id")]
        public int? update_user_id{ get; set; }

    }
}
/*
 * 本文件由根据实体插件自动生成，请勿更改
 * =========================== */

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JZ.Manage
{
    public class Types 
    {

        [Key]
        [Required]
        [StringLength(128)]
        [Display(Name="type_code")]
        public string type_code{ get; set; }

        [StringLength(256)]
        [Display(Name="type_name")]
        public string type_name{ get; set; }

        [StringLength(128)]
        [Display(Name="type_category")]
        public string type_category{ get; set; }

        [Display(Name="is_deleted")]
        public bool? is_deleted{ get; set; }

        [Display(Name="orderby")]
        public int? orderby{ get; set; }

        [Display(Name="insert_user_id")]
        public int? insert_user_id{ get; set; }

        [Display(Name="insert_date")]
        public DateTime? insert_date{ get; set; }

        [Display(Name="update_user_id")]
        public int? update_user_id{ get; set; }

        [Display(Name="update_date")]
        public DateTime? update_date{ get; set; }

    }
}
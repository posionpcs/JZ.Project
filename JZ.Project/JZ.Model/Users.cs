/*
 * 本文件由根据实体插件自动生成，请勿更改
 * =========================== */

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JZ.Manage
{
    public class Users 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Display(Name="user_id")]
        public int user_id{ get; set; }

        [StringLength(128)]
        [Display(Name="user_code")]
        public string user_code{ get; set; }

        [StringLength(128)]
        [Display(Name="user_name")]
        public string user_name{ get; set; }

        [StringLength(128)]
        [Display(Name="user_pwd")]
        public string user_pwd{ get; set; }

        [Display(Name="sex")]
        public bool? sex{ get; set; }

        [StringLength(64)]
        [Display(Name="phone_num")]
        public string phone_num{ get; set; }

        [StringLength(64)]
        [Display(Name="tel_num")]
        public string tel_num{ get; set; }

        [StringLength(64)]
        [Display(Name="address")]
        public string address{ get; set; }

        [Display(Name="sign_info")]
        public string sign_info{ get; set; }

        [Display(Name="last_login_date")]
        public DateTime? last_login_date{ get; set; }

        [StringLength(128)]
        [Display(Name="last_login_ip")]
        public string last_login_ip{ get; set; }

        [StringLength(256)]
        [Display(Name="last_login_address")]
        public string last_login_address{ get; set; }

        [Display(Name="is_deleted")]
        public bool? is_deleted{ get; set; }

        [Display(Name="is_actived")]
        public bool? is_actived{ get; set; }

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
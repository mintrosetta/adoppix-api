using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("UserFollows", Schema = "adoppix_admin")]
    public partial class UserFollow
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("fromUserId")]
        [StringLength(450)]
        public string FromUserId { get; set; }
        [Required]
        [Column("toUserId")]
        [StringLength(450)]
        public string ToUserId { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }

        [ForeignKey("FromUserId")]
        [InverseProperty("UserFollowFromUsers")]
        public virtual User FromUser { get; set; }
        [ForeignKey("ToUserId")]
        [InverseProperty("UserFollowToUsers")]
        public virtual User ToUser { get; set; }
    }
}

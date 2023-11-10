using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AdopPixAPI.DataAccess.Models
{
    [Table("Users", Schema = "adoppix_admin")]
    public partial class User
    {
        public User()
        {
            AuctionBids = new HashSet<AuctionBid>();
            AuctionWons = new HashSet<AuctionWon>();
            Auctions = new HashSet<Auction>();
            PostComments = new HashSet<PostComment>();
            PostLikes = new HashSet<PostLike>();
            Posts = new HashSet<Post>();
            ProductUsers = new HashSet<ProductUser>();
            Products = new HashSet<Product>();
            UserBanks = new HashSet<UserBank>();
            UserCreditCards = new HashSet<UserCreditCard>();
            UserFollowFromUsers = new HashSet<UserFollow>();
            UserFollowToUsers = new HashSet<UserFollow>();
        }

        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Column("roleId")]
        public int RoleId { get; set; }
        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Column("isConfirmEmail")]
        public bool IsConfirmEmail { get; set; }
        [Column("phoneNumber")]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [Column("isConfirmPhoneNumber")]
        public bool IsConfirmPhoneNumber { get; set; }
        [Required]
        [Column("passwordHash")]
        public byte[] PasswordHash { get; set; }
        [Required]
        [Column("passwordSalt")]
        public byte[] PasswordSalt { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime? Updated { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("Users")]
        public virtual UserRole Role { get; set; }
        [InverseProperty("User")]
        public virtual UserProfile UserProfile { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AuctionWon> AuctionWons { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Auction> Auctions { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<PostComment> PostComments { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<PostLike> PostLikes { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Post> Posts { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<ProductUser> ProductUsers { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<Product> Products { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserBank> UserBanks { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserCreditCard> UserCreditCards { get; set; }
        [InverseProperty("FromUser")]
        public virtual ICollection<UserFollow> UserFollowFromUsers { get; set; }
        [InverseProperty("ToUser")]
        public virtual ICollection<UserFollow> UserFollowToUsers { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TripleTBE.Models;

[Index("Email", Name = "UQ__Users__A9D10534DD2F8A91", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(20)]
    public string? Role { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("User")]
    [JsonIgnore]
    public virtual Cart? Cart { get; set; }

    [InverseProperty("User")]
    [JsonIgnore]
    public virtual ICollection<News> News { get; set; } = new List<News>();

    [InverseProperty("User")]
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("User")]
    [JsonIgnore]
    public virtual UserProfile? UserProfile { get; set; }

    /* =========================================================
       NỘI DUNG BỔ SUNG: COURT BOOKINGS & CHATTING
    ========================================================= */

    // Danh sách các sân mà User này sở hữu (Dành cho Role = 'CourtOwner')
    [InverseProperty("Owner")]
    [JsonIgnore]
    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();

    // Danh sách đơn đặt sân của User này (Dành cho Role = 'Customer')
    [InverseProperty("User")]
    [JsonIgnore]
    public virtual ICollection<CourtBooking> CourtBookings { get; set; } = new List<CourtBooking>();

    // Các phòng chat với tư cách là Khách hàng mua/đặt sân
    [InverseProperty("Customer")]
    [JsonIgnore]
    public virtual ICollection<ChatRoom> CustomerChatRooms { get; set; } = new List<ChatRoom>();

    // Các phòng chat với tư cách là Chủ sân hỗ trợ khách
    [InverseProperty("Owner")]
    [JsonIgnore]
    public virtual ICollection<ChatRoom> OwnerChatRooms { get; set; } = new List<ChatRoom>();

    // Tất cả tin nhắn mà User này đã gửi đi
    [InverseProperty("Sender")]
    [JsonIgnore]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
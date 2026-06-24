using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripleTBE.Models;

namespace TripleTBE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly BadmintonDbContext _context;

    public ChatController(BadmintonDbContext context)
    {
        _context = context;
    }

    // 1. Lấy hoặc Tạo phòng chat tự động giữa Khách và Chủ Sân
    // GET: api/Chat/GetOrCreateRoom?customerId=2&ownerId=3
    [HttpGet("GetOrCreateRoom")]
    public async Task<ActionResult<ChatRoom>> GetOrCreateRoom([FromQuery] int customerId, [FromQuery] int ownerId)
    {
        var room = await _context.ChatRooms
            .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.OwnerId == ownerId);

        if (room == null)
        {
            room = new ChatRoom
            {
                CustomerId = customerId,
                OwnerId = ownerId,
                CreatedAt = DateTime.Now
            };
            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();
        }

        return room;
    }

    // 2. Lấy danh sách tin nhắn của một phòng chat (Sắp xếp theo thời gian tăng dần để hiển thị)
    // GET: api/Chat/Room/1/Messages
    [HttpGet("Room/{roomId}/Messages")]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetRoomMessages(int roomId)
    {
        return await _context.ChatMessages
            .Where(m => m.RoomId == roomId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    // 3. Gửi tin nhắn mới vào phòng chat
    // POST: api/Chat/SendMessage
    [HttpPost("SendMessage")]
    public async Task<ActionResult<ChatMessage>> SendMessage(ChatMessage message)
    {
        message.SentAt = DateTime.Now;
        message.IsRead = false;

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        return Ok(message);
    }

    // 4. Đánh dấu tất cả tin nhắn trong phòng là ĐÃ ĐỌC (Khi đối phương click vào xem phòng chat)
    // PUT: api/Chat/Room/1/MarkAsRead
    [HttpPut("Room/{roomId}/MarkAsRead")]
    public async Task<IActionResult> MarkAsRead(int roomId, [FromQuery] int readerId)
    {
        // Tìm các tin nhắn của người KHÁC gửi trong phòng này mà chưa được đọc
        var unreadMessages = await _context.ChatMessages
            .Where(m => m.RoomId == roomId && m.SenderId != readerId && m.IsRead == false)
            .ToListAsync();

        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Đã đánh dấu đọc toàn bộ tin nhắn." });
    }
}
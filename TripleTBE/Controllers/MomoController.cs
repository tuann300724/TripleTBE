using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TripleTBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MomoController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // ================= CẤU HÌNH MÔI TRƯỜNG THẬT 100% =================
        private const string PartnerCode = "MOMOLAER20260329";
        private const string AccessKey = "VOqWT7PApxTRcDdD";
        private const string SecretKey = "zeynEz1XQReSDazeYiP8nQkCoAl1kl0h"; // Chuẩn chữ Es theo ảnh
        private const string MomoEndpoint = "https://payment.momo.vn/v2/gateway/api/create"; // Endpoint Real

        public MomoController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("create-momo")]
        public async Task<IActionResult> CreateMomo([FromBody] CreateMomoRequest req)
        {
            var requestId = $"{PartnerCode}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var orderId = req.OrderId;

            long amount = req.Amount;
            var extraData = string.IsNullOrEmpty(req.ExtraData) ? "" : req.ExtraData;
            var orderInfo = req.OrderInfo ?? "";

            // Link IPN ảo để không bị lỗi chữ ký localhost trên môi trường thật
            var finalIpnUrl = string.IsNullOrEmpty(req.IpnUrl) || req.IpnUrl.Contains("localhost")
                ? "https://google.com/momo-ipn"
                : req.IpnUrl;

            // ================= MÃ HÓA URL THEO TIÊU CHUẨN MOMO REAL =================
            // Môi trường thật bắt buộc phải mã hóa khoảng trắng thành %20 trước khi băm chữ ký
            var encodedOrderInfo = Uri.EscapeDataString(orderInfo);

            // ================= RAW SIGNATURE REAL =================
            var rawSignature =
                $"accessKey={AccessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={finalIpnUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={encodedOrderInfo}" + // Bắt buộc dùng chuỗi đã mã hóa
                $"&partnerCode={PartnerCode}" +
                $"&redirectUrl={req.RedirectUrl}" +
                $"&requestId={requestId}" +
                $"&requestType=captureWallet";

            var signature = ComputeHmacSha256(rawSignature, SecretKey);

            // ================= BODY JSON REAL =================
            var body = new
            {
                partnerCode = PartnerCode,
                accessKey = AccessKey,
                requestId,
                amount,
                orderId,
                orderInfo = encodedOrderInfo, // Gửi chuỗi đã mã hóa sang MoMo
                redirectUrl = req.RedirectUrl,
                ipnUrl = finalIpnUrl,
                requestType = "captureWallet",
                extraData,
                signature,
                lang = "vi"
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(MomoEndpoint, content);
            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        private static string ComputeHmacSha256(string message, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    public class CreateMomoRequest
    {
        public string OrderId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public string ExtraData { get; set; } = "";
    }
}
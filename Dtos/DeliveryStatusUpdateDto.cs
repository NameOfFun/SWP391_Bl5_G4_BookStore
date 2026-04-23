using System.ComponentModel.DataAnnotations;

namespace BookStore.Dtos
{
    public class DeliveryStatusUpdateDto
    {
        [Required]
        public int OrderId { get; set; }

        /// <summary>True: Delivered, False: Failed</summary>
        [Required]
        public bool IsSuccess { get; set; }

        /// <summary>Dành cho trường hợp giao thất bại (Không liên lạc được, Sai địa chỉ, Khách không nhận)</summary>
        public string? FailedReason { get; set; }

        /// <summary>Ghi chú thêm từ shipper</summary>
        public string? Note { get; set; }

        /// <summary>Ảnh minh chứng giao hàng</summary>
        public Microsoft.AspNetCore.Http.IFormFile? ProofImage { get; set; }

        /// <summary>Đường dẫn ảnh minh chứng sau khi lưu</summary>
        public string? ProofImagePath { get; set; }
    }
}

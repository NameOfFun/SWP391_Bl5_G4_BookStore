using BookStore.Dtos.Admin.Voucher;

namespace BookStore.Service.Interfaces;

public interface IVoucherService
{
    Task<(IReadOnlyList<VoucherListDto> Items, int TotalCount)> GetPagedAsync(
        string? search, string? status, string? validity, int page, int pageSize);

    Task<UpdateVoucherDto?> GetForEditAsync(int id);

    Task CreateAsync(CreateVoucherDto dto, string userId);

    Task UpdateAsync(int id, UpdateVoucherDto dto, string userId);

    Task ToggleStatusAsync(int id, string userId);
}

using BookStore.Dtos.Admin.Voucher;
using BookStore.Models;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Implements;

public class VoucherService : IVoucherService
{
    private readonly BookStoreDbContext _context;

    public VoucherService(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<VoucherListDto> Items, int TotalCount)> GetPagedAsync(
        string? search, string? status, string? validity, int page, int pageSize)
    {
        var now = DateTime.Today;
        var query = _context.Vouchers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(v =>
                v.Code.Contains(s) ||
                (v.Description != null && v.Description.Contains(s)));
        }

        if (status == "active")
            query = query.Where(v => v.IsActive);
        else if (status == "inactive")
            query = query.Where(v => !v.IsActive);

        if (validity == "valid")
            query = query.Where(v =>
                (v.ValidFrom == null || v.ValidFrom <= now) &&
                (v.ValidTo == null || v.ValidTo >= now));
        else if (validity == "expired")
            query = query.Where(v => v.ValidTo != null && v.ValidTo < now);
        else if (validity == "notstarted")
            query = query.Where(v => v.ValidFrom != null && v.ValidFrom > now);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VoucherListDto
            {
                VoucherId = v.VoucherId,
                Code = v.Code,
                Description = v.Description,
                DiscountType = v.DiscountType,
                DiscountValue = v.DiscountValue,
                MinOrderAmount = v.MinOrderAmount,
                UsageLimit = v.UsageLimit,
                TimesUsed = v.TimesUsed,
                ValidFrom = v.ValidFrom,
                ValidTo = v.ValidTo,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt
            })
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<UpdateVoucherDto?> GetForEditAsync(int id)
    {
        var v = await _context.Vouchers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.VoucherId == id);

        if (v == null) return null;

        return new UpdateVoucherDto
        {
            VoucherId = v.VoucherId,
            Code = v.Code,
            Description = v.Description,
            DiscountType = v.DiscountType,
            DiscountValue = v.DiscountValue,
            MinOrderAmount = v.MinOrderAmount,
            ValidFrom = v.ValidFrom,
            ValidTo = v.ValidTo,
            UsageLimit = v.UsageLimit,
            IsActive = v.IsActive,
            TimesUsed = v.TimesUsed
        };
    }

    public async Task CreateAsync(CreateVoucherDto dto, string userId)
    {
        var code = dto.Code.Trim().ToUpperInvariant();

        var duplicate = await _context.Vouchers.AnyAsync(v => v.Code == code);
        if (duplicate)
            throw new ArgumentException("Mã voucher đã tồn tại.");

        ValidateDiscountType(dto.DiscountType, dto.DiscountValue);
        ValidateDates(dto.ValidFrom, dto.ValidTo);

        var entity = new Voucher
        {
            Code = code,
            Description = dto.Description?.Trim(),
            DiscountType = dto.DiscountType,
            DiscountValue = dto.DiscountValue,
            MinOrderAmount = dto.MinOrderAmount,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            UsageLimit = dto.UsageLimit,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastManagedByUserId = userId
        };

        _context.Vouchers.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateVoucherDto dto, string userId)
    {
        var entity = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == id);
        if (entity == null)
            throw new InvalidOperationException($"Không tìm thấy voucher có Id = {id}.");

        ValidateDiscountType(dto.DiscountType, dto.DiscountValue);
        ValidateDates(dto.ValidFrom, dto.ValidTo);

        entity.Description = dto.Description?.Trim();
        entity.DiscountType = dto.DiscountType;
        entity.DiscountValue = dto.DiscountValue;
        entity.MinOrderAmount = dto.MinOrderAmount;
        entity.ValidFrom = dto.ValidFrom;
        entity.ValidTo = dto.ValidTo;
        entity.UsageLimit = dto.UsageLimit;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.LastManagedByUserId = userId;

        await _context.SaveChangesAsync();
    }

    public async Task ToggleStatusAsync(int id, string userId)
    {
        var entity = await _context.Vouchers.FirstOrDefaultAsync(v => v.VoucherId == id);
        if (entity == null)
            throw new InvalidOperationException($"Không tìm thấy voucher có Id = {id}.");

        entity.IsActive = !entity.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.LastManagedByUserId = userId;

        await _context.SaveChangesAsync();
    }

    private static void ValidateDiscountType(string discountType, decimal discountValue)
    {
        if (discountType == "P" && (discountValue < 1 || discountValue > 100))
            throw new ArgumentException("Giảm giá phần trăm phải trong khoảng 1–100.");
    }

    private static void ValidateDates(DateTime? validFrom, DateTime? validTo)
    {
        if (validFrom.HasValue && validTo.HasValue && validFrom >= validTo)
            throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
    }
}

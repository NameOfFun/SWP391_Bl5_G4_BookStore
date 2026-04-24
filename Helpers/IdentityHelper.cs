using Microsoft.AspNetCore.Identity;

namespace BookStore.Helpers;

public static class IdentityHelper
{
    public static string GetErrors(IdentityResult result) =>
        string.Join(" ", result.Errors.Select(e => e.Description));
}

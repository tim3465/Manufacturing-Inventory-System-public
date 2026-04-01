using CncApp.Application.Dtos.ShiftIssueLogs;

namespace CncApp.Application.Services.ShiftIssueLogs;

public partial class ShiftIssueLogService
{
    public async Task<List<ShiftIssueLogResultDto>> GetByShiftAsync(int shiftId, CancellationToken ct = default)
    {
        var logs = await _shiftIssueLogRepository.ListByShiftAsync(shiftId, ct);
        var dtos = _mapper.Map<List<ShiftIssueLogResultDto>>(logs);

        // Resolve creator display names from User entities
        var userIds = logs
            .Where(l => l.CreatedByUserId.HasValue)
            .Select(l => l.CreatedByUserId!.Value)
            .Distinct()
            .ToList();

        if (userIds.Count > 0)
        {
            var users = await _userRepository.ListAllAsync(ct);
            var userLookup = users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionary(u => u.Id);

            foreach (var dto in dtos)
            {
                if (dto.CreatedByUserId.HasValue && userLookup.TryGetValue(dto.CreatedByUserId.Value, out var user))
                {
                    dto.CreatedByUserDisplayName = user.FirstName != null && user.LastName != null
                        ? $"{user.FirstName} {user.LastName}"
                        : user.UserName;
                }
            }
        }

        return dtos;
    }
}

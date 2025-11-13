using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Data;
using Universe.Api.Models;

[ApiController]
[Route("api/gamification")]
public class GamificationController : ControllerBase
{
    private readonly AppDbContext _context;
    public GamificationController(AppDbContext context) => _context = context;

    // GET: api/gamification/user/{userId}
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserStats(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with ID {userId} not found" });

        var stats = await _context.UserStats
            .Include(s => s.UserBadges).ThenInclude(ub => ub.Badge)
            .Include(s => s.UserRewards).ThenInclude(ur => ur.Reward)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (stats == null)
        {
            stats = new UserStats { UserID = userId, User = user, Points = 0, Streak = 0 };
            _context.UserStats.Add(stats);
            await _context.SaveChangesAsync();
        }

        return Ok(MapToUserStatsDto(stats));
    }

    // GET: api/gamification/leaderboard
    [HttpGet("leaderboard")]
    [Authorize]
    public async Task<IActionResult> GetLeaderboard()
    {
        var leaderboard = await _context.UserStats
            .Include(s => s.User)
            .OrderByDescending(s => s.Points)
            .Take(50)
            .Select(s => new LeaderboardEntryDto
            {
                UserID = s.UserID,
                Username = s.User.Username,
                Points = s.Points
            })
            .ToListAsync();

        return Ok(leaderboard);
    }

    // GET: api/gamification/rewards
    [HttpGet("rewards")]
    [Authorize]
    public async Task<IActionResult> GetAvailableRewards()
    {
        var rewards = await _context.Rewards
            .OrderBy(r => r.CostPoints)
            .Select(r => new RewardDto
            {
                RewardId = r.RewardId,
                Title = r.Title,
                CostPoints = r.CostPoints,
                RedeemedAt = DateTime.MinValue // available rewards not redeemed yet
            })
            .ToListAsync();

        return Ok(rewards);
    }


    // POST: api/gamification/user/{userId}/addpoints?points=10
    [HttpPost("user/{userId}/addpoints")]
    [Authorize]
    public async Task<IActionResult> AddPoints(int userId, [FromQuery] int points)
    {
        var stats = await EnsureUserStatsExists(userId);
        stats.Points += points;
        await _context.SaveChangesAsync();
        await CheckAndAwardBadges(stats);

        return Ok(MapToUserStatsDto(stats));
    }

    // POST: api/gamification/user/{userId}/play
    [HttpPost("user/{userId}/play")]
    [Authorize]
    public async Task<IActionResult> PlayMiniGame(int userId, [FromQuery] string gameType = "spin")
    {
        var stats = await EnsureUserStatsExists(userId);
        var rnd = new Random();
        int award;
        string message;

        switch (gameType.ToLower())
        {
            case "math":
                award = 20; message = "Smart! +20 points from Math Game"; break;
            case "guess":
                award = 15; message = "Nice Guess! +15 points"; break;
            default:
                double roll = rnd.NextDouble();
                award = roll switch
                {
                    < 0.6 => 5,
                    < 0.85 => 15,
                    < 0.97 => 50,
                    _ => 200
                };
                message = roll switch
                {
                    < 0.6 => "Nice! +5 points",
                    < 0.85 => "Great! +15 points",
                    < 0.97 => "Amazing! +50 points",
                    _ => "JACKPOT! +200 points"
                };
                break;
        }

        stats.Points += award;
        await _context.SaveChangesAsync();
        await CheckAndAwardBadges(stats);

        return Ok(new GamePlayResultDto
        {
            Success = true,
            PointsAwarded = award,
            Message = message,
            NewStats = MapToUserStatsDto(stats)
        });
    }

    // POST: api/gamification/user/{userId}/redeem/{rewardId}
    [HttpPost("user/{userId}/redeem/{rewardId}")]
    [Authorize]
    public async Task<IActionResult> RedeemReward(int userId, int rewardId)
    {
        var stats = await EnsureUserStatsExists(userId);
        var reward = await _context.Rewards.FindAsync(rewardId);
        if (reward == null)
            return NotFound(new { message = "Reward not found" });

        if (stats.Points < reward.CostPoints)
            return BadRequest(new { message = "Insufficient points" });

        stats.Points -= reward.CostPoints;
        _context.UserRewards.Add(new UserReward
        {
            UserStatsId = stats.Id,
            RewardId = reward.RewardId,
            RedeemedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(new RedeemResponseDto
        {
            Success = true,
            Message = $"You redeemed {reward.Title}",
            NewStats = MapToUserStatsDto(stats)
        });
    }

    // ---------------- Helper Methods ----------------
    private async Task<UserStats> EnsureUserStatsExists(int userId)
    {
        var user = await _context.Users.FindAsync(userId) ?? throw new Exception($"User {userId} not found");
        var stats = await _context.UserStats
            .Include(s => s.UserBadges).ThenInclude(ub => ub.Badge)
            .Include(s => s.UserRewards).ThenInclude(ur => ur.Reward)
            .FirstOrDefaultAsync(s => s.UserID == userId);

        if (stats == null)
        {
            stats = new UserStats { UserID = userId, User = user, Points = 0, Streak = 0 };
            _context.UserStats.Add(stats);
            await _context.SaveChangesAsync();
        }

        return stats;
    }

    private async Task CheckAndAwardBadges(UserStats stats)
    {
        var allBadges = await _context.Badges.ToListAsync();
        var existingBadgeIds = stats.UserBadges.Select(ub => ub.BadgeId).ToHashSet();

        var rules = new List<(string code, Func<UserStats, bool> match)>
        {
            ("FIRST_WIN", s => s.Points >= 5),
            ("SCORE_100", s => s.Points >= 100),
            ("STREAK_7", s => s.Streak >= 7)
        };

        foreach (var rule in rules)
        {
            var badge = allBadges.FirstOrDefault(b => b.Code == rule.code);
            if (badge != null && rule.match(stats) && !existingBadgeIds.Contains(badge.BadgeId))
            {
                stats.UserBadges.Add(new UserBadge
                {
                    BadgeId = badge.BadgeId,
                    UserStatsId = stats.Id,
                    AwardedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    private UserStatsDto MapToUserStatsDto(UserStats stats)
    {
        return new UserStatsDto
        {
            UserID = stats.UserID,
            Points = stats.Points,
            Streak = stats.Streak,
            Badges = stats.UserBadges.Select(ub => new BadgeDto
            {
                BadgeId = ub.BadgeId,
                Code = ub.Badge.Code,
                Name = ub.Badge.Name,
                Description = ub.Badge.Description,
                IconUrl = ub.Badge.IconUrl
            }).ToList(),
            RewardsRedeemed = stats.UserRewards.Select(ur => new RewardDto
            {
                RewardId = ur.RewardId,
                Title = ur.Reward.Title,
                CostPoints = ur.Reward.CostPoints,
                RedeemedAt = ur.RedeemedAt
            }).ToList()
        };
    }
}

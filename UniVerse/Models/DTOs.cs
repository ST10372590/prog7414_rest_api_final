using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Universe.Api.Models
{

    public class RegisterRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Student"; // default
    }

    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    namespace Universe.Api.Models
    {
        public class CourseDto
        {
            public string CourseID { get; set; }
            public string CourseTitle { get; set; }
            public string CourseDescription { get; set; }
            public int Credits { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

            // Changed from LecturerName → LecturerID
            public int LecturerID { get; set; }

            public List<ModuleDto> Modules { get; set; }
            public List<AssessmentDto> Assessments { get; set; }
        }
    }

    public class ModuleDto
    {
        public string ModuleID { get; set; }
        public string CourseID { get; set; }
        public string ModuleTitle { get; set; }
        public string ContentType { get; set; }
        public string ContentLink { get; set; }
        public string CompletionStatus { get; set; }
    }

    public class AssessmentDto
    {
        public int AssessmentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }
    }

    public class AssessmentCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }

        [BindRequired]
        public string CourseID { get; set; } // Only include the FK
        public string? ModuleID { get; set; }
    }


    public class NotificationDTO
    {
        public int NotificationID { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
    }

    public class SettingsDto
    {
        // Profile / optional copy of some profile fields
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }

        // Notifications
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool InAppNotifications { get; set; }

        // Privacy
        public bool ProfilePublic { get; set; }
        public bool ShareUsageData { get; set; }

        // Preferences
        public string Theme { get; set; }
        public int ItemsPerPage { get; set; }

        // (Optional) canonical user fields to update together
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class MessageRequest
    {
        public int SenderID { get; set; }
        public int ReceiverID { get; set; }
        public string Content { get; set; }
        public string? FileAttachment { get; set; }
        public string ReadStatus { get; set; } = "Unread";
    }

    // DTO for safe response
    public class MessageResponseDto
    {
        public int MessageID { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public int ReceiverID { get; set; }
        public string ReceiverName { get; set; }
        public string Content { get; set; }
        public string ReadStatus { get; set; }
        public string? FileAttachment { get; set; }
    }

    public class UserStatsDto
    {
        public int UserID { get; set; }
        public int Points { get; set; }
        public int Streak { get; set; }
        public List<BadgeDto> Badges { get; set; } = new();
        public List<RewardDto> RewardsRedeemed { get; set; } = new();
    }

    public class BadgeDto
    {
        public int BadgeId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string IconUrl { get; set; } = "";
    }

    public class LeaderboardEntryDto
    {
        public int UserID { get; set; }
        public string Username { get; set; } = null!;
        public int Points { get; set; }
    }

    public class GamePlayResultDto
    {
        public bool Success { get; set; }
        public int PointsAwarded { get; set; }
        public string Message { get; set; } = "";
        public UserStatsDto NewStats { get; set; }
    }

    public class RedeemResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public UserStatsDto NewStats { get; set; }
    }

    public class GoogleLoginRequestDto
    {
        public string IdToken { get; set; }
    }

    public class RewardDto
    {
        public int RewardId { get; set; }
        public string Title { get; set; } = null!;
        public int CostPoints { get; set; }
        public DateTime RedeemedAt { get; set; }
    }


}

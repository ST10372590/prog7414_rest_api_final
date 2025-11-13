using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universe.Api.Models
{
    // 1. User Profile Data
    public class User
    {
        [Key]
        public int UserID { get; set; } // PK
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePhoto { get; set; }
        public string Role { get; set; } = "Student";
        public string PreferredLanguage { get; set; } = "English";
        public DateTime? LastLogin { get; set; }
        public string? DeviceToken { get; set; }

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Navigation
        public UserSettings Settings { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }

        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
        public ICollection<UserReward> UserRewards { get; set; } = new List<UserReward>();
    }

    // 2. Course Data
    public class Course
    {
        [Key]
        public string CourseID { get; set; } // PK
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
        public int LecturerID { get; set; } // FK → User
        public int Credits { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        


        // Navigation
        public User Lecturer { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Assessment> Assessments { get; set; }
    }

    // 3. Module Data
    public class Module
    {
        [Key]
        public string ModuleID { get; set; } // PK
        public string CourseID { get; set; } // FK → Course
        public string ModuleTitle { get; set; }
        public string ContentType { get; set; }
        public string ContentLink { get; set; }
        public string CompletionStatus { get; set; }

        // Navigation
        public Course Course { get; set; }
    }

    // 4. Notifications Data
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; } // PK
        public int UserID { get; set; } // FK
        public string Type { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }

        // Navigation
        public User User { get; set; }
    }

    // 5. Messaging Data
    public class Message
    {
        [Key]
        public int MessageID { get; set; } // PK
        public int SenderID { get; set; } // FK → User
        public int ReceiverID { get; set; } // FK → User or Group
        public string Content { get; set; }
        public string? FileAttachment { get; set; }
        public string ReadStatus { get; set; }

        // Navigation
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }

    // 6. Calendar & Scheduling Data
    public class CalendarEvent
    {
        [Key]
        public string EventID { get; set; } // PK
        public int UserID { get; set; } // FK
        public string Title { get; set; }
        public string EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ColorCode { get; set; }
        public string Description { get; set; }

        // Navigation
        public User User { get; set; }
    }

    // 7. File Management Data
    public class ClassFile
    {
        [Key]
        public int FileID { get; set; } // PK
        public int UploaderID { get; set; } // FK
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        // Navigation
        public User Uploader { get; set; }
    }

    // 8. Assessments & Grading Data
    public class Assessment
    {
        [Key]
        public int AssessmentID { get; set; } // PK
        public string CourseID { get; set; } // FK → Course
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxMarks { get; set; }
        public string FilePath { get; set; }
        public string ModuleID { get; set; } = string.Empty;

        // Navigation
        public Course Course { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }

    // 9. Submissions Table
    public class Submission
    {
        [Key]
        public int SubmissionID { get; set; } // PK
        public int AssessmentID { get; set; } // FK

        [ForeignKey(nameof(Student))]
        public int UserID { get; set; } // FK
        public string FileLink { get; set; }
        public DateTime SubmittedAt { get; set; }
        public decimal Grade { get; set; }
        public string? Feedback { get; set; }

        // Navigation
        public Assessment Assessment { get; set; }
        public User Student { get; set; }
    }

    // 10. Schedule Data
    public class Schedule
    {
        [Key]
        public int Id { get; set; } // PK
        public string Title { get; set; }
        public string Venue { get; set; }
        public string Lecturer { get; set; }
        public DateTime Time { get; set; }
    }

    // 12. Announcement Data
    public class Announcement
    {
        [Key]
        public int Id { get; set; } // PK
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        // Foreign key to the module
        [Required]
        public string ModuleId { get; set; }

        // Optional: store module title for display purposes
        public string ModuleTitle { get; set; }
    }

    public class Group
    {
        [Key]
        public int GroupID { get; set; }
        public string Name { get; set; }

        public ICollection<GroupMember> Members { get; set; }
        public ICollection<GroupMessage> Messages { get; set; }
    }

    public class GroupMember
    {
        [Key]
        public int Id { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }

        public Group Group { get; set; }
        public User User { get; set; }
    }

    public class GroupMessage
    {
        [Key]
        public int Id { get; set; }
        public int GroupID { get; set; }
        public int SenderID { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public Group Group { get; set; }
        public User Sender { get; set; }
    }

    // 13. Progress Data
    public class Progress
    {
        [Key]
        public int Id { get; set; } // PK
        public string Student { get; set; }
        public string Course { get; set; }
        public string Grade { get; set; }
    }

    public class UserCourse
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Course")]
        public string CourseID { get; set; }
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }

    public class UserSettings
    {
        [Key]
        public int UserID { get; set; } // PK and FK to Users.UserID

        // Profile (stored redundantly here for settings screen convenience, you can keep canonical user fields in User)
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }

        // Notification preferences
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool InAppNotifications { get; set; }

        // Privacy
        public bool ProfilePublic { get; set; }
        public bool ShareUsageData { get; set; }

        // Application preferences
        public string Theme { get; set; } // "light" | "dark" | "system"
        public int ItemsPerPage { get; set; }

        // Navigation
        [ForeignKey("UserID")]
        public User User { get; set; }
    }

    public class UserStats
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }
        public int Points { get; set; } = 0;
        public int Streak { get; set; } = 0; // consecutive days
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;

        public List<UserBadge> UserBadges { get; set; } = new();

        // Rewards redeemed
        public List<UserReward> UserRewards { get; set; } = new();
    }

    public class Badge
    {
        [Key]
        public int BadgeId { get; set; }

        [Required]
        public string Code { get; set; } = null!; // e.g. "FIRST_WIN", "100_POINTS"

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public string IconUrl { get; set; } = ""; // optional

        // Navigation property
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }

    public class UserBadge
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserStats))]
        public int UserStatsId { get; set; }
        public UserStats UserStats { get; set; }

        [ForeignKey(nameof(Badge))]
        public int BadgeId { get; set; }
        public Badge Badge { get; set; }

        public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
    }

    public class Reward
    {
        [Key]
        public int RewardId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public int CostPoints { get; set; } // points required to redeem

        // Navigation
        public ICollection<UserReward> UserRewards { get; set; } = new List<UserReward>();
    }

    public class UserReward
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserStats))]
        public int UserStatsId { get; set; }
        public UserStats UserStats { get; set; }

        [ForeignKey(nameof(Reward))]
        public int RewardId { get; set; }
        public Reward Reward { get; set; }

        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
    }


}

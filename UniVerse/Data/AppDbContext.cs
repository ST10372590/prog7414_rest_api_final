using System;
using Microsoft.EntityFrameworkCore;
using Universe.Api.Models;

namespace Universe.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === Existing DbSets ===
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Progress> ProgressRecords { get; set; }
        public DbSet<ClassFile> Files { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<UserSettings> UserSetting { get; set; }
        public DbSet<UserStats> UserStats { get; set; }

        // === NEW Gamification DbSets ===
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<UserReward> UserRewards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Primary Keys ===
            modelBuilder.Entity<User>().HasKey(u => u.UserID);
            modelBuilder.Entity<Course>().HasKey(c => c.CourseID);
            modelBuilder.Entity<Module>().HasKey(m => m.ModuleID);
            modelBuilder.Entity<Notification>().HasKey(n => n.NotificationID);
            modelBuilder.Entity<Message>().HasKey(m => m.MessageID);
            modelBuilder.Entity<CalendarEvent>().HasKey(c => c.EventID);
            modelBuilder.Entity<ClassFile>().HasKey(f => f.FileID);
            modelBuilder.Entity<Assessment>().HasKey(a => a.AssessmentID);
            modelBuilder.Entity<Submission>().HasKey(s => s.SubmissionID);
            modelBuilder.Entity<Schedule>().HasKey(s => s.Id);
            modelBuilder.Entity<Announcement>().HasKey(a => a.Id);
            modelBuilder.Entity<Progress>().HasKey(p => p.Id);
            modelBuilder.Entity<UserSettings>().HasKey(u => u.UserID);

            // === Relationships (Existing) ===
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Lecturer)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.LecturerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Module>()
                .HasOne(m => m.Course)
                .WithMany(c => c.Modules)
                .HasForeignKey(m => m.CourseID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Assessment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assessments)
                .HasForeignKey(a => a.CourseID);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Assessment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssessmentID);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.UserID);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID);

            modelBuilder.Entity<UserSettings>()
                .HasOne(us => us.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(us => us.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // === NEW Relationships: Gamification ===

            // Each UserBadge links a User ↔ Badge (many-to-many)
            // --- UserStats → User ---
            modelBuilder.Entity<UserStats>()
                .HasOne(us => us.User)
                .WithOne() // If your User class doesn’t have a UserStats property
                .HasForeignKey<UserStats>(us => us.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // --- UserStats → UserBadge (1-to-many) ---
            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.UserStats)
                .WithMany(us => us.UserBadges)
                .HasForeignKey(ub => ub.UserStatsId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Badge → UserBadge (1-to-many) ---
            modelBuilder.Entity<UserBadge>()
                .HasOne(ub => ub.Badge)
                .WithMany(b => b.UserBadges)
                .HasForeignKey(ub => ub.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- UserStats → UserReward (1-to-many) ---
            modelBuilder.Entity<UserReward>()
                .HasOne(ur => ur.UserStats)
                .WithMany(us => us.UserRewards)
                .HasForeignKey(ur => ur.UserStatsId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Reward → UserReward (1-to-many) ---
            modelBuilder.Entity<UserReward>()
                .HasOne(ur => ur.Reward)
                .WithMany(r => r.UserRewards)
                .HasForeignKey(ur => ur.RewardId)
                .OnDelete(DeleteBehavior.Cascade);

            // === Seed Users ===
            modelBuilder.Entity<User>().HasData(
                new User { UserID = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@universe.edu", Username = "alice", PasswordHash = "hashed_pw_1", Role = "Lecturer", PreferredLanguage = "English", PhoneNumber = "0123456789" },
                new User { UserID = 2, FirstName = "Brian", LastName = "Smith", Email = "brian@universe.edu", Username = "brian", PasswordHash = "hashed_pw_2", Role = "Lecturer", PreferredLanguage = "English", PhoneNumber = "0123456790" },
                new User { UserID = 3, FirstName = "Chloe", LastName = "Adams", Email = "chloe@universe.edu", Username = "chloe", PasswordHash = "hashed_pw_3", Role = "Lecturer", PreferredLanguage = "English", PhoneNumber = "0123456791" },
                new User { UserID = 4, FirstName = "David", LastName = "Nguyen", Email = "david@universe.edu", Username = "david", PasswordHash = "hashed_pw_4", Role = "Lecturer", PreferredLanguage = "English", PhoneNumber = "0123456792" }
            );

            // === Seed Courses ===
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseID = "C001", CourseTitle = "Introduction to Programming", CourseDescription = "Learn the fundamentals of C# programming.", LecturerID = 1, Credits = 12, StartDate = DateTime.SpecifyKind(new DateTime(2025, 1, 15), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(new DateTime(2025, 6, 15), DateTimeKind.Utc) },
                new Course { CourseID = "C002", CourseTitle = "Web Development Basics", CourseDescription = "Introduction to HTML, CSS, and JavaScript.", LecturerID = 2, Credits = 10, StartDate = DateTime.SpecifyKind(new DateTime(2025, 2, 1), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(new DateTime(2025, 7, 1), DateTimeKind.Utc) },
                new Course { CourseID = "C003", CourseTitle = "Database Management Systems", CourseDescription = "Learn SQL and relational databases.", LecturerID = 3, Credits = 15, StartDate = DateTime.SpecifyKind(new DateTime(2025, 3, 1), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(new DateTime(2025, 8, 1), DateTimeKind.Utc) },
                new Course { CourseID = "C004", CourseTitle = "Mobile App Development", CourseDescription = "Develop Android apps using Kotlin.", LecturerID = 4, Credits = 14, StartDate = DateTime.SpecifyKind(new DateTime(2025, 4, 1), DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(new DateTime(2025, 9, 1), DateTimeKind.Utc) }
            );

            // === Seed Modules ===
            modelBuilder.Entity<Module>().HasData(
                // Course 1
                new Module { ModuleID = "M001", CourseID = "C001", ModuleTitle = "C# Basics", ContentType = "Video", ContentLink = "https://example.com/csharp-basics", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M002", CourseID = "C001", ModuleTitle = "Data Types and Variables", ContentType = "Article", ContentLink = "https://example.com/data-types", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M003", CourseID = "C001", ModuleTitle = "Control Structures", ContentType = "Quiz", ContentLink = "https://example.com/control-structures", CompletionStatus = "Incomplete" },

                // Course 2
                new Module { ModuleID = "M004", CourseID = "C002", ModuleTitle = "HTML Fundamentals", ContentType = "Video", ContentLink = "https://example.com/html", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M005", CourseID = "C002", ModuleTitle = "CSS Styling", ContentType = "Article", ContentLink = "https://example.com/css", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M006", CourseID = "C002", ModuleTitle = "JavaScript Basics", ContentType = "Video", ContentLink = "https://example.com/js", CompletionStatus = "Incomplete" },

                // Course 3
                new Module { ModuleID = "M007", CourseID = "C003", ModuleTitle = "Introduction to Databases", ContentType = "Video", ContentLink = "https://example.com/db-intro", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M008", CourseID = "C003", ModuleTitle = "SQL Queries", ContentType = "Article", ContentLink = "https://example.com/sql-queries", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M009", CourseID = "C003", ModuleTitle = "Database Design", ContentType = "Quiz", ContentLink = "https://example.com/db-design", CompletionStatus = "Incomplete" },

                // Course 4
                new Module { ModuleID = "M010", CourseID = "C004", ModuleTitle = "Kotlin Basics", ContentType = "Video", ContentLink = "https://example.com/kotlin-basics", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M011", CourseID = "C004", ModuleTitle = "UI Design with XML", ContentType = "Article", ContentLink = "https://example.com/ui-design", CompletionStatus = "Incomplete" },
                new Module { ModuleID = "M012", CourseID = "C004", ModuleTitle = "Integrating Firebase", ContentType = "Video", ContentLink = "https://example.com/firebase", CompletionStatus = "Incomplete" }
            );

        }
    }

    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}

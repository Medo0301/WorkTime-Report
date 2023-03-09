using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace WorkTimeReport.Models
{
    public class AppDbContext :IdentityDbContext<EmployeeUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<GoingOut> GoingOuts { get; set; }
        public DbSet<WorkingTime> WorkingTimes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GoingOut>().HasKey(s => s.Id);
            builder.Entity<GoingOut>()
                    .HasOne(e => e.User)
                    .WithMany(e => e.GoingOuts)
                    .HasForeignKey(e => e.EmpUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<GoingOut>(entity =>
            {
                entity.Property(e => e.DateOfAbsence)
                    .HasColumnType("datetime2");

                entity.Property(e => e.StartAbsence)
                    .HasColumnType("datetime2");

                entity.Property(e => e.EndAbsence)
                    .HasColumnType("datetime2");

                entity.Property(e => e.HoursOfAbsence)
                    .HasConversion(
                        timeSpan => timeSpan.ToString(),
                        timeSpanString => TimeSpan.Parse(timeSpanString)
                    );

                entity.Property(e => e.Reason)
                    .HasMaxLength(1000);
            });

            builder.Entity<WorkingTime>().HasKey(s => s.Id);
            builder.Entity<WorkingTime>()
                    .HasOne(e => e.User)
                    .WithMany(e => e.WorkingTimes)
                    .HasForeignKey(e => e.EmpUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<WorkingTime>(entity =>
            {
                entity.Property(e => e.Date)
                    .HasColumnType("datetime2");

                entity.Property(e => e.StartWork)
                    .HasColumnType("datetime2");

                entity.Property(e => e.EndWork)
                    .HasColumnType("datetime2");

                entity.Property(e => e.HoursForDay)
                    .HasConversion(
                        timeSpan => timeSpan.ToString(),
                        timeSpanString => TimeSpan.Parse(timeSpanString)
                    );

                entity.Property(e => e.TypeOfDay)
                    .HasDefaultValue(TypeOfDay.WorkingDay);
            });
        }
    }
}

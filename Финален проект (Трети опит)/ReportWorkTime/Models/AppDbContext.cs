using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ReportWorkTime.Models
{
    public class AppDbContext : IdentityDbContext<EmployeeUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<GoingOut> GoingOut { get; set; }
        public DbSet<WorkingDay> WorkingDays { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GoingOut>().HasKey(s => s.Id);
            modelBuilder.Entity<GoingOut>()
                    .HasOne(e => e.User)
                    .WithMany(e => e.GoingOuts)
                    .HasForeignKey(e => e.EmpUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GoingOut>(entity =>
            {
                entity.Property(e => e.DateOfAbsence)
                    .HasColumnType("datetime2");

                entity.Property(e => e.StartAbsence)
                    .HasColumnType("datetime2");

                entity.Property(e => e.EndAbsence)
                    .HasColumnType("datetime2");

                entity.Property(e => e.Reason)
                    .HasMaxLength(1000);

                entity.Property(e => e.HoursOfAbsence)
                    .HasConversion(
                        timeSpan => timeSpan.ToString(),
                        timeSpan => TimeSpan.Parse(timeSpan)
                    );
                entity.Property(e => e.TypeOfDay)
                    .HasDefaultValue(TypeOfDay.WorkingDay);

                entity.Property(e => e.NumberOfDays)
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<WorkingDay>().HasKey(s => s.Id);
            modelBuilder.Entity<WorkingDay>()
                    .HasOne(e => e.User)
                    .WithMany(e => e.WorkingDays)
                    .HasForeignKey(e => e.EmpUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WorkingDay>(entity =>
            {
                entity.Property(e => e.Date)
                    .HasColumnType("datetime2");

                entity.Property(e => e.StartWork)
                    .HasColumnType("datetime2");

                entity.Property(e => e.EndWork)
                    .HasColumnType("datetime2");

                entity.Property(e => e.TypeOfDay)
                    .HasDefaultValue(TypeOfDay.WorkingDay);

                entity.Property(e => e.HoursForDay)
                    .HasConversion(
                        timeSpan => timeSpan.ToString(),
                        timeSpan => TimeSpan.Parse(timeSpan)
                    );
            });
        }
    }
}

using DAL_Task.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL_Task
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<AppUser> appUsers { get; set; }

        public DbSet<TaskModel> Tasks { get; set; }


        public DbSet<Team> Teams { get; set; }

        public DbSet<TeamMember> TeamMembers { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{


        //    //modelBuilder.Entity<TeamMember>()
        //    //    .HasKey(tm => new { tm.TeamId, tm.MemberId });

        //    //modelBuilder.Entity<TeamMember>()
        //    //    .HasOne(tm => tm.Team)
        //    //    .WithMany(t => t.Members)
        //    //    .HasForeignKey(tm => tm.TeamId);

        //    //modelBuilder.Entity<TeamMember>()
        //    //    .HasOne(tm => tm.Member)
        //    //    .WithMany()
        //    //    .HasForeignKey(tm => tm.MemberId);

        //    //modelBuilder.Entity<IdentityUserLogin<string>>().HasNoKey();
        //    //modelBuilder.Entity<IdentityUserRole<string>>().HasNoKey();
        //    //modelBuilder.Entity<IdentityUserToken<string>>().HasNoKey();


        //    // Other configurations as needed
        //}


    }
}

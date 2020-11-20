using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace UGPC_IIUI.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<Change> Changes { get; set; }

        public DbSet<Presentation> Presentations { get; set; }
        public DbSet<Marking> Markings { get; set; }

        public DbSet<Batch> Batches { get; set; }
        public DbSet<Section> Sections { get; set; }


        public DbSet<CommitteeMember> CommitteeMembers { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
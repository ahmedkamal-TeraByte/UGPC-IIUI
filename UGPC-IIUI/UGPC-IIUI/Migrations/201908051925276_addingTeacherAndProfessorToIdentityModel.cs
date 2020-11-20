namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingTeacherAndProfessorToIdentityModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "StudentId", c => c.Int());
            AddColumn("dbo.AspNetUsers", "ProfessorId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "StudentId");
            CreateIndex("dbo.AspNetUsers", "ProfessorId");
            AddForeignKey("dbo.AspNetUsers", "ProfessorId", "dbo.Professors", "ProfessorId");
            AddForeignKey("dbo.AspNetUsers", "StudentId", "dbo.Students", "StudentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "StudentId", "dbo.Students");
            DropForeignKey("dbo.AspNetUsers", "ProfessorId", "dbo.Professors");
            DropIndex("dbo.AspNetUsers", new[] { "ProfessorId" });
            DropIndex("dbo.AspNetUsers", new[] { "StudentId" });
            DropColumn("dbo.AspNetUsers", "ProfessorId");
            DropColumn("dbo.AspNetUsers", "StudentId");
        }
    }
}

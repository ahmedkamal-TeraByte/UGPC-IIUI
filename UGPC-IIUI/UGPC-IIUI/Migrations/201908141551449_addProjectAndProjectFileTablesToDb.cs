namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProjectAndProjectFileTablesToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectFiles",
                c => new
                    {
                        ProjectFileId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        FilePath = c.String(nullable: false),
                        FileType = c.String(),
                    })
                .PrimaryKey(t => t.ProjectFileId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        SubmissionDate = c.DateTime(nullable: false),
                        Status = c.String(),
                        ProjectType = c.String(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectId)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectFiles", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "GroupId", "dbo.Groups");
            DropIndex("dbo.Projects", new[] { "GroupId" });
            DropIndex("dbo.ProjectFiles", new[] { "ProjectId" });
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectFiles");
        }
    }
}

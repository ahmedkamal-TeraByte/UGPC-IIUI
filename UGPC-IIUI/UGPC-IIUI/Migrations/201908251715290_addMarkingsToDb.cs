namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMarkingsToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Markings",
                c => new
                    {
                        MarkingId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        PresentationMarks = c.Int(nullable: false),
                        SupervisorMarks = c.Int(nullable: false),
                        InternalMarks = c.Int(nullable: false),
                        ExternalMarks = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MarkingId)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Markings", "ProjectId", "dbo.Projects");
            DropIndex("dbo.Markings", new[] { "ProjectId" });
            DropTable("dbo.Markings");
        }
    }
}

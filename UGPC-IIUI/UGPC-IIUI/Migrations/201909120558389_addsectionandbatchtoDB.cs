namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsectionandbatchtoDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Batches",
                c => new
                    {
                        BatchId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.BatchId);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        SectionId = c.Int(nullable: false, identity: true),
                        BatchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SectionId)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sections", "BatchId", "dbo.Batches");
            DropIndex("dbo.Sections", new[] { "BatchId" });
            DropTable("dbo.Sections");
            DropTable("dbo.Batches");
        }
    }
}

namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableCommitteeMember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommitteeMembers",
                c => new
                    {
                        CommitteeId = c.Int(nullable: false),
                        ProfessorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CommitteeId, t.ProfessorId })
                .ForeignKey("dbo.Committees", t => t.CommitteeId, cascadeDelete: true)
                .ForeignKey("dbo.Professors", t => t.ProfessorId, cascadeDelete: true)
                .Index(t => t.CommitteeId)
                .Index(t => t.ProfessorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommitteeMembers", "ProfessorId", "dbo.Professors");
            DropForeignKey("dbo.CommitteeMembers", "CommitteeId", "dbo.Committees");
            DropIndex("dbo.CommitteeMembers", new[] { "ProfessorId" });
            DropIndex("dbo.CommitteeMembers", new[] { "CommitteeId" });
            DropTable("dbo.CommitteeMembers");
        }
    }
}

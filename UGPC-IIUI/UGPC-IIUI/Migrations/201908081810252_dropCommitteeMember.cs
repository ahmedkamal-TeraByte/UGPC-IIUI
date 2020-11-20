namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropCommitteeMember : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CommitteeMembers", "CommitteeId", "dbo.Committees");
            DropForeignKey("dbo.CommitteeMembers", "Professor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.CommitteeMembers", new[] { "CommitteeId" });
            DropIndex("dbo.CommitteeMembers", new[] { "Professor_Id" });
            DropTable("dbo.CommitteeMembers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CommitteeMembers",
                c => new
                    {
                        CommitteeId = c.Int(nullable: false),
                        ProfessorId = c.Int(nullable: false),
                        Professor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CommitteeId, t.ProfessorId });
            
            CreateIndex("dbo.CommitteeMembers", "Professor_Id");
            CreateIndex("dbo.CommitteeMembers", "CommitteeId");
            AddForeignKey("dbo.CommitteeMembers", "Professor_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.CommitteeMembers", "CommitteeId", "dbo.Committees", "CommitteeId", cascadeDelete: true);
        }
    }
}

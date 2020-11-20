namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeProfessorToUserInCommitteeMember : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CommitteeMembers", "ProfessorId", "dbo.Professors");
            DropIndex("dbo.CommitteeMembers", new[] { "ProfessorId" });
            DropPrimaryKey("dbo.CommitteeMembers");
            AddColumn("dbo.CommitteeMembers", "UserId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.CommitteeMembers", new[] { "CommitteeId", "UserId" });
            CreateIndex("dbo.CommitteeMembers", "UserId");
            AddForeignKey("dbo.CommitteeMembers", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            DropColumn("dbo.CommitteeMembers", "ProfessorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CommitteeMembers", "ProfessorId", c => c.Int(nullable: false));
            DropForeignKey("dbo.CommitteeMembers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.CommitteeMembers", new[] { "UserId" });
            DropPrimaryKey("dbo.CommitteeMembers");
            DropColumn("dbo.CommitteeMembers", "UserId");
            AddPrimaryKey("dbo.CommitteeMembers", new[] { "CommitteeId", "ProfessorId" });
            CreateIndex("dbo.CommitteeMembers", "ProfessorId");
            AddForeignKey("dbo.CommitteeMembers", "ProfessorId", "dbo.Professors", "ProfessorId", cascadeDelete: true);
        }
    }
}

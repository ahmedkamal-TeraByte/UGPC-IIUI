namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changesInCommitteeModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CommitteeMembers", "ProfessorId", "dbo.Professors");
            DropIndex("dbo.CommitteeMembers", new[] { "ProfessorId" });
            AddColumn("dbo.CommitteeMembers", "Professor_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.CommitteeMembers", "Professor_Id");
            AddForeignKey("dbo.CommitteeMembers", "Professor_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommitteeMembers", "Professor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.CommitteeMembers", new[] { "Professor_Id" });
            DropColumn("dbo.CommitteeMembers", "Professor_Id");
            CreateIndex("dbo.CommitteeMembers", "ProfessorId");
            AddForeignKey("dbo.CommitteeMembers", "ProfessorId", "dbo.Professors", "ProfessorId", cascadeDelete: true);
        }
    }
}

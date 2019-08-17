namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSupervisorToProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "SupervisorId", c => c.Int(nullable: false));
            AddColumn("dbo.Projects", "Supervisor_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Projects", "Supervisor_Id");
            AddForeignKey("dbo.Projects", "Supervisor_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "Supervisor_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "Supervisor_Id" });
            DropColumn("dbo.Projects", "Supervisor_Id");
            DropColumn("dbo.Projects", "SupervisorId");
        }
    }
}

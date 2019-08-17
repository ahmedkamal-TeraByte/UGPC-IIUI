namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSupervisorIdInProject : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Projects", new[] { "Supervisor_Id" });
            DropColumn("dbo.Projects", "SupervisorId");
            RenameColumn(table: "dbo.Projects", name: "Supervisor_Id", newName: "SupervisorId");
            AlterColumn("dbo.Projects", "SupervisorId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Projects", "SupervisorId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Projects", new[] { "SupervisorId" });
            AlterColumn("dbo.Projects", "SupervisorId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Projects", name: "SupervisorId", newName: "Supervisor_Id");
            AddColumn("dbo.Projects", "SupervisorId", c => c.Int(nullable: false));
            CreateIndex("dbo.Projects", "Supervisor_Id");
        }
    }
}

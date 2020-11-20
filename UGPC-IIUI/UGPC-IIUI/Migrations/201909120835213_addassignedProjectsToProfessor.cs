namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addassignedProjectsToProfessor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Professors", "AssignedProjects", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Professors", "AssignedProjects");
        }
    }
}

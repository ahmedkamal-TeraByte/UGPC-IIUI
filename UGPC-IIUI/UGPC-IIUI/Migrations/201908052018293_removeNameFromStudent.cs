namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeNameFromStudent : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Students", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "Name", c => c.String(nullable: false));
        }
    }
}

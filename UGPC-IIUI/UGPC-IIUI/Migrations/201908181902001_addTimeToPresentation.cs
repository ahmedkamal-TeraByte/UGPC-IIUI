namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimeToPresentation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Presentations", "Time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Presentations", "Time");
        }
    }
}

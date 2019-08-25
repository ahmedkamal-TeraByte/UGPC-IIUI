namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changesinpresentaiton : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Presentations", "Date", c => c.DateTime());
            AlterColumn("dbo.Presentations", "Time", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Presentations", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Presentations", "Date", c => c.DateTime(nullable: false));
        }
    }
}

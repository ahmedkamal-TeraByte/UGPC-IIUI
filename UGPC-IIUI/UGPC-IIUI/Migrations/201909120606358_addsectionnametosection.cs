namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsectionnametosection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sections", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sections", "Name");
        }
    }
}

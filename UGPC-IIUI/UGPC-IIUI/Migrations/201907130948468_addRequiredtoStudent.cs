namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRequiredtoStudent : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Students", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Students", "Batch", c => c.String(nullable: false));
            AlterColumn("dbo.Students", "Section", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Students", "Section", c => c.String());
            AlterColumn("dbo.Students", "Batch", c => c.String());
            AlterColumn("dbo.Students", "Name", c => c.String());
        }
    }
}

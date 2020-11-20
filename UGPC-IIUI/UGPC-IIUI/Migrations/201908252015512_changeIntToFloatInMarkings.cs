namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeIntToFloatInMarkings : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Markings", "PresentationMarks", c => c.Single(nullable: false));
            AlterColumn("dbo.Markings", "SupervisorMarks", c => c.Single(nullable: false));
            AlterColumn("dbo.Markings", "InternalMarks", c => c.Single(nullable: false));
            AlterColumn("dbo.Markings", "ExternalMarks", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Markings", "ExternalMarks", c => c.Int(nullable: false));
            AlterColumn("dbo.Markings", "InternalMarks", c => c.Int(nullable: false));
            AlterColumn("dbo.Markings", "SupervisorMarks", c => c.Int(nullable: false));
            AlterColumn("dbo.Markings", "PresentationMarks", c => c.Int(nullable: false));
        }
    }
}

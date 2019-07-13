namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatestudentpasswordtable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Students", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Students", "Password", c => c.String());
        }
    }
}

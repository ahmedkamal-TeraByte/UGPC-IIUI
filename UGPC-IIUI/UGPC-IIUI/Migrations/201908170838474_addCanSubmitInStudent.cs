namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCanSubmitInStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "CanSubmitProposal", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "CanSubmitProposal");
        }
    }
}

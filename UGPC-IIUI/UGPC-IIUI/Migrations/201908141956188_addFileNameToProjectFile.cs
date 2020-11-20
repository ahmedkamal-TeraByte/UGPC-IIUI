namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFileNameToProjectFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectFiles", "FileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectFiles", "FileName");
        }
    }
}

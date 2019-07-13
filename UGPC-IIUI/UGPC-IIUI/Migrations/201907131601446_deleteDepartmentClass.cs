namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteDepartmentClass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.Students", new[] { "DepartmentID" });
            AddColumn("dbo.Students", "Department", c => c.Int(nullable: false));
            DropColumn("dbo.Students", "DepartmentID");
            DropTable("dbo.Departments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            AddColumn("dbo.Students", "DepartmentID", c => c.Int(nullable: false));
            DropColumn("dbo.Students", "Department");
            CreateIndex("dbo.Students", "DepartmentID");
            AddForeignKey("dbo.Students", "DepartmentID", "dbo.Departments", "DepartmentID", cascadeDelete: true);
        }
    }
}

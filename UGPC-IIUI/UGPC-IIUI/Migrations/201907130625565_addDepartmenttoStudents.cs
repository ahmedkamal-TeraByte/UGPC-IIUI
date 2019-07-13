namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDepartmenttoStudents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "DepartmentID", c => c.Int(nullable: false));
            CreateIndex("dbo.Students", "DepartmentID");
            AddForeignKey("dbo.Students", "DepartmentID", "dbo.Departments", "DepartmentID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "DepartmentID", "dbo.Departments");
            DropIndex("dbo.Students", new[] { "DepartmentID" });
            DropColumn("dbo.Students", "DepartmentID");
        }
    }
}

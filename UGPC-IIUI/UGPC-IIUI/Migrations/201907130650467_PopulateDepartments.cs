namespace UGPC_IIUI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateDepartments : DbMigration
    {
        public override void Up()
        {
            Sql("insert into Departments ( Name) values ('SE')");
            Sql("insert into Departments ( Name) values ('CS')");
            Sql("insert into Departments ( Name) values ('IT')");
        }
        
        public override void Down()
        {
        }
    }
}

namespace SmartStore.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Web.Hosting;
    using SmartStore.Core.Data;
    using SmartStore.Core.Domain.Customers;
    using SmartStore.Core.Domain.Security;
    using SmartStore.Data.Setup;
    using SmartStore.Data.Utilities;

    public partial class Menus : DbMigration, IDataSeeder<SmartObjectContext>
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MenuId = c.Int(nullable: false),
                        ParentItemId = c.Int(nullable: false),
                        SystemName = c.String(maxLength: 400),
                        Model = c.String(),
                        Title = c.String(maxLength: 400),
                        ShortDescription = c.String(maxLength: 400),
                        Published = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsDivider = c.Boolean(nullable: false),
                        ShowExpanded = c.Boolean(nullable: false),
                        NoFollow = c.Boolean(nullable: false),
                        NewWindow = c.Boolean(nullable: false),
                        HtmlId = c.String(maxLength: 100),
                        CssClass = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Menu", t => t.MenuId, cascadeDelete: true)
                .Index(t => t.MenuId)
                .Index(t => t.ParentItemId, name: "IX_MenuItem_ParentItemId")
                .Index(t => t.Published, name: "IX_MenuItem_Published")
                .Index(t => t.DisplayOrder, name: "IX_MenuItem_DisplayOrder");
            
            CreateTable(
                "dbo.Menu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SystemName = c.String(nullable: false, maxLength: 400),
                        IsSystemMenu = c.Boolean(nullable: false),
                        Title = c.String(maxLength: 400),
                        Published = c.Boolean(nullable: false),
                        LimitedToStores = c.Boolean(nullable: false),
                        SubjectToAcl = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.SystemName, t.IsSystemMenu }, name: "IX_Menu_SystemName_IsSystemMenu")
                .Index(t => t.Published, name: "IX_Menu_Published")
                .Index(t => t.LimitedToStores, name: "IX_Menu_LimitedToStores")
                .Index(t => t.SubjectToAcl, name: "IX_Menu_SubjectToAcl");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MenuItem", "MenuId", "dbo.Menu");
            DropIndex("dbo.Menu", "IX_Menu_SubjectToAcl");
            DropIndex("dbo.Menu", "IX_Menu_LimitedToStores");
            DropIndex("dbo.Menu", "IX_Menu_Published");
            DropIndex("dbo.Menu", "IX_Menu_SystemName_IsSystemMenu");
            DropIndex("dbo.MenuItem", "IX_MenuItem_DisplayOrder");
            DropIndex("dbo.MenuItem", "IX_MenuItem_Published");
            DropIndex("dbo.MenuItem", "IX_MenuItem_ParentItemId");
            DropIndex("dbo.MenuItem", new[] { "MenuId" });
            DropTable("dbo.Menu");
            DropTable("dbo.MenuItem");
        }

        public bool RollbackOnFailure => true;

        public void Seed(SmartObjectContext context)
        {
            var permissionMigrator = new PermissionMigrator(context);

            permissionMigrator.AddPermission(new PermissionRecord
            {
                Name = "Admin area. Manage Menus",
                SystemName = "ManageMenus",
                Category = "Content Management"
            }, new string[] { SystemCustomerRoleNames.Administrators });


            if (HostingEnvironment.IsHosted && DataSettings.DatabaseIsInstalled())
            {
                DataMigrator.CreateSystemMenus(context);
            }
        }
    }
}

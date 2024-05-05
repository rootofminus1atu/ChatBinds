namespace chatbinds.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameChatKeys",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        ChatKey = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.ThingToSays",
                c => new
                    {
                        HotKey = c.String(nullable: false, maxLength: 128),
                        Text = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HotKey);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ThingToSays");
            DropTable("dbo.GameChatKeys");
        }
    }
}

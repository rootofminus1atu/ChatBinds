using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbinds
{
    public class ThingToSay
    {
        [Key]
        public string HotKey { get; set; }
        public string Text { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public class GameChatKey
    {
        [Key]
        public string Name { get; set; }
        public string ChatKey { get; set; }
    }

    public class Db : DbContext
    {
        public DbSet<ThingToSay> ThingsToSay { get; set; }
        public DbSet<GameChatKey> GameChatKeys { get; set; }

        public Db() : base() { }
        // connect with `Db db = new Db();` in main
    }
}

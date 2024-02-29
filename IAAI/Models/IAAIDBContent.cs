using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace IAAI.Models
{
    public partial class IAAIDBContent : DbContext
    {
        public IAAIDBContent()
            : base("name=IAAIDBContent")
        {
        }

        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<About> Abouts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<CertifiedMember> CertifiedMembers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Personal.Webapi.Entity
{
    public partial class PersonalContext : DbContext
    {
        public PersonalContext()
        {
        }

        public PersonalContext(DbContextOptions<PersonalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CustomRecord> CustomRecord { get; set; }
        public virtual DbSet<CustomType> CustomType { get; set; }
        public virtual DbSet<Income> Income { get; set; }
        public virtual DbSet<OptionType> OptionType { get; set; }
        public virtual DbSet<PayType> PayType { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Wages> Wages { get; set; }
        public virtual DbSet<TokenUser> TokenUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomRecord>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.CustomDate).HasColumnType("date");

                entity.Property(e => e.CustomRecordId).HasColumnName("CustomRecordID").ValueGeneratedOnAdd();

                entity.Property(e => e.CustomTypeId).HasColumnName("CustomTypeID");

                entity.Property(e => e.Describe).HasMaxLength(50);

                entity.Property(e => e.PayTypeId).HasColumnName("PayTypeID");

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e=>e.Amount).HasColumnType("decimal(18, 2)");

            });

            modelBuilder.Entity<CustomType>(entity =>
            {
                entity.Property(e => e.CustomTypeId)
                    .HasColumnName("CustomTypeID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Income>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.IncomeId).HasColumnName("IncomeID").ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OperateId).HasColumnName("OperateID");

                entity.Property(e => e.PayTypeId).HasColumnName("PayTypeID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<OptionType>(entity =>
            {

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.OptionTypeId).HasColumnName("OptionTypeID").ValueGeneratedOnAdd();

                entity.Property(e => e.ParentId)
                    .HasColumnName("ParentID")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PayType>(entity =>
            {
                entity.Property(e => e.PayTypeId)
                    .HasColumnName("PayTypeID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Users>(entity =>
            {

                entity.Property(e => e.LoginName).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Wages>(entity =>
            {

                entity.Property(e => e.AccumulationFund).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BaseWages).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Bonus).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.LeaveAmont).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LeaveDays).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PreTaxWages).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ReceiveWages).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SocialSecurity).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Subsidy).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Tax).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TaxBase).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.WagesDate).HasColumnType("date");

                entity.Property(e => e.WagesId).HasColumnName("WagesID").ValueGeneratedOnAdd();

                entity.Property(e => e.WorkDays).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<TokenUser>(entity =>
            {
                entity.Property(e => e.TokenUserId)
                   .HasColumnName("TokenUserId")
                   .ValueGeneratedOnAdd();

                entity.Property(e => e.ExpriedTime).HasColumnType("datetime");

                entity.Property(e => e.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecurityQuestionAuthAPI.Data;

namespace SecurityQuestionAuthAPI
{
    public class SecurityAuthContext : DbContext
    {
        public DbSet<User>Users{get; set;}
        public DbSet<SecurityQuestion>securityQuestions{get;set;}

        public DbSet<Phrase>phrases{get;set;}

        public DbSet<OTP>Otp{get;set;}
        public SecurityAuthContext(DbContextOptions<SecurityAuthContext>options):base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(Users));
            modelBuilder.Entity<SecurityQuestion>().ToTable(nameof(securityQuestions));

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();

            modelBuilder.Entity<SecurityQuestion>().HasKey(s => s.Id);
            modelBuilder.Entity<SecurityQuestion>().Property(s => s.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<SecurityQuestion>().Property(s => s.QuestionText).IsRequired();
            modelBuilder.Entity<SecurityQuestion>().Property(s => s.Answer).IsRequired();
            modelBuilder.Entity<SecurityQuestion>().HasIndex(s => s.QuestionText).IsUnique();
            modelBuilder.Entity<SecurityQuestion>().HasIndex(s => s.Answer).IsUnique();

            modelBuilder.Entity<Phrase>().HasIndex(p => p.UserEmail).IsUnique();

            modelBuilder.Entity<OTP>().HasKey(o => o.Id);
            modelBuilder.Entity<OTP>().Property(o => o.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OTP>().Property(p => p.CreatedDate)
                                            .HasColumnType("TIMESTAMP")
                                            .ValueGeneratedOnAdd()
                                            .HasDefaultValueSql("CURRENT_TIMESTAMP()");





            //relationship

            modelBuilder.Entity<User>().HasMany(u => u.securityQuestions)
                                        .WithOne(u => u.User)
                                        .HasForeignKey(u => u.UserId)
                                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(u => u.phrase)
                                        .WithOne(u => u.user)
                                        .HasForeignKey(u => u.UserId)
                                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>().HasMany(u => u.otp)
                                        .WithOne(u => u.user)
                                        .HasForeignKey(x => x.UserId)
                                        .OnDelete(DeleteBehavior.Cascade);





        }

    }
}
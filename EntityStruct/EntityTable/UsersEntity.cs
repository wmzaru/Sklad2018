using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStruct.EntityTable
{
    [Table("Users")]
    public class UsersEntity
    {
        public UsersEntity() { }

        [Key]
        public string Username { get; set; }
        public string Password { get; set; }

        public UsersEntity(string user, string password)
        {
            Username = user;
            Password = password;
        }
    }
}

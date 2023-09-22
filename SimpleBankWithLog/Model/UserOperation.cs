using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using SQLite.CodeFirst;
using System.Threading.Tasks;

namespace SimpleBank.Model
{
    [Table("UserOperations")]
    public class UserOperation
    {
        public UserOperation()
        {
        }

        [Key, Autoincrement]
        [Unique]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("OperationId")]
        public int OperationId { get; set; }

        [Column("Role")]
        public string Role { get; set; }

        [Column("DataOperation")]
        public string DataOperation { get; set; }

        [Column("Operation")]
        public string Operation { get; set; }

        [Column("Sum")]
        public int Sum { get; set; }
    }
}

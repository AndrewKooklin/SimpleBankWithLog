﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using SQLite.CodeFirst;
using System.Threading.Tasks;
using SimpleBank.ViewModel;

namespace SimpleBank.Model
{
    [Table("UserOperations")]
    public class UserOperation : ViewModelBase
    {
        private int operationId;

        private string role;

        private string dataOperation;

        private string operation;

        private int sum;

        public UserOperation()
        {
        }

        public UserOperation(int operationId, string role, string dataOperation, 
                             string operation, int sum)
        {
            this.operationId = operationId;
            this.role = role;
            this.dataOperation = dataOperation;
            this.operation = operation;
            this.sum = sum;
        }

        [Key, Autoincrement]
        [Unique]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("OperationId")]
        public int OperationId
        {
            get { return operationId; }
            set
            {
                operationId = value;
                OnPropertyChanged(nameof(OperationId));
            }
        }

        [Column("Role")]
        public string Role
        {
            get { return role; }
            set
            {
                role = value;
                OnPropertyChanged(nameof(Role));
            }
        }

        [Column("DataOperation")]
        public string DataOperation
        {
            get { return dataOperation; }
            set
            {
                dataOperation = value;
                OnPropertyChanged(nameof(DataOperation));
            }
        }

        [Column("Operation")]
        public string Operation
        {
            get { return operation; }
            set
            {
                operation = value;
                OnPropertyChanged(nameof(Operation));
            }
        }

        [Column("Sum")]
        public int Sum
        {
            get { return sum; }
            set
            {
                sum = value;
                OnPropertyChanged(nameof(Sum));
            }
        }
    }
}
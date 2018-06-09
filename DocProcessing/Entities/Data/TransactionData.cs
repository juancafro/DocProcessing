using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Data
{
    class TransactionData : BaseEntity
    {
        public string AssociateAcount { get; set; }
        public string Description { get; set; }
        public int Group { get; set; }
        public bool isTotal { get; set; }

    }
    public enum Groups {

        group1 = 1,
        group2 = 2,
        group3 = 3,
        group4 = 4,
        group5 = 5,
        group6 = 6,
        group7 = 7,
        group8 = 8

    }
}

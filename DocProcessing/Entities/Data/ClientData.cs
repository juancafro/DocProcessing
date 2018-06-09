using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocProcessing.Entities.Data
{
    class ClientData : BaseEntity
    {
        public string AccountNumber { get; set; }
        public string Email { get; set; }
        public List<TransactionData> Transactions = new List<TransactionData>();
        public List<SavingsData> savings = new List<SavingsData>();


        public override string ToString()
        {
            Dictionary<string, object> Information = this.info;
            List<Dictionary<string,object>> t = new List<Dictionary<string,object>> ();
            foreach (TransactionData td in Transactions) {
                t.Add(td.info);
            }
            Information.Add("transactions", t);
            return JsonConvert.SerializeObject(Information);
        }

    }
}

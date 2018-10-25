using GDPR.Common.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public class RecordCollection
    {
        private List<Record> records = new List<Record>();

        private Hashtable keys = new Hashtable();

        public void AddRecord(Record r)
        {
            if (!keys.ContainsKey(r.Type + r.RecordId))
            {
                keys.Add(r.Type + r.RecordId, r.Type + r.RecordId);
                records.Add(r);
            }
        }

        public List<Record> Records
        {
            get
            {
                return this.records;
            }
        }
    }
}

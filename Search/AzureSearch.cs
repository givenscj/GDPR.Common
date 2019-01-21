using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;

namespace GDPR.Common.Search
{
    public class AzureSearch : SearchService
    {
        static public void  SearchApplication()
        {

        }

        static public void SearchTenant()
        {

        }

        static public void PushData(DateTime indexDate)
        {
            //search for updated or new items...
            SearchCredentials creds = new Microsoft.Azure.Search.SearchCredentials(Configuration.SearchKey);
            SearchIndexClient svcClient = new Microsoft.Azure.Search.SearchIndexClient(creds);
            svcClient.IndexName = "applications";

            Document doc = new Document();
            doc.Add("Type", "Application");

            List<IndexAction> actions = new List<IndexAction>();
            IndexAction a = IndexAction.Upload(doc);
            actions.Add(a);

            IndexBatch batch = new IndexBatch(actions);
            svcClient.Documents.Index(batch);
        }

        static public Document CreateDocument(object obj)
        {
            return new Document();
        }
        
    }
}

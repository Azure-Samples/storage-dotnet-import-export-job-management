// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace StorageImportExport.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class ListJobsResponse
    {
        /// <summary>
        /// Initializes a new instance of the ListJobsResponse class.
        /// </summary>
        public ListJobsResponse() { }

        /// <summary>
        /// Initializes a new instance of the ListJobsResponse class.
        /// </summary>
        public ListJobsResponse(string odatametadata = default(string), string odatacount = default(string), string nextLink = default(string), IList<ListJobsResponseValueItem> value = default(IList<ListJobsResponseValueItem>))
        {
            Odatametadata = odatametadata;
            Odatacount = odatacount;
            NextLink = nextLink;
            Value = value;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "odata.metadata")]
        public string Odatametadata { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "odata.count")]
        public string Odatacount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "next-link")]
        public string NextLink { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public IList<ListJobsResponseValueItem> Value { get; set; }

    }
}

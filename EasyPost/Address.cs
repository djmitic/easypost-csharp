﻿using RestSharp;

using System;
using System.Collections.Generic;

namespace EasyPost {
    public class Address : Resource {
        public string id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public bool? residential { get; set; }
        public string federal_tax_id { get; set; }
        public string state_tax_id { get; set; }
        public string carrier_facility { get; set; }
        public List<string> verify { get; set; }
        public List<string> verify_strict { get; set; }
        public string mode { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public Verifications verifications { get; set; }

        /// <summary>
        /// Retrieve an Address from its id.
        /// </summary>
        /// <param name="id">String representing an Address. Starts with "adr_".</param>
        /// <returns>EasyPost.Address instance.</returns>
        public static Address Retrieve(string id) {
            Request request = new Request("v2/addresses/{id}");
            request.AddUrlSegment("id", id);

            return request.Execute<Address>();
        }

        /// <summary>
        /// Create an Address.
        /// </summary>
        /// <param name="parameters">
        /// Optional dictionary containing parameters to create the address with. Valid pairs:
        ///   * {"name", string}
        ///   * {"company", string}
        ///   * {"stree1", string}
        ///   * {"street2", string}
        ///   * {"city", string}
        ///   * {"state", string}
        ///   * {"zip", string}
        ///   * {"country", string}
        ///   * {"phone", string}
        ///   * {"email", string}
        /// All invalid keys will be ignored.
        /// </param>
        /// <param name="verifications">
        /// A list of verifications to perform on the address.
        /// Possible items are "delivery" and "zip4".
        /// </param>
        /// <param name="strict_verifications">
        /// A list of verifications to perform on the address.
        /// Will cause an HttpException to be raised if unsucessful.
        /// Possible items are "delivery" and "zip4".
        /// </param>
        /// <returns>EasyPost.Address instance.</returns>
        public static Address Create(Dictionary<string, object> parameters = null) {
            List<string> verifications = null, strictVerifications = null;
            parameters = parameters ?? new Dictionary<string, object>();

            if (parameters.ContainsKey("verifications")) {
                verifications = (List<string>)parameters["verifications"];
                parameters.Remove("verifications");
            }

            if (parameters.ContainsKey("strict_verifications")) {
                strictVerifications = (List<string>)parameters["strict_verifications"];
                parameters.Remove("strict_verifications");
            }

            return sendCreate(parameters, verifications, strictVerifications);
        }

        /// <summary>
        /// Create this Address.
        /// </summary>
        /// <exception cref="ResourceAlreadyCreated">Address already has an id.</exception>
        public void Create() {
            Create(verify, verify_strict);
        }

        /// <summary>
        /// Create this Address.
        /// </summary>
        /// <param name="verifications">
        /// A list of verifications to perform on the address.
        /// Possible items are "delivery" and "zip4".
        /// </param>
        /// <param name="strict_verifications">
        /// A list of verifications to perform on the address.
        /// Will cause an HttpException to be raised if unsucessful.
        /// Possible items are "delivery" and "zip4".
        /// </param>
        /// <exception cref="ResourceAlreadyCreated">Address already has an id.</exception>
        public void Create(List<string> verifications = null, List<string> strictVerifications = null) {
            if (id != null)
                throw new ResourceAlreadyCreated();
            Merge(sendCreate(this.AsDictionary(), verifications, strictVerifications));
        }

        private static Address sendCreate(Dictionary<string, object> parameters, List<string> verifications = null, List<string> strictVerifications = null) {
            Request request = new Request("v2/addresses", Method.POST);
            request.AddBody(new Dictionary<string, object>() { { "address", parameters } });

            foreach (string verification in verifications ?? new List<string>()) {
                request.AddParameter("verify[]", verification, ParameterType.QueryString);
            }

            foreach (string verification in strictVerifications ?? new List<string>()) {
                request.AddParameter("verify_strict[]", verification, ParameterType.QueryString);
            }

            return request.Execute<Address>();
        }

        /// <summary>
        /// Verify an address.
        /// </summary>
        /// <returns>EasyPost.Address instance. Check message for verification failures.</returns>
        public void Verify(string carrier = null) {
            if (id == null)
                Create();

            Request request = new Request("v2/addresses/{id}/verify");
            request.RootElement = "address";
            request.AddUrlSegment("id", id);

            if (carrier != null)
                request.AddParameter("carrier", carrier, ParameterType.QueryString);

            Merge(request.Execute<Address>());
        }

        /// <summary>
        /// Create and verify an Address.
        /// </summary>
        /// <param name="parameters">
        /// Optional dictionary containing parameters to create the address with. Valid pairs:
        ///   * {"name", string}
        ///   * {"company", string}
        ///   * {"stree1", string}
        ///   * {"street2", string}
        ///   * {"city", string}
        ///   * {"state", string}
        ///   * {"zip", string}
        ///   * {"country", string}
        ///   * {"phone", string}
        ///   * {"email", string}
        /// All invalid keys will be ignored.
        /// </param>
        public static Address CreateAndVerify(Dictionary<string, object> parameters = null) {
            parameters["strict_verifications"] = new List<string>() { "delivery" };
            return Address.Create(parameters);
        }
    }
}
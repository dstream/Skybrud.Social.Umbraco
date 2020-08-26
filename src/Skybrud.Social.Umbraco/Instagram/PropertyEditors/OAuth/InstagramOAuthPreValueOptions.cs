using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Social.Json.Extensions.JObject;

namespace Skybrud.Social.Umbraco.Instagram.PropertyEditors.OAuth {

    public class InstagramOAuthPreValueOptions {

        [JsonIgnore]
        public JObject JObject { get; private set; }

        [JsonProperty("clientid")]
        public string ClientId { get; private set; }

        [JsonProperty("clientsecret")]
        public string ClientSecret { get; private set; }

        [JsonProperty("redirecturi")]
        public string RedirectUri { get; private set; }        

        [JsonProperty("scope")]
        public string ScopeStr { get; private set; }

        [JsonProperty("needprofessionalaccount")]
        public bool NeedProfessionalAccount { get; private set; }

        [JsonIgnore]
        public bool IsValid {
            get {
                if (String.IsNullOrEmpty(ClientId)) return false;
                if (String.IsNullOrEmpty(ClientSecret)) return false;
                if (String.IsNullOrEmpty(RedirectUri)) return false;
                return true;
            }
        }

        public static InstagramOAuthPreValueOptions Get(string contentTypeAlias, string propertyAlias) {

            IDictionary<string, string> prevalues = PreValueHelpers.GetPreValues(contentTypeAlias, propertyAlias);
            if (prevalues.ContainsKey("config"))
            {
                string config;
                prevalues.TryGetValue("config", out config);

                try
                {

                    // Parse the JSON for the config/prevalues
                    JObject obj = JObject.Parse(config);

                    var options = new InstagramOAuthPreValueOptions
                    {
                        JObject = obj,
                        ClientId = obj.GetString("clientid"),
                        ClientSecret = obj.GetString("clientsecret"),
                        RedirectUri = obj.GetString("redirecturi"),
                        ScopeStr = obj.GetString("scope") ?? "",
                        NeedProfessionalAccount = obj.GetBoolean("needprofessionalaccount")
                    };

                    // Determine the scope               
                    if (options.NeedProfessionalAccount)
                    {
                        options.ScopeStr = string.Join(",", Enum.GetValues(typeof(InstagramGraphScope)).Cast<InstagramGraphScope>().Select(v => v.ToString()));
                    }
                    else
                    {
                        options.ScopeStr = string.Join(",", Enum.GetValues(typeof(InstagramScope)).Cast<InstagramScope>().Select(v => v.ToString()));
                    }

                    // Initialize a new instance of the options class
                    return options;


                }
                catch (Exception)
                {
                    return new InstagramOAuthPreValueOptions();
                }
            }
            return new InstagramOAuthPreValueOptions();
        }

        public static InstagramOAuthPreValueOptions Get(string dataTypeName)
        {
            if (!string.IsNullOrEmpty(dataTypeName))
            {
                IDictionary<string, string> prevalues = PreValueHelpers.GetPreValues(dataTypeName);

                if (prevalues.ContainsKey("config"))
                {
                    string config;
                    prevalues.TryGetValue("config", out config);

                    try
                    {
                        // Parse the JSON for the config/prevalues
                        JObject obj = JObject.Parse(config);

                        var options = new InstagramOAuthPreValueOptions
                        {
                            JObject = obj,
                            ClientId = obj.GetString("clientid"),
                            ClientSecret = obj.GetString("clientsecret"),
                            RedirectUri = obj.GetString("redirecturi"),
                            ScopeStr = obj.GetString("scope") ?? "",
                            NeedProfessionalAccount = obj.GetBoolean("needprofessionalaccount")
                        };

                        // Determine the scope                
                        if (options.NeedProfessionalAccount)
                        {
                            options.ScopeStr = string.Join(",", Enum.GetValues(typeof(InstagramGraphScope)).Cast<InstagramGraphScope>().Select(v => v.ToString()));
                        }
                        else
                        {
                            options.ScopeStr = string.Join(",", Enum.GetValues(typeof(InstagramScope)).Cast<InstagramScope>().Select(v => v.ToString()));
                        }

                        // Initialize a new instance of the options class
                        return options;

                    }
                    catch (Exception)
                    {
                        return new InstagramOAuthPreValueOptions();
                    }
                }
            }
            return new InstagramOAuthPreValueOptions();
        }

    }    
}

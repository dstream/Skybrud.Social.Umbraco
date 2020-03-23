using System;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Social.Instagram;
using Skybrud.Social.Instagram.OAuth;
using Umbraco.Core;

namespace Skybrud.Social.Umbraco.Instagram.PropertyEditors.OAuth {

    public class InstagramOAuthData {

        #region Private fields

        private InstagramService _service;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of the authenticated user.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets the username of the authenticated user.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets the name of the authenticated user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the full name of the authenticated user.
        /// </summary>
        [JsonProperty("fullname")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets the URL to the profile picture (avatar) of the authenticated user.
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Access token expire date, 60 days after it created normally
        /// </summary>
        [JsonProperty("accessTokenExpireDate")]
        public DateTime AccessTokenExpireDate { get; set; }

        public bool IsExpired() {
            return AccessTokenExpireDate < DateTime.Now;
        }

        /// <summary>
        /// Going to be expired in [days] day
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool IsExpiredSoon(int days)
        {
            return AccessTokenExpireDate < DateTime.Now.AddDays(days);
        }

        /// <summary>
        /// Gets whether the OAuth data is valid - that is whether the OAuth data has a valid
        /// access token or expired. Calling this property will not check the validate the access 
        /// token against the API.
        /// </summary>
        [JsonIgnore]
        public bool IsValid {
            get { return !String.IsNullOrWhiteSpace(AccessToken) && !IsExpired(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the InstagramService class.
        /// </summary>
        public InstagramService GetService() {
            return _service ?? (_service = InstagramService.CreateFromAccessToken(AccessToken));
        }
        
        /// <summary>
        /// Serializes the OAuth data into a JSON string.
        /// </summary>
        public string Serialize() {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserializes the specified JSON string into an OAuth data object.
        /// </summary>
        /// <param name="str">The JSON string to be deserialized.</param>
        public static InstagramOAuthData Deserialize(string str) {
            return JsonConvert.DeserializeObject<InstagramOAuthData>(str);
        }        

        /// <summary>
        /// Refresh long lived access token
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="propertyAlias"></param>
        public void RefreshLongLivedAccessToken(int contentId, string propertyAlias)
        {
            var content = ApplicationContext.Current.Services.ContentService.GetById(contentId);
            if (content != null)
            {
                var currentOAuthData = Deserialize(content.GetValue<string>(propertyAlias));
                if (!string.IsNullOrWhiteSpace(currentOAuthData.AccessToken))
                {
                    var preValues = InstagramOAuthPreValueOptions.Get(content.ContentType.Alias, propertyAlias);
                    if (!string.IsNullOrEmpty(preValues.ClientSecret))
                    {
                        var client = new InstagramOAuthClient
                        {
                            ClientId = preValues.ClientId,
                            ClientSecret = preValues.ClientSecret,
                            RedirectUri = preValues.RedirectUri
                        };
                        var result = client.RefreshLongLivedAccessToken(currentOAuthData.AccessToken);
                        if (!string.IsNullOrEmpty(result.Body.AccessToken))
                        {
                            //Update & save to current content                        
                            currentOAuthData.AccessToken = result.Body.AccessToken;
                            currentOAuthData.AccessTokenExpireDate = DateTime.Now.AddSeconds(result.Body.ExpiresIn);

                            content.SetValue(propertyAlias, currentOAuthData.Serialize());
                            ApplicationContext.Current.Services.ContentService.SaveAndPublishWithStatus(content);
                        }
                    }
                }
            }
        }

        #endregion

    }

}
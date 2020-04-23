using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using Skybrud.Social.Google;
using Skybrud.Social.Google.OAuth;
using Skybrud.Social.Umbraco.Google.PropertyEditors.OAuth;
using Umbraco.Core.Security;

namespace Skybrud.Social.Umbraco.App_Plugins.Skybrud.Social.Dialogs {

    public partial class GoogleOAuth : System.Web.UI.Page {

        #region Umbraco related properties
        
        public string Callback { get; private set; }

        public string ContentTypeAlias { get; private set; }

        public string PropertyAlias { get; private set; }

        public string Feature { get; private set; }

        #endregion

        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode {
            get { return Request.QueryString["code"]; }
        }

        public string AuthState {
            get { return Request.QueryString["state"]; }
        }

        public string AuthErrorReason {
            get { return Request.QueryString["error_reason"]; }
        }

        public string AuthError {
            get { return Request.QueryString["error"]; }
        }

        public string AuthErrorDescription {
            get { return Request.QueryString["error_description"]; }
        }

        protected override void OnPreInit(EventArgs e) {

            base.OnPreInit(e);

            if (PackageHelpers.UmbracoVersion != "7.2.2") return;

            // Handle authentication stuff to counteract bug in Umbraco 7.2.2 (see U4-6342)
            HttpContextWrapper http = new HttpContextWrapper(Context);
            FormsAuthenticationTicket ticket = http.GetUmbracoAuthTicket();
            http.AuthenticateCurrentRequest(ticket, true);

        }

        protected void Page_Load(object sender, EventArgs e) {

            Title = "Google OAuth";

            Callback = Request.QueryString["callback"];
            ContentTypeAlias = Request.QueryString["contentTypeAlias"];
            PropertyAlias = Request.QueryString["propertyAlias"];
            Feature = Request.QueryString["feature"];

            if (AuthState != null) {
                NameValueCollection stateValue = Session["Skybrud.Social_" + AuthState] as NameValueCollection;
                if (stateValue != null) {
                    Callback = stateValue["Callback"];
                    ContentTypeAlias = stateValue["ContentTypeAlias"];
                    PropertyAlias = stateValue["PropertyAlias"];
                    Feature = stateValue["Feature"];
                }
            }

            // Get the prevalue options
            GoogleOAuthPreValueOptions options = GoogleOAuthPreValueOptions.Get(ContentTypeAlias, PropertyAlias);
            if (!options.IsValid) {
                Content.Text += "Hold on now! The options of the underlying prevalue editor isn't valid.";
                return;
            }

            // Configure the OAuth client based on the options of the prevalue options
            GoogleOAuthClient client = new GoogleOAuthClient {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                RedirectUri = options.RedirectUri
            };

            // Session expired?
            if (AuthState != null && Session["Skybrud.Social_" + AuthState] == null) {
                Content.Text = ErrorMessage("Session expired?");
                return;
            }

            // Check whether an error response was received from Google
            if (AuthError != null) {
                Content.Text = ErrorMessage("Error: " + AuthErrorDescription);
                if (AuthState != null) Session.Remove("Skybrud.Social:" + AuthState);
                if (string.IsNullOrEmpty(AuthErrorDescription))
                {
                    //cancel the authentication process
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "callback", "window.close();", true);
                }
                return;
            }

            string state;            

            // Declare the scope
            GoogleScopeCollection defaultScope = new[] {
                    GoogleScopes.OpenId,
                    GoogleScopes.Email,
                    GoogleScopes.Profile
                };

            var scope = options.Scope != null ? string.Join(" ", options.Scope) : defaultScope.ToString();

            // Redirect the user to the Google login dialog
            if (AuthCode == null) {

                // Generate a new unique/random state
                state = Guid.NewGuid().ToString();

                // Save the state in the current user session
                Session["Skybrud.Social_" + state] = new NameValueCollection {
                    { "Callback", Callback},
                    { "ContentTypeAlias", ContentTypeAlias},
                    { "PropertyAlias", PropertyAlias},
                    { "Feature", Feature}
                };
                

                // Construct the authorization URL
                string url = client.GetAuthorizationUrl(state, scope, GoogleAccessType.Offline, GoogleApprovalPrompt.consent);
                
                // Redirect the user
                Response.Redirect(url);
                return;
            
            }

            GoogleAccessTokenResponse info;
            try {
                info = client.GetAccessTokenFromAuthorizationCode(AuthCode);
            } catch (Exception ex) {
                Content.Text = ErrorMessage("<b>Unable to acquire access token</b><br />" + ex.Message);
                return;
            }

            try {

                ////verify the scope, ensure user grant all of scopes
                //var missingPermissions = info.IsAllScopeGranted(scope);
                //if (missingPermissions != null && missingPermissions.Any())
                //{
                //    Content.Text = ErrorMessage($"Missing permissions, please grant me these permissions: {string.Join(", ", missingPermissions)}", true);                    
                    //scope = string.Join(" ", missingPermissions);
                    //// Construct the authorization URL
                    //// Generate a new unique/random state
                    //state = Guid.NewGuid().ToString();
                    //// Save the state in the current user session
                    //Session["Skybrud.Social_" + state] = new NameValueCollection {
                    //    { "Callback", Callback},
                    //    { "ContentTypeAlias", ContentTypeAlias},
                    //    { "PropertyAlias", PropertyAlias},
                    //    { "Feature", Feature}
                    //};

                    //string url = client.GetAuthorizationUrl(state, scope, GoogleAccessType.Offline, GoogleApprovalPrompt.consent) + $"&include_granted_scopes=true";

                    //// Redirect the user
                    //Response.Redirect(url);
                //    return;
                //}

                // Initialize the Google service
                GoogleService service = GoogleService.CreateFromAccessToken(info.AccessToken);

                // Get information about the authenticated user
                GoogleUserInfo user = service.GetUserInfo();

                var locations = service.MyBusiness.Locations.List(user.Id);
                if(locations.Body.Items == null || !locations.Body.Items.Any()) {
                    Content.Text = ErrorMessage("There's no locations in this account, please try another one", true);
                    return;
                }
                
                Content.Text += "<p>Hi <strong>" + user.Name + "</strong></p>";                

                // Set the callback data
                var data = new GoogleOAuthData {
                    Id = user.Id,
                    Name = user.Name,
                    Avatar = user.Picture,
                    ClientId = client.ClientIdFull,
                    ClientSecret = client.ClientSecret,                    
                    RefreshToken = info.RefreshToken
                };

                if (locations.Body.Items.Count() > 1)
                {
                    Content.Text += "<p>Please select a location below: </p>";

                    rptSelectItems.DataSource = locations.Body.Items;
                    rptSelectItems.DataBind();
                    // print the oauth data to the font-end
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "callback", String.Format(
                        "var oautData = {0};",
                        data.Serialize()
                    ), true);
                }
                else
                {
                    Content.Text += "<p>Please wait while you're being redirected...</p>";
                    var location = locations.Body.Items.First();
                    data.LocationUrl = location.Url;
                    data.LocationName = location.Name;                    
                    // Update the UI and close the popup window
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "callback", String.Format(
                        "self.opener." + Callback + "({0}); window.close();",
                        data.Serialize()
                    ), true);
                }                

            } catch (Exception ex) {
                Content.Text = ErrorMessage("<b>Unable to get user information</b><br />" + ex.Message);
                return;
            }

        }

        private string ErrorMessage(string error, bool bold = false)
        {
            return bold ? $"<div class=\"error\"><strong>{error}</strong></div>"
                : $"<div class=\"error\">{error}</div>";
        }
    
    }

}
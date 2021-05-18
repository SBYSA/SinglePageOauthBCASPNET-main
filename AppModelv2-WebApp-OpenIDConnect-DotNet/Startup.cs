using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin;
using Owin;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Web;
using System.IdentityModel.Claims;
using IdentityModel.Client;

[assembly: OwinStartup(typeof(AppModelv2_WebApp_OpenIDConnect_DotNet.Startup))]

namespace AppModelv2_WebApp_OpenIDConnect_DotNet
{
    
    public class Startup
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["redirectUri"];
        string PostredirectUri = System.Configuration.ConfigurationManager.AppSettings["PostredirectUri"];
        // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
        static string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

        string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecretOauth"];

        // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
        string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);
        string tokenuri = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["AccessOauthTokenUrl"], tenant);

        string scope = System.Configuration.ConfigurationManager.AppSettings["ScopeOauth"];
        

        /// <summary>
        /// Configure OWIN to use OpenIdConnect 
        /// </summary>
        /// <param name="app"></param>
        /// 
        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());


            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    // Sets the ClientId, authority, RedirectUri as obtained from web.config
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Authority = authority,
                    RedirectUri = redirectUri,
                    // PostLogoutRedirectUri is the page that users will be redirected to after sign-out. In this case, it is using the home page
                    PostLogoutRedirectUri = redirectUri,
                    //Scope = OpenIdConnectScope.OpenIdProfile,
                    Scope = scope,
                    // ResponseType is set to request the code id_token - which contains basic information about the signed-in user
                    ResponseType = OpenIdConnectResponseType.CodeIdToken,
                    //ResponseType=OpenIdConnectResponseType.Code,

                    // OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        AuthenticationFailed = OnAuthenticationFailed,

                        AuthorizationCodeReceived = async (context) =>
                        {
                             var code = context.Code;
                             var idtoken = context.ProtocolMessage.IdToken;
                             //context.ProtocolMessage.Scope = scope;
                             
                         

                             ClientCredential credential = new ClientCredential(clientId, clientSecret);
                            //string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                            string tenantID = tenant;

                            //string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;

                            AuthenticationContext authContext = new AuthenticationContext("https://login.microsoftonline.com/"+tenant);

                             AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential);



                             if (!string.IsNullOrEmpty(result.AccessToken))
                             {

                                 var userName =result.UserInfo.DisplayableId;

                                 //A améliorer car on perd avec le scope de business central le username et le name dans le claims controller...  ce que j'ai fait en dessous ne fonctionne pas
                                 #region Ne fonctionne pas 

                                 if (!string.IsNullOrEmpty(userName))
                                 {
                                     
                                     //((ClaimsIdentity)incomingPrincipal.Identity).AddClaim(new Claim(ClaimTypes.NameIdentifier, "User"));
                                     ClaimsIdentity claimsIdentity = new ClaimsIdentity();
                                     claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.IdentityModel.Claims.ClaimTypes.NameIdentifier,userName));
                                     System.Security.Claims.ClaimsPrincipal.Current.AddIdentity(claimsIdentity);
                                 }

                                 var fistname = result.UserInfo.GivenName;
                                 var Lastname = result.UserInfo.FamilyName;
                                 var name = Lastname + " " + fistname;
                                 if (!string.IsNullOrEmpty(name))
                                 {
                                     ClaimsIdentity claimsIdentity = new ClaimsIdentity();
                                     claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.IdentityModel.Claims.ClaimTypes.Name, name));
                                     System.Security.Claims.ClaimsPrincipal.Current.AddIdentity(claimsIdentity);

                                 }

                                 #endregion

                                 //  context.AuthenticationTicket.Identity.AddClaim(new System.Security.Claims.Claim("urn:oidc:access_token", result.AccessToken));
                                 var httpClient = new System.Net.Http.HttpClient();
                                 httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

                                 const string environmentsUri = "https://api.businesscentral.dynamics.com/v2.0/d1176b80-8023-4b7a-adc8-99a22336517f/SBLawyer-DEMO/api/SBConsulting/SBLawyer/v1.0/companies(867e17c6-e1ed-ea11-aa61-00224838d3b2)/matters";

                                 var response = httpClient.GetAsync(environmentsUri).Result;
                                 var content = response.Content.ReadAsStringAsync().Result;
                                 Console.WriteLine(content);
                             }
                        }
                    }
                }
            );
        }

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }
}

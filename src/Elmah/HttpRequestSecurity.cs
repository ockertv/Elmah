#region License, Terms and Author(s)
//
// ELMAH - Error Logging Modules and Handlers for ASP.NET
// Copyright (c) 2004-9 Atif Aziz. All rights reserved.
//
//  Author(s):
//
//      Atif Aziz, http://www.raboof.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

[assembly: Elmah.Scc("$Id: HttpRequestSecurity.cs addb64b2f0fa 2012-03-07 18:50:16Z azizatif $")]

namespace Elmah
{
    #region Imports

    using System;
    using System.Web;

    #endregion

    /// <summary>
    /// Security-related helper methods for web requests.
    /// </summary>

    public sealed class HttpRequestSecurity
    {
        /// <summary>
        /// Determines whether the request is from the local computer or not.
        /// </summary>
        /// <remarks>
        /// This method is primarily for .NET Framework 1.x where the
        /// <see cref="HttpRequest.IsLocal"/> was not available.
        /// </remarks>

        public static bool IsLocal(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

#if NET_1_0 || NET_1_1

            string userHostAddress = Mask.NullString(request.UserHostAddress);
            
            return userHostAddress.Equals("127.0.0.1") /* IP v4 */ || 
                userHostAddress.Equals("::1") /* IP v6 */ ||
                userHostAddress.Equals(request.ServerVariables["LOCAL_ADDR"]);
#else
            return request.IsLocal;
#endif
        }

        public static string LoggedOnUsername(HttpRequest request)
        {
            HttpCookie userCookie = request.Cookies["elmah_user"];

            if (userCookie == null) return string.Empty;

            try
            {
                return StrEncrypt.DecryptStringAES(userCookie.Value, SecurityConfiguration.Default.Password, SecurityConfiguration.Default.EncryptionSalt);
            }catch
            {
                return string.Empty;
            }
        }


        public static void LogIn(string username, HttpContext context)
        {
            HttpCookie userCookie = new HttpCookie("elmah_auth");

            userCookie.Value = StrEncrypt.EncryptStringAES(username, SecurityConfiguration.Default.Password, SecurityConfiguration.Default.EncryptionSalt);

            context.Response.Cookies.Add(userCookie);
            
        }
        private HttpRequestSecurity()
        {
            throw new NotSupportedException();
        }
    }
}

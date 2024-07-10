using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using A.AppPreferences;
using A.EmbeddedResourceManager;
using A.User;
using Newtonsoft.Json;

namespace A.Exceptions.UserActionException
{
    public class UserActException : Exception
    {
        private string JsonFilePath = @"A.Exceptions.UserActionExceptionMessages.ErrorMessages.json";
        private string Message { get; set; } = string.Empty;

        private string InnerMessage { get; set; } = string.Empty;

        private string ErrorCode { get; set; }

        private bool ErrorCodeFounded { get; set; } = false;

        public string GetMessage => Message;

        public string GetInnerMessage => InnerMessage;

        public string GetCompleteMessage => $"Exception: {Message} \n Inner Exception: {InnerMessage}";

        public bool GetErrorCodeFounded => ErrorCodeFounded;

        public UserActException() { }

        public UserActException(string errorCode) : base(errorCode)
        {
            this.ErrorCode = errorCode;
            SetErrorCodeMessage(this.ErrorCode);
        }

        /* CASE: Not throwed UserActException but catched System Exception (based on user action) out of which UserActException was created */
        public UserActException(string message, Exception innerExc) : base(message, innerExc)
        {
            this.ErrorCodeFounded = false;
            this.Message = message;
            this.InnerMessage = ((innerExc != null) ? innerExc.Message : string.Empty);
        }

        private void SetErrorCodeMessage(string errorCode)
        {
            string currentLanguage = App.UserData.CurrentCulture;
            string jsonContent = EmbeddedResource.GetEmbeddedJSONFileContent(JsonFilePath);

            if (jsonContent != string.Empty)
            {
                Dictionary<string, Dictionary<string, string>> errorMessages = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);

                if (!errorMessages.ContainsKey(App.UserData.CurrentCulture))
                {
                    currentLanguage = PrefUserSettings.PrefLanguageDefault;
                }

                Dictionary<string, string> dicLocalErrMsgs = errorMessages[currentLanguage];

                if (dicLocalErrMsgs.ContainsKey(errorCode))
                {
                    this.Message = dicLocalErrMsgs[errorCode];
                    this.ErrorCodeFounded = true;
                }
                else
                {
                    this.Message = "Error code not defined.";
                    this.ErrorCodeFounded = false;
                }
            }
            else
            {
                this.Message = "Embedded UAE JSON file could not be read";
                this.ErrorCodeFounded = false;
            }
        }


    }
}
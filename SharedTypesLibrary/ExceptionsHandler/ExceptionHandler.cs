using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharedTypesLibrary.ExceptionsHandler;
using static SharedTypesLibrary.Enums.Enums;

namespace ExceptionsHandling
{
    public class ReplacePlaceHolders
    {
        public static List<string> _listMarks = new List<string>{ "#provider" };
    }

    public class ExceptionHandler : Exception
    {
        public static Dictionary<string, Dictionary<string, string>>? dicJsonContent { get; set; } = null;

        public static void DeserializeJsonExceptionFile(string jsonContent)
        {
            dicJsonContent = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);
        }

        private string MessageCustom { get; set; } = string.Empty;

        private string InnerMessageCustom { get; set; } = string.Empty;

        private string ErrorCodeUser { get; set; }

        private string? ErrorCodeAPI { get; set; }

        private bool ErrorCodeFounded { get; set; } = false;

        public string CustomMessage => MessageCustom;

        public string CustomInnerMesage => InnerMessageCustom;

        public string CustomCompleteMessage => $"Exception: {MessageCustom} \n Inner Exception: {InnerMessageCustom}";

        public bool GetErrorCodeFounded => ErrorCodeFounded;

        public ExceptionHandler() { }

        public ExceptionHandler(string errorCodeUser, string culture) : base(errorCodeUser)
        {
            this.ErrorCodeUser = errorCodeUser;
            SetErrorCodeMessage(this.ErrorCodeUser, errorCodeAPI: null, extraErrorInfo: null, dicReplaceParams: null, culture);
        }

        public ExceptionHandler(string errorCodeUser, string? errorCodeApi, string culture) : base(errorCodeUser)
        {
            this.ErrorCodeUser = errorCodeUser;
            this.ErrorCodeAPI = errorCodeApi;
            SetErrorCodeMessage(this.ErrorCodeUser, errorCodeApi, extraErrorInfo: null, dicReplaceParams: null, culture);
        }

        public ExceptionHandler(string errorCodeUser, string? errorCodeApi, string extraErrors, string culture) : base(errorCodeUser)
        {
            this.ErrorCodeUser = errorCodeUser;
            SetErrorCodeMessage(this.ErrorCodeUser, errorCodeAPI: null, extraErrors, dicReplaceParams: null, culture);
        }

        public ExceptionHandler(string errorCode, string? errorCodeAPI, string extraErrors, Dictionary<string, string>? dicReplaceParams, string culture) : base(errorCode)
        {
            this.ErrorCodeUser = errorCode;
            SetErrorCodeMessage(this.ErrorCodeUser, errorCodeAPI, extraErrors, dicReplaceParams, culture);
        }


        /* CASE: Not throwed ExceptionHandler but catched System Exception (based on user action) out of which ExceptionHandler was created */
        public ExceptionHandler(string message, string culture, Exception? innerExc) : base(message, innerExc)
        {
            this.ErrorCodeFounded = false;
            this.MessageCustom = message;
            this.InnerMessageCustom = ((innerExc != null) ? innerExc.Message : string.Empty);
        }

        private void SetErrorCodeMessage(string errorCodeUser, string? errorCodeAPI, string? extraErrorInfo, Dictionary<string,string>? dicReplaceParams, string culture)
        {
            string currentLanguage = culture;

            if (dicJsonContent != null)
            {
                if (!dicJsonContent.ContainsKey(currentLanguage))
                {
                    currentLanguage = "en";
                }

                Dictionary<string, string> dicLocalErrMsgs = dicJsonContent[currentLanguage];

                if (!string.IsNullOrEmpty(errorCodeUser) && dicLocalErrMsgs.ContainsKey(errorCodeUser))
                {
                    this.MessageCustom = dicLocalErrMsgs[errorCodeUser];
                    this.ErrorCodeFounded = true;
                }

                if (!string.IsNullOrEmpty(errorCodeAPI) && dicLocalErrMsgs.ContainsKey(errorCodeAPI))
                {
                    this.MessageCustom += dicLocalErrMsgs[errorCodeAPI];
                    this.ErrorCodeFounded = true;
                }

                if (!string.IsNullOrEmpty(extraErrorInfo))
                {
                    this.MessageCustom += extraErrorInfo;
                }

                if (dicReplaceParams != null)
                {
                    foreach (KeyValuePair<string,string> item in dicReplaceParams)
                    {
                        this.MessageCustom = this.MessageCustom.Replace(item.Key, item.Value);
                    }
                }

                if (string.IsNullOrEmpty(MessageCustom))
                {
                    this.MessageCustom = "Error code not defined.";
                    this.ErrorCodeFounded = false;
                }
            }
            else
            {
                this.MessageCustom = "Embedded UAE JSON file could not be read";
                this.ErrorCodeFounded = false;
            }
        }

        public static (T? type, ExceptionHandler exception) ReadGenericHttpErrors<T>(T? type, string responseString, string culture)
        {
            if (responseString.Contains("errors"))
            {
                Dictionary<string, List<string>> dicGenericErrors = GenericHttpErrorReader.ExtractErrorsFromWebAPIResponse(responseString);

                var strErrors = string.Empty;

                foreach (var error in dicGenericErrors)
                {
                    foreach (var errorInfo in error.Value)
                        strErrors += errorInfo + "\r\n";
                }

                return (default(T), new ExceptionHandler("UAE_401", null, extraErrors: strErrors, culture));
            }
            else
            {
                return (default(T), new ExceptionHandler("UAE_400", culture));
            }
        }
    }
}
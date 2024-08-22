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
    public class ExceptionHandler : Exception
    {
        public static Dictionary<string, Dictionary<string, string>>? dicJsonContent { get; set; } = null;

        public static void DeserializeJsonExceptionFile(string jsonContent)
        {
            dicJsonContent = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonContent);
        }

        private string MessageCustom { get; set; } = string.Empty;

        private string InnerMessageCustom { get; set; } = string.Empty;

        private string ErrorCode { get; set; }

        private bool ErrorCodeFounded { get; set; } = false;

        public string CustomMessage => MessageCustom;

        public string CustomInnerMesage => InnerMessageCustom;

        public string CustomCompleteMessage => $"Exception: {MessageCustom} \n Inner Exception: {InnerMessageCustom}";

        public bool GetErrorCodeFounded => ErrorCodeFounded;

        public ExceptionHandler() { }

        public ExceptionHandler(string errorCode, string culture) : base(errorCode)
        {
            this.ErrorCode = errorCode;
            SetErrorCodeMessage(this.ErrorCode, "", culture);
        }

        public ExceptionHandler(string errorCode, string extraErrors, string culture) : base(errorCode)
        {
            this.ErrorCode = errorCode;
            SetErrorCodeMessage(this.ErrorCode, extraErrors, culture);
        }

        /* CASE: Not throwed ExceptionHandler but catched System Exception (based on user action) out of which ExceptionHandler was created */
        public ExceptionHandler(string message, string culture, Exception? innerExc) : base(message, innerExc)
        {
            this.ErrorCodeFounded = false;
            this.MessageCustom = message;
            this.InnerMessageCustom = ((innerExc != null) ? innerExc.Message : string.Empty);
        }

        private void SetErrorCodeMessage(string errorCode, string extraErrors, string culture)
        {
            string currentLanguage = culture;

            if (dicJsonContent != null)
            {
                if (!dicJsonContent.ContainsKey(currentLanguage))
                {
                    currentLanguage = "en";
                }

                Dictionary<string, string> dicLocalErrMsgs = dicJsonContent[currentLanguage];

                if (dicLocalErrMsgs.ContainsKey(errorCode))
                {
                    this.MessageCustom = dicLocalErrMsgs[errorCode];
                    this.ErrorCodeFounded = true;

                    if (extraErrors != string.Empty)
                    {
                        this.MessageCustom += extraErrors;
                    }
                }
                else
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

                var temp = string.Empty;

                foreach (var error in dicGenericErrors)
                {
                    foreach (var errorInfo in error.Value)
                        temp += errorInfo + "\r\n";
                }

                return (default(T), new ExceptionHandler("UAE_901", extraErrors: temp, culture));
            }
            else
            {
                return (default(T), new ExceptionHandler("UAE_900", culture));
            }
        }
    }
}
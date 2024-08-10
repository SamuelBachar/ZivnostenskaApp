using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        private string Message { get; set; } = string.Empty;

        private string InnerMessage { get; set; } = string.Empty;

        private string ErrorCode { get; set; }

        private bool ErrorCodeFounded { get; set; } = false;

        public string GetMessage => Message;

        public string GetInnerMessage => InnerMessage;

        public string GetCompleteMessage => $"Exception: {Message} \n Inner Exception: {InnerMessage}";

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
            this.Message = message;
            this.InnerMessage = ((innerExc != null) ? innerExc.Message : string.Empty);
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
                    this.Message = dicLocalErrMsgs[errorCode];
                    this.ErrorCodeFounded = true;

                    if (extraErrors != string.Empty)
                    {
                        this.Message += extraErrors;
                    }
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
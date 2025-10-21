using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.UI
{
    public class ExtendedConsole
    {
        private static ConsoleColor _defaultInfoFG = ConsoleColor.White;

        public static ConsoleColor DefaultInfoForeground
        {
            get { return ExtendedConsole._defaultInfoFG; }
            set { ExtendedConsole._defaultInfoFG = value; }
        }

        private static ConsoleColor _defaultInfoBG = ConsoleColor.Black;

        public static ConsoleColor DefaultInfoBackground
        {
            get { return ExtendedConsole._defaultInfoBG; }
            set { ExtendedConsole._defaultInfoBG = value; }
        }

        private static ConsoleColor _defaultWarningFG = ConsoleColor.Black;

        public static ConsoleColor DefaultWarningForeground
        {
            get { return ExtendedConsole._defaultWarningFG; }
            set { ExtendedConsole._defaultWarningFG = value; }
        }

        private static ConsoleColor _defaultWarningBG = ConsoleColor.Yellow;

        public static ConsoleColor DefaultWarningBackground
        {
            get { return ExtendedConsole._defaultWarningBG; }
            set { ExtendedConsole._defaultWarningBG = value; }
        }

        private static ConsoleColor _defaultErrorFG = ConsoleColor.White;

        public static ConsoleColor DefaultErrorForeground
        {
            get { return ExtendedConsole._defaultErrorFG; }
            set { ExtendedConsole._defaultErrorFG = value; }
        }

        private static ConsoleColor _defaultErrorBG = ConsoleColor.Red;

        public static ConsoleColor DefaultErrorBackground
        {
            get { return ExtendedConsole._defaultErrorBG; }
            set { ExtendedConsole._defaultErrorBG = value; }
        }

        private static ConsoleColor _defaultSuccessFG = ConsoleColor.Green;

        public static ConsoleColor DefaultSuccessForeground
        {
            get { return ExtendedConsole._defaultSuccessFG; }
            set { ExtendedConsole._defaultSuccessFG = value; }
        }

        private static ConsoleColor _defaultSuccessBG = ConsoleColor.Black;

        public static ConsoleColor DefaultSuccessBackground
        {
            get { return ExtendedConsole._defaultSuccessBG; }
            set { ExtendedConsole._defaultSuccessBG = value; }
        }


        private static ConsoleColor _defaultFailureFG = ConsoleColor.Red;

        public static ConsoleColor DefaultFailureForeground
        {
            get { return ExtendedConsole._defaultFailureFG; }
            set { ExtendedConsole._defaultFailureFG = value; }
        }

        private static ConsoleColor _defaultFailureBG = ConsoleColor.Black;

        public static ConsoleColor DefaultFailureBackground
        {
            get { return ExtendedConsole._defaultFailureBG; }
            set { ExtendedConsole._defaultFailureBG = value; }
        }


        private static void Message(NotificationSeverity style,string i_strMessage, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            if (Validator.IsNullString(i_strMessage))
                return;

            ConsoleColor aOrigFG = Console.ForegroundColor;
            ConsoleColor aOrigBG = Console.BackgroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.WriteLine();
            Console.WriteLine("[ "+DateTime.Now+" ] ");

            switch (style)
            {
                case NotificationSeverity.Info:
                    //Console.Write("Info : ");
                    break;

                case NotificationSeverity.Warning:
                    Console.Write("Warning : ");
                    break;
                case NotificationSeverity.Error:
                    Console.Write("Warning : ");
                    break;
            }

            Console.WriteLine(i_strMessage);

            Console.ForegroundColor = aOrigFG;
            Console.BackgroundColor = aOrigBG;

        }

        private static void Message(NotificationSeverity style, string message,bool simple,bool isWriteline)
        {
            if (Validator.IsNullString(message))
                return;

            ConsoleColor aOrigFG = Console.ForegroundColor;
            ConsoleColor aOrigBG = Console.BackgroundColor;

            ConsoleColor aNewFG = aOrigFG;
            ConsoleColor aNewBG = aOrigBG;

            string strCaption = String.Empty;

            switch (style)
            {
                case NotificationSeverity.Info:
                    aNewFG = _defaultInfoFG;
                    aNewBG = _defaultInfoBG;
                    break;

                case NotificationSeverity.Warning:
                    strCaption = "Warning : ";

                    aNewFG = _defaultWarningFG;
                    aNewBG = _defaultWarningBG;
                    break;
                case NotificationSeverity.Error:
                case NotificationSeverity.ErrorCritical:
                    strCaption = "Error : ";

                    aNewFG = _defaultErrorFG;
                    aNewBG = _defaultErrorBG;
                    break;
                case NotificationSeverity.Success:
                    aNewFG = _defaultSuccessFG;
                    aNewBG = _defaultSuccessBG;
                    break;
                case NotificationSeverity.Failure:
                    aNewFG = _defaultFailureFG;
                    aNewBG = _defaultFailureBG;
                    break;
            }

            if (!simple)
            {
                Console.WriteLine("[ " + DateTime.Now + " ]");
            }

            Console.ForegroundColor = aNewFG;
            Console.BackgroundColor = aNewBG;

            if (!simple) {
                Console.Write(strCaption);
            }



            if (!simple || isWriteline)
                Console.WriteLine(message);
            else
                Console.Write(message);

            Console.ForegroundColor = aOrigFG;
            Console.BackgroundColor = aOrigBG;

        }

        public static void Info(string message)
        {
            Message(NotificationSeverity.Info, message,false,false);
        }

        public static void Warning(string message)
        {
            Message(NotificationSeverity.Warning, message,false,false);
        }

        public static void Error(string message)
        {
            Message(NotificationSeverity.Error, message,false,false);
        }

        public static void InfoWrite(string message)
        {
            Message(NotificationSeverity.Info, message, true, false);
        }

        public static void WarningWrite(string message)
        {
            Message(NotificationSeverity.Warning, message, true, false);
        }

        public static void ErrorWrite(string message)
        {
            Message(NotificationSeverity.Error, message, true, false);
        }

        public static void InfoWriteLine(string message)
        {
            Message(NotificationSeverity.Info, message, true, true);
        }

        public static void WarningWriteLine(string message)
        {
            Message(NotificationSeverity.Warning, message, true, true);
        }

        public static void ErrorWriteLine(string message)
        {
            Message(NotificationSeverity.Error, message, true, true);
        }

        public static void Write(NotificationSeverity style, string message)
        {
            Message(style, message, true, false);
        }

        public static void WriteLine(NotificationSeverity style, string message)
        {
            Message(style, message, true, true);
        }

    }
}

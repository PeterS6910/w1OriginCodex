using System;
using System.Windows.Forms;

namespace Contal.IwQuick.UI
{
    public class Dialog
    {
        public const string DEFAULT_ERROR_CAPTION = "Error"; 
        public const string DEFAULT_WARNING_CAPTION = "Warning";
        public const string DEFAULT_INFORMATION_CAPTION = "Information";
        public const string DEFAULT_QUESTION_CAPTION = "Question";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(String message)
        {
            Warning(DEFAULT_WARNING_CAPTION, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        public static void Warning(String caption, String message)
        {
            if (IwQuick.Sys.QuickApp.InteractiveProcesOrService)
                MessageBox.Show(message ?? String.Empty, caption ?? String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                IwQuick.Log.Implicit.Message("Dialog::Warning", NotificationSeverity.Warning, message ?? String.Empty, false, true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Error(String message)
        {
            Error(DEFAULT_ERROR_CAPTION, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        public static void Error(String caption, String message)
        {
            if (IwQuick.Sys.QuickApp.InteractiveProcesOrService)
                MessageBox.Show(message ?? String.Empty, caption ?? String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                IwQuick.Log.Implicit.Message("Dialog::Error", NotificationSeverity.Error, message ?? String.Empty, false, true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public static void Error(Exception error)
        {
            if (null == error)
                return;

            Error(error.GetType().ToString(), error.Message ?? String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Info(String message)
        {
            Info(DEFAULT_INFORMATION_CAPTION, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        public static void Info(String caption, String message)
        {
            if (IwQuick.Sys.QuickApp.InteractiveProcesOrService)
                MessageBox.Show(message ?? String.Empty, caption ?? String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                IwQuick.Log.Implicit.Message("Dialog::Info", NotificationSeverity.Info, message ?? String.Empty, false, true, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Question(String message)
        {
            return Question(DEFAULT_QUESTION_CAPTION, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="defaultButton"></param>
        /// <returns></returns>
        public static bool Question(String message, MessageBoxDefaultButton defaultButton)
        {
            DialogResult dialogResult = 
                MessageBox.Show(
                    message ?? String.Empty, 
                    MessageBoxIcon.Question.ToString(), 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question, 
                    defaultButton);
            return (dialogResult == DialogResult.Yes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Question(String caption, String message)
        {
            DialogResult dialogResult = 
                MessageBox.Show(
                    message ?? String.Empty, 
                    caption ?? String.Empty, 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
            return (dialogResult == DialogResult.Yes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="defaultButton"></param>
        /// <returns></returns>
        public static bool Question(String caption, String message, MessageBoxDefaultButton defaultButton)
        {
            DialogResult dialogResult = 
                MessageBox.Show(
                    message ?? String.Empty, 
                    caption ?? String.Empty, 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question, defaultButton);

            return (dialogResult == DialogResult.Yes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool WarningQuestion(String message)
        {
            return WarningQuestion(DEFAULT_WARNING_CAPTION, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool WarningQuestion(String caption, String message)
        {
            DialogResult dialogResult = 
                MessageBox.Show(
                    message ?? String.Empty, 
                    caption ?? String.Empty, 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

            return (dialogResult == DialogResult.Yes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isYesDefault"></param>
        /// <returns></returns>
        public static bool ErrorQuestion(String message, bool isYesDefault)
        {
            DialogResult dialogResult = 
                MessageBox.Show(
                    message ?? String.Empty, 
                    MessageBoxIcon.Question.ToString(), 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Error,
                    isYesDefault 
                        ? MessageBoxDefaultButton.Button1 
                        : MessageBoxDefaultButton.Button2);
            return (dialogResult == DialogResult.Yes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ErrorQuestion(String message)
        {
            return ErrorQuestion(message ?? String.Empty, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ErrorQuestion(String caption, String message)
        {
            DialogResult dialogResult = 
                MessageBox.Show(
                    message ?? String.Empty, 
                    caption ?? String.Empty, 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Error);

            return (dialogResult == DialogResult.Yes);
        }
    }
}

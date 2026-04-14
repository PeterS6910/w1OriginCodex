using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Contal.IwQuick.Net;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.UI
{
    public partial class ConsoleTextBox : RichTextBox, IConsoleTextBox
    {
        private bool _autoScroll = true;
        private bool _hexMode = false;
        private bool _renderLocalInput = false;
        private int _maxLines = 1024;
        

        private const int EM_GETFIRSTVISIBLELINE = 0xCE;
        private const int WM_VSCROLL = 0x115;
        private const int WM_HSCROLL = 0x114;
        private const int SB_BOTTOM = 7;
        private int _consoleRitchTextBoxPostion = 0;
        private long _blockSize = 16; 
        private bool _lastCharCarriageReturn = false;

        //KeyEventArgs previousKey = null;
        
        public ConsoleTextBox()
        {
            InitializeComponent();            
            MenuStripInit();           
        }     

        public ConsoleTextBox(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            MenuStripInit();                  
            MaxLines = _maxLines;
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SelectionColor = Color.Black;
        }

        public void MenuStripInit()
        {
            this.ContextMenuStrip = contextMenuStripConsoleRTB;
           
            this.toolStripMenuItemCut.Click += new EventHandler(toolStripMenuItemCut_Click);
            this.toolStripMenuItemCopy.Click += new EventHandler(toolStripMenuItemCopy_Click);
            this.toolStripMenuItemPaste.Click += new EventHandler(toolStripMenuItemPaste_Click);            
        }
        void toolStripMenuItemCut_Click(object sender, EventArgs e)
        {
            this.Cut();
        }            

        void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
             this.Copy();                     
        }

        void toolStripMenuItemPaste_Click(object sender, EventArgs e)
        {
            if (SendCommand != null)
            {
                SendCommand(Clipboard.GetText()); 
            }
            if (_renderLocalInput)
            {
                Push(Clipboard.GetText());                
            }
        }       

        /// <summary>
        /// Gets or sets if autoscrolling is enable or disable in ConsoleTextBox 
        /// </summary>
        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                this.Select(this.Text.Length, 0);
                Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, WM_VSCROLL, SB_BOTTOM, 0);
                _autoScroll = value;
            }
        }

        /// <summary>
        /// Gets or sets data forwarding after recieving some
        /// </summary> 
        public bool RenderLocalInput
        {
            get { return _renderLocalInput; }
            set { _renderLocalInput = value; }
        }

        /// <summary>
        /// Gets or sets hexadecimal mode 
        /// </summary>
        /// 
        public bool HexMode
        {
            get { return _hexMode; }
            set { _hexMode = value; }
        }

        /// <summary>
        /// Gets or sets maximum lines in ConsoleTextBox
        /// </summary>
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }

        public void Push(char[] data)
        {
            StringBuilder msgB = new StringBuilder();
            int i = 0;
            if (_hexMode)
            {

                foreach (char letter in data)
                {
                    i++;
                    int value = Convert.ToInt32(letter);
                    msgB.Append(String.Format("{0:X2}", value) + " ");
                    if (i == _blockSize)
                    {
                        msgB.Append("\n");
                        i = 0;
                    }                   
                }
                if (i!=0)
                {
                     msgB.Append("\n");
                }
                Push(msgB.ToString());
            }
            else
            {
                foreach (char letter in data)
                {
                    msgB.Append(letter);
                }
                Push(msgB.ToString());
            }
        }

        public void PushColoredRow(string message, Color fgColor, Color bgColor)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<string, Color, Color>(PushColoredRow), message, fgColor, bgColor);
                }
                catch { }
            }
            else
            {
                int firstCharIndex = this.GetFirstCharIndexOfCurrentLine();

                int coloringLength = 250;
                if (message.Length < coloringLength)
                {
                    int spaceCnt = coloringLength - message.Length;
                    for (int i = 0; i < spaceCnt; i++)
                        message += " ";
                }   
                
                Push(message);
                
                int currentLine = this.GetLineFromCharIndex(firstCharIndex);
                int lastCharIndex = this.GetFirstCharIndexFromLine(currentLine + 1);
                if (lastCharIndex == -1)
                    lastCharIndex = this.TextLength;

                this.SelectionStart = firstCharIndex;
                this.SelectionLength = this.Lines[currentLine].Length;
                this.SelectionColor = fgColor;
                this.SelectionBackColor = bgColor;

                this.DeselectAll();
                this.SelectionColor = Color.Black;
                this.SelectionBackColor = Color.White;
                
                Push("\r\n");
            }
        }

        public void Push(string message, Color fgColor, Color bgColor)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new Action<string, Color, Color>(Push), message, fgColor, bgColor);
                }
                catch (ObjectDisposedException)
                {
                }
            }
            else
            {

                int IndexOfZrero = message.IndexOf('\0');
                if (IndexOfZrero >= 0)
                {
                    message = message.Replace("\0", "");
                }

                // disable redraw                                
                Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, (uint)11, 0, 0);

                lock (this)
                {

                    string[] filteredLines = new string[_maxLines];

                    if (this.Lines.Length > 2 * _maxLines)
                    {
                        this.Select(0, this.GetFirstCharIndexFromLine(_maxLines));
                        this.SelectedText = String.Empty;
                    }
                }

                int linesLenght = this.Lines.Length;

                int lastLineIndexBeforeAppend = linesLenght > 0 ? linesLenght - 1 : 0;
                int oldPos = linesLenght > 0 ? this.TextLength - this.Lines[linesLenght - 1].Length : 0;

                int firstVisibleLineIndex = (int)Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);

                AppendMessageToOutput(message, true);

                int firstMsgChar = this.GetFirstCharIndexFromLine(lastLineIndexBeforeAppend);

                if (firstMsgChar == -1)//test
                {
                    firstMsgChar = 0;
                }

                message = this.Text.Substring(firstMsgChar, this.TextLength - firstMsgChar);

                this.SelectionStart = firstMsgChar;
                this.SelectionLength = this.TextLength - firstMsgChar;
                this.SelectionColor = fgColor;
                this.SelectionBackColor = bgColor;

                /*
                if (_renderLocalInput)
                {
                    this.Select(oldPos, message.Length);
                    this.SelectionColor = Color.Blue;

                    if (message != "\r\n")
                        this.Select(this.Text.Length, 0);
                    else
                        this.DeselectAll();
                }
                */

                if (_autoScroll)
                {
                    this.Select(this.TextLength, 0);
                    Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, WM_VSCROLL, SB_BOTTOM, 0);
                    this.SelectionStart = _consoleRitchTextBoxPostion;
                }
                else
                {
                    this.SelectionStart = this.GetFirstCharIndexFromLine(firstVisibleLineIndex);
                    this.SelectionLength = 0;
                }

                this.DeselectAll();
                this.SelectionColor = Color.Black;
                this.SelectionBackColor = Color.White;

                Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, (uint)11, 1, 0);
                this.Invalidate();
            }
        }

        /// <summary>
        /// Works like RitchTextBox.AppendText() but with special handling 
        /// of incoming messages with chars "\n", "\r", "\b" like
        /// </summary>
        public void Push(string message)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new DString2Void(Push), message);
                }
                catch (ObjectDisposedException)
                {
                }
            }
            else
            {
                
                int IndexOfZrero = message.IndexOf('\0');               
                if (IndexOfZrero >= 0)
                {
                    message = message.Replace("\0", "");
                    
                }
                
                // disable redraw                                
                Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, (uint)11, 0, 0);

                lock (this)
                {

                    string[] filteredLines = new string[_maxLines];

                    if (this.Lines.Length > 2 * _maxLines)
                    {
                        this.Select(0, this.GetFirstCharIndexFromLine(_maxLines));
                        this.SelectedText = String.Empty;
                    }
                }

                //lock (this)
                //{

                //    string[] filteredLines = new string[MAX_MESSAGES_IN_BUFFER];

                //    if (this.Lines.Length > 2 * MAX_MESSAGES_IN_BUFFER)
                //    {
                //        for (int i = 0; i < filteredLines.Length; i++)
                //        {
                //            filteredLines[i] = this.Lines[this.Lines.Length - MAX_MESSAGES_IN_BUFFER + i];
                //        }
                //        this.Lines = filteredLines;
                //    }
                //}

                int linesLenght = this.Lines.Length;

                int lastLineIndexBeforeAppend = linesLenght > 0 ? linesLenght - 1 : 0;
                int oldPos = linesLenght > 0 ? this.TextLength - this.Lines[linesLenght - 1].Length : 0;

                int firstVisibleLineIndex = (int)Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);              

                AppendMessageToOutput(message, true);
                //richTextBox1.AppendText(message);                

                //int linesCountBeforeAppend = linesLenght > 0 ? linesLenght - 1 : 0;                


                if (!_hexMode)
                {
                    //StringBuilder newMessage = new StringBuilder();
                    //for (int i = lastLineIndexBeforeAppend; i < this.Lines.Length; i++)
                    //{
                    //    newMessage.Append(this.Lines[i]);
                    //}

                    int firstMessageCharacterIndexInOutput = this.GetFirstCharIndexFromLine(lastLineIndexBeforeAppend);

                    if (firstMessageCharacterIndexInOutput == -1)//test
                    {
                        firstMessageCharacterIndexInOutput = 0;
                    }

                    message = this.Text.Substring(firstMessageCharacterIndexInOutput, this.TextLength - firstMessageCharacterIndexInOutput);

                    //message = newMessage.ToString();

                    //_output.SelectionStart = oldPos;
                    //_output.SelectionLength = message.Length;
                    //_output.SelectionColor = Color.Black;
                    //_output.SelectionBackColor = Color.White;

                    int endOfLineCharacterIndexInMessage = -1;
                    int indexOfWarningInMessage = message.ToLower().IndexOf("warning");
                    int indexOfErrorInMessage = message.ToLower().IndexOf("error");

                    if (indexOfWarningInMessage >= 0)
                    {
                        //errorOrWarningPos = this.Text.ToLower().IndexOf("warning", errorOrWarningPos);
                        while (indexOfWarningInMessage >= 0)
                        {
                            endOfLineCharacterIndexInMessage = message.IndexOf('\n', indexOfWarningInMessage);
                            if (endOfLineCharacterIndexInMessage >= 0)
                            {
                                this.SelectionStart = firstMessageCharacterIndexInOutput + indexOfWarningInMessage;
                                this.SelectionLength = endOfLineCharacterIndexInMessage - indexOfWarningInMessage;
                                this.SelectionColor = Color.Black;
                                this.SelectionBackColor = Color.Yellow;
                            }
                            else
                            {
                                this.SelectionStart = firstMessageCharacterIndexInOutput + indexOfWarningInMessage;
                                this.SelectionLength = message.Length - indexOfWarningInMessage;
                                this.SelectionColor = Color.Black;
                                this.SelectionBackColor = Color.Yellow;
                            }
                            indexOfWarningInMessage = message.ToLower().IndexOf("warning", indexOfWarningInMessage + 1);
                        }
                    }

                    if (indexOfErrorInMessage >= 0)
                    {
                        while (indexOfErrorInMessage >= 0)
                        {
                            endOfLineCharacterIndexInMessage = message.IndexOf('\n', indexOfErrorInMessage);
                            if (endOfLineCharacterIndexInMessage >= 0)
                            {
                                this.SelectionStart = firstMessageCharacterIndexInOutput + indexOfErrorInMessage;
                                this.SelectionLength = endOfLineCharacterIndexInMessage - indexOfErrorInMessage;
                                this.SelectionColor = Color.Red;
                            }
                            else
                            {
                                this.SelectionStart = firstMessageCharacterIndexInOutput + indexOfErrorInMessage;
                                this.SelectionLength = message.Length - indexOfErrorInMessage;
                                this.SelectionColor = Color.Red;
                            }

                            indexOfErrorInMessage = message.ToLower().IndexOf("error", indexOfErrorInMessage + 1);
                        }
                    }
                }                

                if (_renderLocalInput)
                {
                    this.Select(oldPos, message.Length);
                    this.SelectionColor = Color.Blue;

                    if (message != "\r\n")
                        this.Select(this.Text.Length, 0);
                    else
                        this.DeselectAll();
                }

                if (_autoScroll)
                {
                    this.Select(this.TextLength, 0);
                    Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, WM_VSCROLL, SB_BOTTOM, 0);
                    this.SelectionStart = _consoleRitchTextBoxPostion;
                }
                else
                {
                    this.SelectionStart = this.GetFirstCharIndexFromLine(firstVisibleLineIndex);
                    this.SelectionLength = 0;
                }

                this.DeselectAll();
                this.SelectionColor = Color.Black;
                this.SelectionBackColor = Color.White;

                Contal.IwQuick.Sys.Microsoft.DllUser32.SendMessage(this.Handle, (uint)11, 1, 0);
                this.Invalidate();
            }
        }

        public void AppendMessageToOutput(string message, bool checkCarriageReturn)
        {
            if (message != null && message.Length > 0)
            {
                if (checkCarriageReturn)
                {
                    if (_lastCharCarriageReturn)
                    {
                        _lastCharCarriageReturn = false;
                        message = "\r" + message;
                    }

                    int posCarriageReturn = message.IndexOf('\r');
                    while (posCarriageReturn >= 0)
                    {
                        if (posCarriageReturn < message.Length - 1)
                        {
                            if (message[posCarriageReturn + 1] != '\n')
                            {
                                if (posCarriageReturn > 0)
                                {
                                    string appendText = message.Substring(0, posCarriageReturn);
                                    AppendMessageToOutput(appendText, false);
                                }

                                SetOutputTextPositionToStartLine();
                                message = message.Substring(posCarriageReturn + 1);
                                posCarriageReturn = -1;
                            }

                            posCarriageReturn = message.IndexOf('\r', posCarriageReturn + 1);
                        }
                        else
                        {
                            message = message.Remove(posCarriageReturn);
                            _lastCharCarriageReturn = true;
                            posCarriageReturn = -1;
                        }
                    }
                }

                int posBackSpace = message.IndexOf('\b');
                while (posBackSpace >= 0)
                {
                    int countBackSpace = 1;
                    while (posBackSpace + countBackSpace < message.Length && message[posBackSpace + countBackSpace] == '\b')
                    {
                        countBackSpace++;
                    }

                    if (posBackSpace > 0)
                    {
                        string appendText = message.Substring(0, posBackSpace);
                        AppendMessageToOutput(appendText, false);
                    }

                    int actPos = _consoleRitchTextBoxPostion;
                    for (int i = 0; i < countBackSpace; i++)
                    {
                        if (actPos > 1)
                        {
                            if (this.Text[actPos - 1] == '\n' && this.Text[actPos - 2] == '\r')
                            {
                                actPos--;
                            }
                        }

                        actPos--;
                    }
                    if (actPos == -1 || actPos < 0)
                    {
                        actPos = 0;
                    }
                    this.Select(actPos, _consoleRitchTextBoxPostion - actPos);
                    this.SelectedText = String.Empty;
                    _consoleRitchTextBoxPostion = actPos;

                    message = message.Substring(posBackSpace + countBackSpace);

                    posBackSpace = message.IndexOf('\b');
                }

                AppendTextToOutput(message);
            }
        }

        private void SetOutputTextPositionToStartLine()
        {

            if (_consoleRitchTextBoxPostion > 0 && _consoleRitchTextBoxPostion <= this.TextLength)
            {
                _consoleRitchTextBoxPostion = this.GetFirstCharIndexOfCurrentLine();
            }
        }

        private void AppendTextToOutput(string message)
        {
            if (message != null)
            {
                if (_consoleRitchTextBoxPostion == this.TextLength)
                {
                    this.AppendText(message);
                    _consoleRitchTextBoxPostion = this.TextLength;
                }
                else
                {
                    if (this.TextLength - _consoleRitchTextBoxPostion >= message.Length)
                    {
                        string removeText = message.Replace("\r\n", "");
                        removeText = removeText.Replace("\n", "");
                        this.Select(_consoleRitchTextBoxPostion, removeText.Length);
                        this.SelectedText = message;//
                        _consoleRitchTextBoxPostion = this.TextLength;
                    }
                    else
                    {
                        int length = this.Text.Length - _consoleRitchTextBoxPostion;
                        string replaceText = String.Empty;
                        if (length > 0)
                            replaceText = message.Substring(0, length);
                        string removeText = replaceText.Replace("\r\n", "");
                        removeText = removeText.Replace("\n", "");

                        if (_consoleRitchTextBoxPostion < this.TextLength)
                        {
                            this.Select(_consoleRitchTextBoxPostion, replaceText.Length);
                            //int oldLength = this.TextLength;
                            this.SelectedText = replaceText;
                            message = message.Substring(this.TextLength - _consoleRitchTextBoxPostion);
                        }

                        this.AppendText(message);
                        _consoleRitchTextBoxPostion = this.TextLength;
                    }
                }
            }
        }
        private string GetUTF8DataString(Contal.IwQuick.Data.ByteDataCarrier buffer)
        {
            string sb = buffer.GetUTF8String();
            sb = sb.Replace("\r\r", "\r");
            sb = sb.Replace("\b \b", "\b");

            return sb;
        }
        private void WipeInvisibleCharacters(Contal.IwQuick.Data.ByteDataCarrier buffer)
        {
            for (int i = 0; i < buffer.ActualSize; i++)
            {
                if ((buffer[i] >= 29 && buffer[i] < 32) ||
                    buffer[i] == 0)
                    // space
                    buffer[i] = 32;

            }
        }

        public new void Clear()
        {
            lock (this)
            {
                _lastCharCarriageReturn = false;
                _consoleRitchTextBoxPostion = 0;
                base.Clear();
            }
        }


        public event DChar2Void OnInput;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (OnInput != null)
            {
                e.Handled = true;

                //char[] buffer = null;
                bool newline = false;//e.KeyChar == '\r' || e.KeyChar == '\n';
                //if (e.KeyChar == '\r')
                //{

                //    buffer = new char[2];
                //    buffer[0] = '\r';
                //    buffer[1] = '\n';
                //    newline = true;
                //}
                //else
                //{
                //buffer = new char[1];
                //buffer[0] = e.KeyChar;
                //}

                if (_renderLocalInput)
                {
                    string t = string.Empty;
                    if (newline)
                        t += "\r\n";
                    else
                        t += e.KeyChar;

                    //disable drow chars Ctrl - C Ctrl - V shortcuts 
                    if ((int)e.KeyChar != 22 && ((int)e.KeyChar != 3))
                    {
                        this.Push(t);                        
                    }                   
                }
                OnInput(e.KeyChar);
            }
            else
            {

            }
        }
        public event DString2Void SendCommand;

        protected override void OnKeyDown(KeyEventArgs e)
        {            
             if (e.KeyCode == Keys.Back)
            {
                _consoleRitchTextBoxPostion = this.SelectionStart;
                e.Handled = true;
            }
             if ((e.Control == true) && (e.KeyCode == Keys.V))
             {
                 e.Handled = true;
                 if (_renderLocalInput)
                 {
                     Push(Clipboard.GetText());
                 }   
                 if (SendCommand != null)
                 {
                     SendCommand(Clipboard.GetText());
                 }                             
             }            
        }               
       
        
        protected override void OnDoubleClick(EventArgs e)
        {
            if (Dialog.Question("Do you want to clear the output ?"))
            {
                Clear();

                foreach (ConsoleTextBox peer in this.peers)
                {
                    peer.Clear();
                }
            }
        }

        #region Scroll or erase multiple textboxes simultanously
        private List<ConsoleTextBox> peers = new List<ConsoleTextBox>();

        /// <summary>
        /// Establish a 2-way binding between RTBs for scrolling.
        /// </summary>
        /// <param name="arg">Another RTB</param>
        public void BindScroll(ConsoleTextBox arg)
        {
            if (peers.Contains(arg) || arg == this) { return; }
            peers.Add(arg);
            arg.BindScroll(this);
        }

        private void DirectWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
            {
                foreach (ConsoleTextBox peer in this.peers)
                {
                    Message peerMessage = Message.Create(peer.Handle, m.Msg, m.WParam, m.LParam);
                    peer.DirectWndProc(ref peerMessage);
                }
            }

            base.WndProc(ref m);
        }
        #endregion
    }
}

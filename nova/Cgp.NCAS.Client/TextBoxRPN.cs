using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Client
{
    public partial class TextBoxRPN : RichTextBox
    {
        private Dictionary<string, DictionaryExpressionType> _words;
        private ExpressionExtensions _expressionExtensions = new ExpressionExtensions();
        private List<string> _usedVariables = new List<string>();
        private int _columnHigh;

        public event Action<List<string>> ChangedVariables;

        public TextBoxRPN()
        {
            InitializeComponent();
            AcceptsTab = true;
            Font = new Font(FontFamily.GenericSansSerif, 12);
            EnableAutoDragDrop = false;
            DetectUrls = false;
            WordWrap = false;
            AutoWordSelection = true;
            _intellisenseManager = new IntellisenseManager(this);

            Controls.Add(_intellisenseBox);
            _intellisenseBox.Size = new Size(250, 150);
            _intellisenseBox.Visible = false;
            _intellisenseBox.KeyDown += IntellisenseBox_KeyDown;
            _intellisenseBox.DoubleClick += IntellisenseBox_DoubleClick;

            _columnHigh = (int)Font.Height;
        }

        public int ColumnHigh
        {
            get { return _columnHigh; }
        }

        public void SetDictionary(Dictionary<string, DictionaryExpressionType> dictionary)
        {
            _words = dictionary;
        }

        private bool _enablePainting = true;
        internal bool EnablePainting
        {
            get { return _enablePainting; }
            set { _enablePainting = value; }
        }

        private IntellisenseManager _intellisenseManager;
        private ListBox _intellisenseBox = new ListBox();
        public ListBox IntellisenseBox
        {
            get { return _intellisenseBox; }
            set { _intellisenseBox = value; }
        }
        private TreeView _intellisenseTree = new TreeView();
        public TreeView IntellisenseTree
        {
            get { return _intellisenseTree; }
            set { _intellisenseTree = value; }
        }
        private Keys _intellisenseKey = Keys.Control | Keys.Space;

        private WordStruct _lastWord;
        struct WordStruct
        {
            public int begin;
            public int type;
            string content;
            public int length;

            public string Content
            {
                get { return content; }
                set
                {
                    content = value;
                    if (content == "&&" || content == "||" || content == "+" || content == "-" || content == "+" ||
                        content == "*" || content == "/" || content == "<" || content == ">" || content == "=" ||
                        content == "<=" || content == ">=" || content == "!=" || content == "^")
                        type = 0;
                    else
                        type = 1;
                    if (content == "!")
                        type = 17;
                    if (content == "(" || content == ")")
                        type = 18;

                    length = content.Length;
                }
            }
        }

        void IntellisenseBox_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }
        void IntellisenseBox_DoubleClick(object sender, EventArgs e)
        {
            _intellisenseManager.ConfirmIntellisense();
        }

        private const int WM_COPY = 0x301;
        private const int WM_CUT = 0x300;
        private const int WM_PASTE = 0x302;
        private const int WM_CLEAR = 0x303;
        private const int WM_UNDO = 0x304;
        private const int EM_UNDO = 0xC7;
        private const int EM_CANUNDO = 0xC6;
        private const int WM_PAINT = 0x000F;
        private const int WM_CHAR = 0x102;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    {
                        if (_enablePainting)
                        {
                            base.WndProc(ref m);
                        }
                        else
                        {
                            m.Result = IntPtr.Zero;
                        }
                    }
                    break;
                default:
                    {
                        base.WndProc(ref m);
                    }
                    break;
            }
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            if (EnablePainting)
            {
                base.OnPaint(pe);
            }
        }
        protected override void OnTextChanged(EventArgs e)
        {
            _enablePainting = false;
            IsTermOK(this);
            _enablePainting = true;
            base.OnTextChanged(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            //base.OnKeyPress(e);
            _enablePainting = false;
            MyKeyPress(this, e);
            _enablePainting = true;

            if (e.KeyChar == '.')
            {
                //if (_lastWord == null)
                _intellisenseManager.ShowIntellisenseBox(ObtainActualWord());
                Focus();
            }
        }

        private string ObtainActualWord()
        {
            string actualText = Text.Substring(0, SelectionStart);
            int lastSpace = actualText.LastIndexOf(' ');
            lastSpace++;
            if (lastSpace >= 0)
            {
                actualText = actualText.Substring(lastSpace, actualText.Length - lastSpace);
            }

            if (_words.ContainsKey(actualText))
            {
                DictionaryExpressionType det = _words[actualText];
                return _expressionExtensions.GetAllExtensions(det.ValueType);
            }
            return string.Empty;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == _intellisenseKey)
            {
                _intellisenseManager.ShowIntellisenseBox(ObtainActualWord());
                e.Handled = true;
                Focus();
                return;
            }

            if (_intellisenseBox.Visible)
            {
                #region ESCAPE - Hide Intellisense
                if (e.KeyCode == Keys.Escape)
                {
                    _intellisenseManager.HideIntellisenseBox();
                    e.Handled = true;
                }
                #endregion

                #region Navigation - Up, Down, PageUp, PageDown, Home, End
                else if (e.KeyCode == Keys.Up)
                {
                    _intellisenseManager.NavigateUp(1);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    _intellisenseManager.NavigateDown(1);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.PageUp)
                {
                    _intellisenseManager.NavigateUp(10);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.PageDown)
                {
                    _intellisenseManager.NavigateDown(10);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Home)
                {
                    _intellisenseManager.NavigateHome();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.End)
                {
                    _intellisenseManager.NavigateEnd();
                    e.Handled = true;
                }
                #endregion

                #region Typing - Back
                else if (e.KeyCode == Keys.Back)
                {
                    _intellisenseManager.TypeBackspace();
                }
                #endregion

                #region Typing - Brackets
                else if (e.KeyCode == Keys.D9)
                {
                    // Trap the open bracket key, displaying a cheap and
                    // cheerful tooltip if the word just typed is in our tree
                    // (the parameters are stored in the tag property of the node)
                }
                else if (e.KeyCode == Keys.D8)
                {
                    // Close bracket key, hide the tooltip textbox
                }
                #endregion

                #region Typing - TAB and Enter
                else if (e.KeyCode == Keys.Tab)
                {
                    _intellisenseManager.ConfirmIntellisense();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    _intellisenseManager.ConfirmIntellisense();
                    e.Handled = true;
                }
                #endregion
            }

            base.OnKeyDown(e);
        }

        #region RichTextBoxEvents
        private void MyKeyPress(RichTextBox workingTB, KeyPressEventArgs e)
        {
            //e.KeyChar = Char.ToUpper(e.KeyChar);

            if (e.KeyChar == '(')
            {
                if (InsertParentheses(workingTB))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '&')
            {
                if (InsertPart(workingTB, "&& "))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '|')
            {
                if (InsertPart(workingTB, "|| "))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '+')
            {
                if (InsertPart(workingTB, "+ "))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '-')
            {
                if (InsertPart(workingTB, "- "))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '*')
            {
                if (InsertPart(workingTB, "* "))
                {
                    e.Handled = true;
                    return;
                }
            }
            else if (e.KeyChar == '/')
            {
                if (InsertPart(workingTB, "/ "))
                {
                    e.Handled = true;
                    return;
                }
            }
        }



        #endregion

        #region KeyPressMetod
        private bool InsertParentheses(RichTextBox WorkRTB)
        {
            int DelBod = WorkRTB.SelectionStart;
            int Posun = 0;

            string TextRT = WorkRTB.Text;
            string ResultString = TextRT.Substring(0, DelBod);
            if (ResultString != string.Empty && ResultString[ResultString.Length - 1] != ' ')
            {
                Posun++;
                ResultString += " ";
            }
            ResultString += "() ";
            ResultString += TextRT.Substring(DelBod, WorkRTB.Text.Length - DelBod);

            WorkRTB.Text = ResultString;
            WorkRTB.SelectionStart = DelBod + 1 + Posun;
            WorkRTB.SelectionLength = 0;
            WorkRTB.SelectionColor = Color.Black;
            WorkRTB.ScrollToCaret();
            return true;
        }

        private bool InsertPart(RichTextBox WorkRTB, string InsertString)
        {
            int DelBod = WorkRTB.SelectionStart;
            int Posun = 0;

            string TextRT = WorkRTB.Text;
            string ResultString = TextRT.Substring(0, DelBod);
            if (ResultString != string.Empty && ResultString[ResultString.Length - 1] != ' ')
            {
                Posun++;
                ResultString += " ";
            }
            ResultString += InsertString;
            ResultString += TextRT.Substring(DelBod, WorkRTB.Text.Length - DelBod);

            WorkRTB.Text = ResultString;
            WorkRTB.SelectionStart = DelBod + InsertString.Length + Posun;
            WorkRTB.SelectionLength = 0;
            WorkRTB.SelectionColor = Color.Black;
            WorkRTB.ScrollToCaret();
            return true;
        }
        #endregion

        #region Expression is OK

        private void IsTermOK(RichTextBox workingTB)
        {
            int Position = workingTB.SelectionStart;
            workingTB.SelectionStart = 0;
            workingTB.SelectionLength = workingTB.Text.Length;
            workingTB.SelectionColor = Color.Black;
            workingTB.SelectionLength = 0;
            AllWords(workingTB);
            workingTB.SelectionStart = Position;
            workingTB.SelectionLength = 0;
            workingTB.SelectionColor = Color.Black;
            workingTB.SelectionLength = 0;
        }

        private void AllWords(RichTextBox workingTB)
        {
            string text = workingTB.Text;
            string actString = string.Empty;
            WordStruct actWord;
            _lastWord = new WordStruct();
            _lastWord.Content = string.Empty;
            actWord = new WordStruct();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ' || text[i] == '\n' || text[i] == '\r')
                {
                    continue;
                }

                actString += text[i];
                if ((i == text.Length - 1) || !WillContiue(text[i], text[i + 1]))
                {
                    actWord.Content = actString;
                    actString = string.Empty;
                    actWord.begin = i + 1;
                    CompareWord(workingTB, actWord);
                }
            }

            if (actString != string.Empty)
            {
                actWord.Content = actString;
                actWord.begin = text.Length;
                CompareWord(workingTB, actWord);
            }
        }

        private void AddUsedVariable(string name)
        {
            string cleanName = GetClearName(name);
            if (!_usedVariables.Contains(cleanName))
            {
                _usedVariables.Add(cleanName);
                if (ChangedVariables != null)
                {
                    ChangedVariables(_usedVariables);
                }
            }
        }

        private string GetClearName(string name)
        {
            string[] parts = name.Split('.');
            if (parts.Length == 1)
            {
                return name;
            }
            if (_expressionExtensions.IsStringExtensions(parts[parts.Length - 1]))
            {
                int lastSeparator = name.LastIndexOf('.');
                return name.Substring(0, lastSeparator);
            }
            return name;
        }

        private bool WillContiue(char z1, char z2)
        {
            byte type2 = GetCharMode(z2);
            if (type2 == 10) return true;
            byte type1 = GetCharMode(z1);
            if (type1 == type2)
                return true;
            return false;
        }

        private byte GetCharMode(char pwChar)
        {
            //if (pwChar >= '0' && pwChar <= '9')
                //return 3;
            //if (pwChar >= 'A' && pwChar <= 'Z')
                //return 1;
            if (pwChar == ' ')
                return 0;
            //if (pwChar == '.')
            //    return 10;
            if (pwChar == '&' || pwChar == '|' || pwChar == '^' || pwChar == '*' || pwChar == '<'
                || pwChar == '>' || pwChar == '=' || pwChar == '+' || pwChar == '-' || pwChar == '/')
                return 2;
            if (pwChar == '!')
                return 7;
            if (pwChar == '(')
                return 8;
            if (pwChar == ')')
                return 9;
            return 50;
        }

        private void CompareWord(RichTextBox workingTB, WordStruct actWord)
        {
            if (_lastWord.Content != string.Empty)
            {
                if (actWord.type == _lastWord.type)
                {
                    HighlightWord(workingTB, actWord);
                    HighlightWord(workingTB, _lastWord);
                }
            }
            if (actWord.type == 1)
                HighlighVariable(actWord);
            _lastWord = actWord;
        }

        private void HighlightWord(RichTextBox workingTB, WordStruct act)
        {
            if (act.begin - act.length < 0) return;
            workingTB.SelectionStart = act.begin - act.length;
            workingTB.SelectionLength = act.length;
            workingTB.SelectionColor = Color.Red;
            workingTB.SelectionLength = 0;
        }
        private void HighlighVariable(WordStruct act)
        {
            if (act.begin - act.length < 0) return;
            SelectionStart = act.begin - act.length;
            SelectionLength = act.length;
            if (IsStringInDisctionary(act.Content))
            {
                SelectionColor = Color.Blue;
                AddUsedVariable(act.Content);
            }
            else
                SelectionColor = Color.DarkRed;
            SelectionLength = 0;
        }

        private bool IsStringInDisctionary(string word)
        {
            if (_words.ContainsKey(word))
                return true;

            string[] parts = word.Split('.');
            string findWord = string.Empty;
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (!_expressionExtensions.IsStringExtensions(parts[parts.Length - 1]))
                {
                    return false;
                }
                findWord += parts[i];
                if (i != parts.Length - 2)
                    findWord += ".";
            }
            return _words.ContainsKey(findWord);
        }
        #endregion
       
    }

    internal class IntellisenseManager
    {
        private TextBoxRPN _parentTextBox;

        public IntellisenseManager(TextBoxRPN parent)
        {
            _parentTextBox = parent;
        }

        #region Methods
        /// <summary>
        /// Shows the intellisense box.
        /// </summary>
        public void ShowIntellisenseBox(string showString)
        {
            string actWord = string.Empty;
            if (showString == null || showString == string.Empty) return;
            UpdateIntellisense(showString);
            ShowIntellisenseBoxWithoutUpdate();
        }
        internal void ShowIntellisenseBoxWithoutUpdate()
        {
            if (_parentTextBox.IntellisenseTree == null)
            {
                return;
            }

            //our box has some elements, choose the first
            try
            {
                _parentTextBox.IntellisenseBox.SelectedIndex = 0;
            }
            catch { }


            //Get top-left coordinate for our intellisenseBox
            Point topLeft = _parentTextBox.GetPositionFromCharIndex(_parentTextBox.SelectionStart);
            //topLeft.Offset(-35, 18);
            topLeft.Offset(-35, _parentTextBox.ColumnHigh);

            #region Place the intellisense box, to fit the space...
            if (_parentTextBox.Size.Height < (topLeft.Y + _parentTextBox.IntellisenseBox.Height))
            {
                topLeft.Offset(0, -18 - 18 - _parentTextBox.IntellisenseBox.Height);
            }

            if (_parentTextBox.Size.Width < (topLeft.X + _parentTextBox.IntellisenseBox.Width))
            {
                topLeft.Offset(35 + 15 - _parentTextBox.IntellisenseBox.Width, 0);
            }

            if (topLeft.X < 0)
            {
                topLeft.X = 0;
            }

            if (topLeft.Y < 0)
            {
                topLeft.Y = 0;
            }
            #endregion

            _parentTextBox.IntellisenseBox.Location = topLeft;
            _parentTextBox.IntellisenseBox.Visible = true;
            _parentTextBox.Focus();
        }
        /// <summary>
        /// Hides the intellisense box.
        /// </summary>
        public void HideIntellisenseBox()
        {
            _parentTextBox.IntellisenseBox.Items.Clear();
            _parentTextBox.IntellisenseBox.Visible = false;
        }
        /// <summary>
        /// Navigates up in the intellisense box.
        /// </summary>
        public void NavigateUp(int elements)
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (!_parentTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (_parentTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            if (_parentTextBox.IntellisenseBox.SelectedIndex > elements - 1)
            {
                _parentTextBox.IntellisenseBox.SelectedIndex -= elements;
            }
            else
            {
                _parentTextBox.IntellisenseBox.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// Navigates down in the intellisense box.
        /// </summary>
        public void NavigateDown(int elements)
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (!_parentTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (_parentTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            if (_parentTextBox.IntellisenseBox.SelectedIndex < _parentTextBox.IntellisenseBox.Items.Count - elements - 1)
            {
                _parentTextBox.IntellisenseBox.SelectedIndex += elements;
            }
            else
            {
                _parentTextBox.IntellisenseBox.SelectedIndex = _parentTextBox.IntellisenseBox.Items.Count - 1;
            }
        }
        /// <summary>
        /// Navigates to the first element in the intellisense box.
        /// </summary>
        public void NavigateHome()
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (!_parentTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (_parentTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            _parentTextBox.IntellisenseBox.SelectedIndex = 0;
        }
        /// <summary>
        /// Navigates to the last element in the intellisense box.
        /// </summary>
        public void NavigateEnd()
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (!_parentTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (_parentTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            _parentTextBox.IntellisenseBox.SelectedIndex = _parentTextBox.IntellisenseBox.Items.Count - 1;
        }
        /// <summary>
        /// Calls, when a backspace typed.
        /// </summary>
        public void TypeBackspace()
        {
            //#region Some checkings for the intellisense box
            ////Do nothing if the intellisense is not visible...
            //if (!_parentTextBox.IntellisenseBox.Visible)
            //{
            //    return;
            //}
            //#endregion

            //m_LastCharWasAScopeOperator = false;
            //UpdateIntellisense(false, "", "\b");
        }
        /// <summary>
        /// Calls, when an alphanumerical character typed.
        /// </summary>
        /// <param name="c"></param>
        public void TypeAlphaNumerical(char c)
        {
        }
        /// <summary>
        /// Calls, when a non-alphanumerical character typed.
        /// </summary>
        /// <param name="c"></param>
        public void TypeNonAlphaNumerical(char c)
        {

        }
        /// <summary>
        /// Updates the intellisense box's elements to show the right object list.
        /// </summary>
        /// <param name="forceNextLevel"></param>
        /// <param name="word"></param>
        /// <param name="justRead"></param>
        /// <returns></returns>
        public bool UpdateIntellisense( string word)
        {
            //Clear all elements
            _parentTextBox.IntellisenseBox.Items.Clear();
            string[] words = word.Split(' ');

            foreach (string part in words)
            {
                _parentTextBox.IntellisenseBox.Items.Add(part);
            }
            //Show box
            //ShowIntellisenseBoxWithoutUpdate();
            return true;
        }
        /// <summary>
        /// Confirms the selection from the intellisense, and write the selected text back to the textbox.
        /// </summary>
        public void ConfirmIntellisense()
        {
            try
            {
                _parentTextBox.SelectedText = _parentTextBox.IntellisenseBox.SelectedItem.ToString();
                //_parentTextBox.SelectedText = ((ListBox)_parentTextBox.IntellisenseBox.SelectedItem).Text;
            }
            catch
            {
                return;
            }
            //Hide the intellisense
            HideIntellisenseBox();
        }
        #endregion
    }  
    
}

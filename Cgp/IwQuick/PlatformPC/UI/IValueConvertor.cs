namespace Contal.IwQuick.UI
{
    public interface IValueConvertor
    {
        /// <summary>
        /// converts interger-like value into these properties of control if possible :
        /// numericupdown,progressbar - value
        /// combobox, listbox, listview - selectedindex
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (value > 0 ? true : false)
        /// with other types do nothing YET
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        void ToUI(long value,System.Windows.Forms.Control control);

        /// <summary>
        /// converts interger-like value into these properties of control if possible (including formating):
        /// everything to text, formated by format variable ( if null or empty, only assignment )
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        void ToUI(long value, string format, System.Windows.Forms.Control control);

        /// <summary>
        /// converts float value into these properties of control if possible :
        /// numericupdown,progressbar - value
        /// combobox, listbox, listview - selectedindex
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (value > 0 ? true : false)
        /// with other types do nothing YET
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        void ToUI(double value, System.Windows.Forms.Control control);

        /// <summary>
        /// converts float value into these properties of control if possible (including formating):
        /// everything to text, formated by format variable ( if null or empty, only assignment )
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        void ToUI(double value, string format, System.Windows.Forms.Control control);

        /// <summary>
        /// converts string to the control's property if possible
        /// numericupdown,progressbar - value (if string convertible to decimal)
        /// combobox, listbox, listview - selecteditem
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (if the text equals to value, otherwise unchecked)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        void ToUI(string value, System.Windows.Forms.Control control);


        /*************************************************************************************************************************/



        /// <summary>
        /// retrieves value from the control according these rules :
        /// numericupdown, progressbar - value
        /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
        /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - if checked and text convertible to value, the text
        /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
        /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
        /// 
        /// return true, if conversion succeeded
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        bool FromUI(ref byte value, System.Windows.Forms.Control control);
        bool FromUI(ref short value, System.Windows.Forms.Control control);
        bool FromUI(ref ushort value, System.Windows.Forms.Control control);
        bool FromUI(ref int value, System.Windows.Forms.Control control);
        bool FromUI(ref uint value, System.Windows.Forms.Control control);
        bool FromUI(ref long value, System.Windows.Forms.Control control);
        bool FromUI(ref ulong value, System.Windows.Forms.Control control);

        // using value.TryParse
        bool FromUI(ref byte value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref short value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref ushort value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref int value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref uint value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref long value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref ulong value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);

        /// <summary>
        /// retrieves value from the control according these rules :
        /// numericupdown, progressbar - value
        /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
        /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - if checked and text convertible to value, the text
        /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
        /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
        /// 
        /// return true, if conversion succeeded
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        bool FromUI(ref float value, System.Windows.Forms.Control control);
        bool FromUI(ref double value, System.Windows.Forms.Control control);

        // using value.TryParse
        bool FromUI(ref float value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        bool FromUI(ref double value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);

        /// <summary>
        /// retrieves value from the control property
        /// numericupdown,progressbar - value 
        /// combobox, listbox, listview - selecteditem (it's text)
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (the text of the control, otherwise String.Empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        bool FromUI(ref string value, System.Windows.Forms.Control control);
        
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    public partial class DateTimeEditor
    {
        /// <summary>
        ///  UI we drop down to pick dates.
        /// </summary>
        private class DateTimeUI : Control
        {
            private readonly MonthCalendar _monthCalendar = new DateTimeMonthCalendar();
            private object _value;
            private IWindowsFormsEditorService _edSvc;

            public DateTimeUI()
            {
                InitializeComponent();
                Size = _monthCalendar.SingleMonthSize;
                _monthCalendar.Resize += MonthCalResize;
            }

            public object Value
            {
                get
                {
                    return _value;
                }
            }

            public void End()
            {
                _edSvc = null;
                _value = null;
            }

            private void MonthCalKeyDown(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        OnDateSelected(sender, null);
                        break;
                }
            }

            protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
            {
                base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

                //Resizing the editor to fit to the SingleMonth size after Dpi changed.
                Size = _monthCalendar.SingleMonthSize;
            }

            private void InitializeComponent()
            {
                _monthCalendar.DateSelected += OnDateSelected;
                _monthCalendar.KeyDown += MonthCalKeyDown;
                Controls.Add(_monthCalendar);
            }

            private void MonthCalResize(object sender, EventArgs e)
            {
                Size = _monthCalendar.Size;
            }

            private void OnDateSelected(object sender, DateRangeEventArgs e)
            {
                _value = _monthCalendar.SelectionStart;
                _edSvc.CloseDropDown();
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                _monthCalendar.Focus();
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                _edSvc = edSvc;
                _value = value;

                if (value != null)
                {
                    DateTime dt = (DateTime)value;
                    _monthCalendar.SetDate((dt.Equals(DateTime.MinValue)) ? DateTime.Today : dt);
                }
            }

            class DateTimeMonthCalendar : MonthCalendar
            {
                protected override bool IsInputKey(Keys keyData) => keyData switch
                {
                    Keys.Enter => true,
                    _ => base.IsInputKey(keyData),
                };
            }
        }
    }
}

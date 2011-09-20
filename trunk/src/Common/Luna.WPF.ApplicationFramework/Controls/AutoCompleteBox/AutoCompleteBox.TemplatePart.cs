// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Luna.WPF.ApplicationFramework.Controls.Automation.Peers;
using Properties = Luna.WPF.ApplicationFramework.Controls;
namespace Luna.WPF.ApplicationFramework.Controls
{
    using Luna.WPF.ApplicationFramework.Behaviors;

    /// <summary>
    /// Represents a control that combines a text box and a drop down popup 
    /// containing a selection control. AutoCompleteBox allows users to filter 
    /// an items list.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    [TemplatePart(Name = AutoCompleteTextBox.ElementSelectionAdapter, Type = typeof(ISelectionAdapter))]
    [TemplatePart(Name = AutoCompleteTextBox.ElementTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = AutoCompleteTextBox.ElementPopup, Type = typeof(Popup))]
    [TemplatePart(Name = AutoCompleteTextBox.ElementDropDownToggle, Type = typeof(ToggleButton))]
    [StyleTypedProperty(Property = AutoCompleteTextBox.ElementTextBoxStyle, StyleTargetType = typeof(TextBox))]
    [StyleTypedProperty(Property = AutoCompleteTextBox.ElementItemContainerStyle, StyleTargetType = typeof(ItemsElementBehavior))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StatePopupClosed, GroupName = VisualStates.GroupPopup)]
    [TemplateVisualState(Name = VisualStates.StatePopupOpened, GroupName = VisualStates.GroupPopup)]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Large implementation keeps the components contained.")]
    public partial class AutoCompleteTextBox : Control, IUpdateVisualState
    {
        #region Template part and style names

        /// <summary>
        /// Specifies the name of the ListBox TemplatePart.
        /// </summary>
        private const string ElementSelectionAdapter = "SelectionAdapter";

        /// <summary>
        /// Specifies the name of the ToggleButton TemplatePart.
        /// </summary>
        private const string ElementDropDownToggle = "DropDownToggle";

        /// <summary>
        /// Specifies the name of the Popup TemplatePart.
        /// </summary>
        private const string ElementPopup = "Popup";

        /// <summary>
        /// The name for the text box part.
        /// </summary>
        private const string ElementTextBox = "Text";

        /// <summary>
        /// The name for the text box style.
        /// </summary>
        private const string ElementTextBoxStyle = "TextStyle";

        /// <summary>
        /// The name for the adapter's item container style.
        /// </summary>
        private const string ElementItemContainerStyle = "ContainerStyle";

        #endregion
    }
}

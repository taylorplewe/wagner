﻿#pragma checksum "..\..\Popup.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9C5497964AED597940D7BBFAA2EA3AB37C782D1B564FF3E3048C5965936A4471"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Wagner;


namespace Wagner {
    
    
    /// <summary>
    /// Popup
    /// </summary>
    public partial class Popup : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wagner.Popup window_tile_picker;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid_top_bar;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label_title;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_close;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock text_descr;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox textbox_main;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label_spacer;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel panel_buttons;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_ok;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\Popup.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button button_cancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Wagner;component/popup.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Popup.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.window_tile_picker = ((Wagner.Popup)(target));
            
            #line 14 "..\..\Popup.xaml"
            this.window_tile_picker.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.window_popup_MouseDown);
            
            #line default
            #line hidden
            
            #line 14 "..\..\Popup.xaml"
            this.window_tile_picker.KeyDown += new System.Windows.Input.KeyEventHandler(this.window_popup_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.grid_top_bar = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.label_title = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.button_close = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\Popup.xaml"
            this.button_close.Click += new System.Windows.RoutedEventHandler(this.button_close_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.text_descr = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.textbox_main = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.label_spacer = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.panel_buttons = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 9:
            this.button_ok = ((System.Windows.Controls.Button)(target));
            
            #line 40 "..\..\Popup.xaml"
            this.button_ok.Click += new System.Windows.RoutedEventHandler(this.button_ok_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.button_cancel = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\Popup.xaml"
            this.button_cancel.Click += new System.Windows.RoutedEventHandler(this.button_close_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}


   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      Wagner.Popup   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Wagner     -   $��window_tile_picker��  � Title$  Popup��   style_window#��[   � 
ResizeMode$ NoResize=�$	��280��$	��200�� � WindowStartupLocation$ CenterScreen=� � Topmost . .���  � RowDefinitions �� $
��auto ��� �� $
��auto �.+�� -    �Name$ grid_top_bar��
������  � ColumnDefinitions �� $
��auto ��� �� $
��auto �.+��� -    ��Name$ label_title��  style_title_label#��[ .��{Title here}�� -    ��Name$ button_close��  style_button_window_control#��[ $��2��.��×� $��1��$��	36,8,36,0q�.+��� $��Center=�	 �� Orientation$	 Vertical=�.+��� $��Center=�.���� -   
 ��Name$
 
text_descr��$��WrapWithOverflow=�{Text here}�� -    ��Name$ textbox_main��$y�	Collapsed=�$	��180���� -   $ label_spacer��$y�	Collapsed=�.��	Invisible�� -    ��Name$ panel_buttons��$��10q�$	 
Horizontal=�$��Right=�$��2��.+��� -	   $ 	button_ok��$��8,0,0,0q� �� 	IsDefault . .��OK�� -
   $ button_cancel��$��8,0,0,0q� �� IsCancel . .��Cancel
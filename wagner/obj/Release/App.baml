   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      
Wagner.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     &localclr-namespace:Wagner       �� 	Resources     
brush_form �� Color  brush_dark1  brush_dark2  brush_light1  brush_light2  brush_light3  brush_light3.5  brush_light4  brush_light5 	 style_window �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value	 N� EnterActions
 g� Duration W� From	 W� To �� 
TargetName �� TargetProperty N� ExitActions �� 
TargetName 
 style_title_label'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� From	 �� To ��Name �� 
Background �� Bullet  template_button  style_button_window_control % !style_button_window_control_blank ��Name J� SnapsToDevicePixels �� Padding   Transitions%  System.Windows.VisualTransition   GeneratedDuration	   To8  2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime  �� Value  ComboBoxToggleButton! � ColumnDefinitions" F�Name  ComboBoxTextBox# �� 	ClickMode$ d� 	IsChecked% �� Mode& �� RelativeSource' J� IsHitTestVisible( �� SelectionBoxItem) �� 
IsReadOnly* �� 
IsReadOnly+ �� IsDropDownOpen, � AllowsTransparency- �Name. � ActualWidth/ �� MaxDropDownHeight0 K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation1   DirectionalNavigation2 P� 
SourceName  style_open_close_tab3 �� Triggers  style_close_arrow4 E� IsClosed5 E� 
StartPoint6 �� Point  style_open_arrow  style_anchor_arrow_ul  style_anchor_arrow_ur  style_anchor_arrow_dl  style_anchor_arrow_dr7 �� Orientation8 �Name9 m� IsSubmenuOpen: z� SharedSizeGroup; m� InputGestureText < m� TopLevelHeaderTemplateKey= m� SubmenuItemTemplateKey��%H#  &	        &	      &	 &     &	 9     &	 L     &	 _     &	 r     &	 �     &	 �     &		 �     7   7  '�� �    7  &	
 �    7  '�� }    7  '�� �    7  7  7  '	� �    7  7  '�� &	    7  7  '�� �    7  &	 8    7  &	 �    7   &	 0    7  &	 4    7  7  &	 H    '��     7  7  7  7  7  '�� �    &	 �    7   7  &	 �    7  &	 2    7  &	 �    7  &	 .    7  &	 �    7  &	 ,    7  'o� �    7  (�� [    *$"MenuItem.TopLevelHeaderTemplateKey+)7  7  7  (�� �    *!MenuItem.SubmenuItemTemplateKey+)7  7  'm�      7  7< 7= �� $ #272520���� $ #1c1b17���� $ #0e0d0b���� $ #302e27���� $ #38352e���� $ #464339���� $ #625e50���� $ #a39d8f���� $ #e8e7e3���� " �.���  �@�WindowStyle$
 None=���  �@�AllowsTransparency . ��  � �� �� " �.R��� $��Border��8��  8�� $��1q�._�� .+��� �� $��
ResizeGrip��$��Right=�$��Bottom=�$y�	Collapsed=� P�  � y�$ Visible=�	 �� .���� .��W� $
 0:0:0D�$ 0X�$ 1X�$ Border��$ Opacity� �� .���� .��W� $
 0:0:0D�$ 1X�$ 0X�$ Border��$ Opacity�P�  �@�
ResizeMode$ CanResizeWithGrip=�. ��� $ 
ResizeGrip�� � y�$ Visible=��� " ��.���  � ��8   ��  � ��$ 10, 0, 10, 0q��� " ��.���  � ��8   ��  � ��$ 	Helvetica)���  � ��$ 10, 0, 10, 0q���  � ��$ 14(���  � ��$ Center=��� " ��.���  � ��8   �� " ��.���  � ��8   ��  � ��$ Center=���  � ��$ 0,3,0,5q���  �@J�SnapsToDevicePixels . ��  � �� �� " ��.R��� $��border��$��1q�8�� 8��    CommonStates��.   
Normal��  Disabled��  ReadOnly��  	MouseOver��. �� .���� $ 	#00a39d8fX�$ 	#ff625e50X�$
 0:0:0D�$ border��$2 ,(Border.BorderBrush).(SolidColorBrush.Color)�  Focused��. �� .���� $ 	#00a39d8fX�$ 	#ffa39d8fX�$
 0:0:0D�$ border��$2 ,(Border.BorderBrush).(SolidColorBrush.Color)�._��� $ PART_ContentHost��$��0q��� " 	�.���  � ��$ Center=���  � �� �� " 	�.R��� $��	10,6,10,6q�
 ����  � $��18��$��18��.+�A� $��border��$��1��8��  A� $��	checkMark��$��5q����	#00000000   CommonStates��.   
Normal��  	MouseOver��. �� .���� $ 	#00a39d8fX�$ 	#ff625e50X�$
 0:0:0D�$ border��$, &(Shape.Stroke).(SolidColorBrush.Color)�  Pressed��. ��   Disabled��. �� .���� $ #000000X�$ #625e50X�$
 0:0:0D�$
 text��$4 .(TextBlock.Foreground).(SolidColorBrush.Color)�  CheckStates��.   Checked��. �� .���� $ 	#00000000X�$ 	#ffa39d8fX�$
 0:0:0D�$ 	checkMark��$* $(Shape.Fill).(SolidColorBrush.Color)�  	Unchecked��  Indeterminate��._��� $
��text��$��8,0,0,0q�8�� $��Center=�$
��Left=���. �� " ��.���  � ��$ Center=���  � �� �� " ��.R��� $��	10,6,10,6q�
 ����  �� $��border��$��18��$��18��$��1q�8��  ._��� $��	checkMark��$��4q�
������    CommonStates��.   
Normal��  	MouseOver��. �� .���� $ 	#00a39d8fX�$ 	#ff625e50X�$
 0:0:0D�$ border��$2 ,(Border.BorderBrush).(SolidColorBrush.Color)�  Pressed��. ��   Disabled��  CheckStates��.   Checked��. �� .���� $ 	#00000000X�$ 	#ffa39d8fX�$
 0:0:0D�$ 	checkMark��$1 +(Border.Background).(SolidColorBrush.Color)�  	Unchecked��  Indeterminate��._��� $
��text��$��8,0,0,0q�8�� $��Center=�$
��Left=���. �� " ��.���  � ��# �  ��  � ��8   ��  � ��$ 	12,5,12,5q��� " ��.���  � ��# �  ��  � ��8   ��  � ��$ 	12,1,12,5q���  � ��$ 48����  � ��$ 24(��� " ��.���  � ��# �  ��  � ��8   ��  � ��$ 	12,1,12,5q���  � ��$ 48����  � ��$ 24(��� " ��.R��� $ Border��$��Center=�$��Center=� . #��z��#��z��#��z��# z��   CommonStates��   $ 0:0:0.1D�  $ 0D�$ Pressed��.   
Normal��  	MouseOver��. �� .���� $0 *(Panel.Background).(SolidColorBrush.Color)�$ Border��.g�  $ 0��$  #464339��  Pressed��  Disabled��. �� .���� $4 .(TextBlock.Foreground).(SolidColorBrush.Color)�$
 text��.g�  $ 0��$  #625e50��._��� $
��text��8��  $��Center=�$��Center=�#��z���� " d�.R�� ! �� �� $��24 �   CommonStates��.   
Normal��  	MouseOver��. �� .���� $ #625e50X�$
 0D�$ Border��$2 ,(Border.BorderBrush).(SolidColorBrush.Color)��� $ #625e50X�$
 0D�$ Arrow��$) #(Path.Fill).(SolidColorBrush.Color)�  Pressed��  Disabled��  CheckStates��.   Checked��.+��� $ Border��$��2��
������ $��1q�8��  �� $��0��
������ F� $" Arrow��$��1��$��Center=�$��Center=�"����        р�[ ��[ � �     	8�� �� " ��.R��� $ PART_ContentHost��|�.  #��z���� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � ��$ 80����  � ��$ 20����  � ��8   ��  � �� �� " ��.R��    CommonStates��.   
Normal��  	MouseOver��  Disabled��.+�d� $��ToggleButton��8�� $��2��|�.  $# Press=�$ �� *IsDropDownOpen+$% TwoWay=�& �� *TemplatedParent+�� $��ContentSite��' .  #��z( $��8,3,24,5q�$��Stretch=�$
��Left=��� $��PART_EditableTextBox��8�� $
��Left=�$��Bottom=�$��3,3,23,3q�|�. 
������ $y�Hidden=�#* z) ��Z� � $��Popup��$��Bottom=�#��z+ , . |�.  $��Slide=�.��� $- DropDown�� . #��z. #��z/ .+��� $ DropDownBorder��$��1q�8�� 8�� �� $��1,0,1,0q� . .���� 0 . $1 	Contained=� P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$ 95��P� $2 Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� � ��$ 0,0,0,0q��� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� �� " ��.R��� $��Border��$ 6q� . 
������    CommonStates��.   
Normal��  	MouseOver��. �� .���� $ #38352eX�$
 0:0:0D�$ Border��$0 *(Panel.Background).(SolidColorBrush.Color)�._��� �� " �.���  � ��$ 24����  � ��$ 48����  � ��8   ��  � ��$	 Top=���  � ��$
 Left=�3 P�  �@J�IsMouseOver . . ���  � ��8  �� " F�.���  � ��8   ��  � �� B� ��D� E� 4 . $
5 8,24&���@� �� $6 16,16&��� $6 16,32&��� " F�.���  � ��8   ��  � �� B� ��D� E� 4 . $5 16,24&���@� �� $
6 8,16&��� $
6 8,32&��� " F�.���  � ��8   ��  � �� B� ��D� E� 4 . $	5 0,0&���@� �� $
6 0,16&��� $
6 16,0&��� " F�.���  � ��8   ��  � �� B� ��D� E� 4 . $
5 36,0&���@� �� $6 36,16&��� $
6 20,0&��� " F�.���  � ��8   ��  � �� B� ��D� E� 4 . $
5 0,36&���@� �� $6 16,36&��� $
6 0,20&��� " F�.���  � ��8   ��  � �� B� ��D� E� 4 . $5 36,36&���@� �� $6 36,20&��� $6 20,36&��� " o�.���  �@�OverridesDefaultStyle . ��  �@J�SnapsToDevicePixels . ��  � �� �� " o�.R��� 8��  ._��� }�. $7 
Horizontal=�0 . �� " m�.R��� $ Border��
������ ._�� .+��� $��	14,8,14,8q�$��Header����. � $8 Popup��$��Bottom=�#��z9 , . |�.  $
��Fade=�.���� $��SubmenuBorder�� . 8��  ._��� 0 . $1 Cycle=� P� " �@m�IsSuspendingPopupAnimation . . ��� $ Popup�� � ��$
 None=�P�  �@m�IsHighlighted . . ��� $ Border��8   � ��P� $2 Popup�� �@�AllowsTransparency . . ��� $ SubmenuBorder�� � ��$ 0,0,4,4q��� $ SubmenuBorder�� �@��Padding$ 0,0,0,3q�P�  � {� .  . ��� 8   � ���� " m�.R��� $��Border��
������ ._�� ! �� $
��Auto �$: Checks���� $	��128��$
��auto �$: Middle���� $
��auto �$: Shortcut��.+��� $��Check��$��0q�$y�	Collapsed=��� $��Header��$��1��$��16,0,0,0q�$��Header��$
��Left=�$��Center=���. �� $��2��#��z; $��8q� P�  Z�  � ��P�  �@m�	IsChecked . . ��� $ Check�� �@d�	IsChecked$
 trueY�P�  �@m�IsCheckable . . ��� $ Check�� � y�$ Visible=��� $ Header�� � ��$ 0q�P�  �@m�IsHighlighted . . ��� $ Border��8    � ��P�  � {� .  . ��� 8   � ���� " m�.���  �@�OverridesDefaultStyle . ��  � ��8   3 P�  �@m�Role$ TopLevelHeader=�. ���  � ��8  ��  �@�IsSharedSizeScope . P�  �@m�Role$ SubmenuItem=�. ���  � ��8  
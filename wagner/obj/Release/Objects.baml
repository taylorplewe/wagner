   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      Wagner.Objects   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Wagner     -     �Name$  window_objects�� � Title$ Objects�� � SizeToContent$ WidthAndHeight=�   style_window#��[   � 
ResizeMode$ NoResize=�.��� .+��  � RowDefinitions �� $
��auto ��� .+�� -    �Name$ grid_top_bar��
������  � ColumnDefinitions �� $
��auto ��� �� $
��auto �.+��� -    ��Name$ label_title��  style_title_label#��[ .��	Objects�� -    ��Name$ button_close��  style_button_window_control#��[ $��2��.��×� $��1�� �� $
��auto ��� .+��� -   	 ��Name$	 panel_settings��
 �� Left$
 0�� �� Orientation$ 
Horizontal=�.+��� -    ��Name$ scroller_settings��$��Hidden=�.���� $ Vertical=�$	��256��$	��512��  
brush_form#��[ .+��� $ Vertical=�$��0,0,0,0q�  brush_light1#��[ .+��� .��	Draw mode�� $ Vertical=�$��8,8,0,8q�.+�	� -    	�Name$ 
radio_tile�� d� 	IsChecked$
 TrueY� 	� 	GroupName$ group_draw_mode��.��Tile�� $ Vertical=�$��8,0,0,0q�.+��� -    ��Name$ checkbox_flip_hor��.��Flip horizontally�� -	   $ checkbox_flip_ver��.��Flip vertically	� -
   $ radio_palette��$ group_draw_mode��.��	Palette�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Zoom�� $ 
Horizontal=�$��8,8,8,8q�.+��� -    ��Name$ textbox_zoom��{�.  $��12,0,0,0q�$��48���� $��0q�.��%�� -   $ button_zoom_reset��$��12,0,0,0q�.��Reset�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Object�� $ Vertical=�$��8,8,0,8q�.+��� -    ��Name$ combobox_objects��$��	20,0,20,0q��� $ 
Horizontal=�$��20,8,0,0q�.+��� -   $ button_new_obj��.��New�� -   $ button_remove_obj��$��8,0,0,0q�.��Remove�� -   $ button_rename_obj��$��8,0,0,0q�.��Rename�� $ 
Horizontal=�$��20,8,0,8q�.+��� .��Width�� -   $ textbox_obj_width��$��40���� .��Height�� -   $ textbox_obj_height��$��40��� $��1��#��[  �� �� $
��auto �.+��� -    ��Name$ 
canvas_obj��}�. .+�(� -    (�Name$ viewbox_obj��.���� -    ��Name$ img_obj��$
��None=� �� BitmapScalingMode$ NearestNeighbor=��� -    ��Name$ rect_obj_border��$��1��  brush_light3#��[ �� -   $ rect_map_sel��$��1��$y�Hidden=�� $��1��$��	0,12,0,12q� �� �� .+��� $ Vertical=�.+��� $ 
Horizontal=�$��0,0,0,0q�.+��� .��State�� -   $ combobox_states��$	��180���� $ 
Horizontal=�$��Right=�$��0,8,0,0q�.+��� -   $ button_new_state��.��New�� -   $ button_remove_state��$��8,0,0,0q�.��Remove�� -   $ button_rename_state��$��8,0,0,0q�.��Rename�� $��0,8,0,0q�$ 
Horizontal=�$��Right=�.+��� .��Width�� -   $ textbox_state_width��$��40���� .��Height�� -   $ textbox_state_height��$��40���� $��0,8,0,0q�$ 
Horizontal=�$��Right=�.+��� $��0,0,4,0q�.��This state is listed�� -   $ textbox_state_dup��$��40���� $��2,0,0,0q�.��times�� $��1��$��	12,0,24,0q�$ Vertical=�.+��� $ 
Horizontal=�$��Right=�$��0,0,0,0q�.+��� .��Frame�� -   $ textbox_frame��$��40��{�.  �� $ 
Horizontal=�$��Right=�$��0,8,0,0q�.+��� .��
# frames�� -    $ textbox_num_frames��$��40���� $ 
Horizontal=�$��Right=�$��0,8,0,0q�.+��� .��Speed�� -!   $ textbox_speed��$��40���� $ 
Horizontal=�$��Right=�$��0,8,0,0q�.+��� -"   $ button_play_stop��.��Play
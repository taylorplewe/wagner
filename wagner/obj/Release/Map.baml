   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      
Wagner.Map   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Wagner     -     �Name$  
window_map�� � Title$	 Map��$	��768��$	��800��   style_window#��[   � 
ResizeMode$ CanResizeWithGrip=�.���  � RowDefinitions �� $
��Auto ��� .+�� -    �Name$ grid_top_bar��
������  � ColumnDefinitions �� $
��auto ��� �� $
��auto ��� $
��auto ��� $
��auto �.+��� -    ��Name$ label_title��  style_title_label#��[ .��Map�� -    ��Name$ 
button_min�� % !style_button_window_control_blank#��[ $��2��$��48��$��24(�.��–�� -   $ 
button_max��#��[ $��3��$��48��$��24(�.��+�� -   $ button_close��  style_button_window_control#��[ $��4��.��×� $��1��$��0q�.+��� -    ��Name$ 
canvas_map��}�. .+�(� -   	 (�Name$	 viewbox_map��
 �� Left$
 0��
 �� Top$ 0��.��� .+��� -	    ��Name$ img_map��$
��None=� �� BitmapScalingMode$ NearestNeighbor=��� -
   $ 	img_walls��$y�	Collapsed=�$ NearestNeighbor=�� -   $ 	grid_objs��.+��� -   $ img_plr��$ NearestNeighbor=�� � -   $ 
grid_iobjs��$y�Hidden=�� -   $ grid_camleads��$y�Hidden=�� -   $ 	grid_grid���� -    ��Name$ rect_map_sel��$��1��$y�Hidden=��� -   $ rect_title_safe��$��1��
�������$y�Hidden=��� -   $ rect_measure_8��$��1��
�������$y�	Collapsed=��� -   $ rect_measure_16��$��1��
�������$y�	Collapsed=��� -   $ rect_measure_nt��$��1��
���iii�$y�	Collapsed=��� -    ��Name$ panel_measure��$y�	Collapsed=�$	
 272��$	 128��  brush_light4#��[ .+��� -    ��Name$ text_measure��  brush_dark2#��[ $��10q�Ben�� -   $ panel_drawing��  
brush_form#��[ $	
 256�� �� Orientation$ Vertical=�$	��207��.+��� .��	Drawing� $��10q�  brush_dark1#��[ .+��� -   $ label_drawing��$��Center=�$��Center=�.��	(walls)�� -   $ img_drawing��$	��128��$	��128��$ NearestNeighbor=��� -   $ label_color_mode���� ��Z� .+��� -   $ panel_settings��$
 0��$ 
Horizontal=�.+��� -    ��Name$ scroller_settings��$��Hidden=�.���� $ Vertical=�$	��256��#��[ .+��� $ Vertical=�$��0,0,0,0q�  brush_light1#��[ .+��� .��	Draw mode�� $ Vertical=�$��8,8,0,8q�.+�	� -    	�Name$ 
radio_tile�� 	� 	GroupName$ group_draw_mode��.��Tile	� -   $ radio_palette��$ group_draw_mode��.��	Palette�� -    ��Name$ checkbox_attr_from_meta��$y�	Collapsed=�$��16,0,0,0q�.��Pull attribute from metatile	� -    $ radio_walls��$ group_draw_mode��.��Walls	� -!   $ 
radio_objs��$ group_draw_mode��.��	Objects�� -"   $ 
label_objs��$y�	Collapsed=�$��24,0,0,0q�.��	Object:�� -#    ��Name$ 
combo_objs��$y�	Collapsed=�$��24,0,8,0q��� -$   $ label_remobjs��$y�	Collapsed=�$��24,0,0,0q�	� -%   $ radio_iobjs��$ group_draw_mode��.��	I-objects�� -&   $ label_iobjs��$y�	Collapsed=�$��24,0,0,0q�.��	Object:�� -'   $ combo_iobjs��$y�	Collapsed=�$��24,0,8,0q��� -(   $ label_remiobjs��$y�	Collapsed=�$��24,0,0,0q�	� -)   $ radio_camleads��$ group_draw_mode��.��Camera leads�� -*   $ panel_camleads_0��$y�	Collapsed=�$ Vertical=�$��8,0,0,0q�.+��� $ 
Horizontal=�.+��� .��Cinematic chain:�� -+    ��Name$ textbox_cin_chain��$��40��.��0�� -,   $ checkbox_cin_loc�� d� 	IsChecked$
 TrueY�.��Move to location�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Size�� $ 
Horizontal=�$��8,8,8,8q�.+��� .��Width�� --   $ textbox_width��$��0,0,4,0q�$��48���� -.   $ label_width_units��$��0q�.��	8x8 tiles�� $ 
Horizontal=�$��8,0,8,8q�.+��� .��Height�� -/   $ textbox_height��$��0,0,4,0q�$��48���� -0   $ label_height_units��$��0q�.��	8x8 tiles�� $ 
Horizontal=�$��8,0,8,8q�.+��� $��Center=�.��	Anchor:� -1   $ grid_anchors��
������ .+�F� -2    F�Name$ 	anchor_ul�� 	 style_anchor_arrow_ul#��[	 F� -3   $ 	anchor_ur�� 
 style_anchor_arrow_ur#��[
 F� -4   $ 	anchor_dl��  style_anchor_arrow_dl#��[ F� -5   $ 	anchor_dr��  style_anchor_arrow_dr#��[ �� $ 
Horizontal=�$��20,0,8,8q�.+��� -6   $ button_size_apply��{�.  .��Apply�� -7   $ button_size_revert��$��8,0,0,0q�{�.  .��Revert�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Zoom�� $ 
Horizontal=�$��8,8,8,8q�.+��� -8   $ textbox_zoom��{�.  $��12,0,0,0q�$��48���� $��0q�.��%�� -9   $ button_zoom_reset��$��12,0,0,0q�.��Reset�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Grid�� $ Vertical=�$��8,8,0,8q�.+��� -:   $ checkbox_grid_8x8��.��8x8�� -;   $ checkbox_grid_16x16��.��16x16�� -<   $ checkbox_grid_attribute��.��	Attribute�� -=   $ checkbox_grid_nametable��.��	Nametable�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Fill�� $ 
Horizontal=�$��20,8,8,8q�.+��� ->   $ button_fill_tile��.��Fill with selected tile�� $ 
Horizontal=�$��20,0,8,8q�.+��� -?   $ button_fill_pal��.��Fill with selected palette�� $ 
Horizontal=�$��20,0,8,8q�.+��� -@   $ button_fill_wall��.��Fill with wall�� $ 
Horizontal=�$��20,0,8,8q�.+��� -A   $ button_clear_wall��.��Clear walls�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��View�� -B   $ checkbox_view_grid��$��8,0,0,0q�.��Walls�� -C   $ checkbox_view_objs��$��8,0,0,0q�.��	Objects�� -D   $ panel_view_obj_batches��$y�	Collapsed=�$ Vertical=�$��16,0,0,8q�.+��� -E   $ checkbox_view_objs_0��.��	Batch 0�� -F   $ checkbox_view_iobjs��$��8,0,0,0q�.��	I-objects�� -G   $ checkbox_view_camleads��$��8,0,0,0q�.��Camera leads�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��	Objects�� $ 
Horizontal=�$��8,0,0,8q�.+��� .��Batch:�� -H   $ textbox_batch��$��40��.��0�� .��
# batches:�� -I   $ textbox_num_batches��$��40��.��1
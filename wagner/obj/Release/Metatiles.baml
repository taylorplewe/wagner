   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      Wagner.Metatiles   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Wagner     -     �Name$  window_metatiles��   style_window#��[   � Title$ 	Metatiles�� � SizeToContent$ WidthAndHeight=�.���  � RowDefinitions �� $
��Auto ��� .+�� -    �Name$ grid_top_bar��
������  � ColumnDefinitions �� $
��auto ��� �� $
��auto �.+��� -    ��Name$ label_title��  style_title_label#��[ .��	Metatiles�� -    ��Name$ button_close��  style_button_window_control#��[ $��2��.��×� $��1��.+��� -    ��Name$ canvas_meta��$	��512��$	��512��.+��� -   	 ��Name$	 img_metatiles��$	��512��$	��512��
 �� BitmapScalingMode$
 NearestNeighbor=�� -   $ 	grid_grid���� -    ��Name$ rect_meta_shade��$y�Hidden=�$	��512��$	��512��
���   � J� Opacity$ 0Q��� -	   $	 img_meta_sel��$��64��$��64��$
 NearestNeighbor=��� -
   $ rect_meta_sel2��$��64��$��64�� �� Left$ 0��
 �� Top$ 0��
���  ��$��2��$y�Hidden=��� -   $ rect_meta_sel1��$��64��$��64��$ 0��$ 0��
�������$��2��$y�Hidden=��� -   $ rect_meta_hover��$��32��$��32��
�������$��1��$y�Hidden=��� ��Z� .+��� -    ��Name$ panel_settings��$ 0�� �� Orientation$ 
Horizontal=�.+��� -    ��Name$ scroller_settings��$��Hidden=���.  .���� $ Vertical=� �� CanVerticallyScroll . $	��256��  
brush_form#��[ .+��� $ Vertical=�  brush_light1#��[ .+��� .��Mode�� $ Vertical=�$��8,8,0,8q�.+�	� -    	�Name$ radio_editing�� d� 	IsChecked$
 TrueY� 	� 	GroupName$ 
group_mode��.��	Editing	� -   $ radio_picking��$ 
group_mode��.��	Picking�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��	Edit mode�� $ Vertical=�$��8,8,0,8q�.+�	� -   $ 
radio_tile��$
 TrueY�$ group_edit_mode��.��Tile	� -   $ radio_palette��$ group_edit_mode��.��	Palette�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Window size�� $ Vertical=�$��8,8,0,8q�.+�	� -   $ radio_2x��$ FalseY�$ 
group_zoom��.��2x	� -   $ radio_4x��$
 TrueY�$ 
group_zoom��.��4x�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Grid�� $ Vertical=�$��8,8,0,8q�.+��� -    ��Name$ checkbox_grid_8x8��.��8x8�� -   $ checkbox_grid_16x16��.��16x16�� $ Vertical=�$��0,8,0,0q�#��[ .+��� .��Fill�� $ 
Horizontal=�$��20,8,8,8q�.+��� -   $ button_fill_tile��.��Fill with selected tile�� $ 
Horizontal=�$��20,0,8,8q�.+��� -   $ button_fill_pal��.��Fill with selected palette
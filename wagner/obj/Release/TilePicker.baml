   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      Wagner.TilePicker   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Wagner     -   $��window_tile_picker��  � Title$  Tile picker��   style_window#��[   � 
ResizeMode$ NoResize=� � SizeToContent$ WidthAndHeight=�$��1q� � AllowsTransparency . .��� .+���  �� Orientation$ Vertical=�.+�� -    �Name$ grid_top_bar��
������  � ColumnDefinitions �� $
��auto ��� �� $
��auto �.+��� -    ��Name$ label_title��  style_title_label#��[ .��Tile picker�� -    ��Name$ button_close��  style_button_window_control#��[ $��2��.��×�� $ 
Horizontal=�.+��� -   	 ��Name$	 panel_palettes��$��64��$��	10,10,0,0q�.+��� -   
 ��Name$
 canvas_palette_1��$��64��.+��� -    ��Name$ rect_pal_1_1��$��64��$��16���� -   $ rect_pal_1_2��
 �� Top$ 16��$��64��$��16���� -	   $ rect_pal_1_3��$ 32��$��64��$��16���� -
   $ rect_pal_1_4��$ 48��$��64��$��16���� -   $ rect_pal_1_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -   $
 canvas_palette_2��$��0,10,0,0q�$��64��.+��� -   $ rect_pal_2_1��$��64��$��16���� -   $ rect_pal_2_2��$ 16��$��64��$��16���� -   $ rect_pal_2_3��$ 32��$��64��$��16���� -   $ rect_pal_2_4��$ 48��$��64��$��16���� -   $ rect_pal_2_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -   $
 canvas_palette_3��$��0,10,0,0q�$��64��.+��� -   $ rect_pal_3_1��$��64��$��16���� -   $ rect_pal_3_2��$ 16��$��64��$��16���� -   $ rect_pal_3_3��$ 32��$��64��$��16���� -   $ rect_pal_3_4��$ 48��$��64��$��16���� -   $ rect_pal_3_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -   $
 canvas_palette_4��$��0,10,0,0q�$��64��.+��� -   $ rect_pal_4_1��$��64��$��16���� -   $ rect_pal_4_2��$ 16��$��64��$��16���� -   $ rect_pal_4_3��$ 32��$��64��$��16���� -   $ rect_pal_4_4��$ 48��$��64��$��16���� -   $ rect_pal_4_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -   $
 canvas_spr_palette_1��$��64��$��0,24,0,0q�.+��� -   $ rect_spr_pal_1_1��$��64��$��16���� -    $ rect_spr_pal_1_2��$ 16��$��64��$��16���� -!   $ rect_spr_pal_1_3��$ 32��$��64��$��16���� -"   $ rect_spr_pal_1_4��$ 48��$��64��$��16���� -#   $ rect_spr_pal_1_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -$   $
 canvas_spr_palette_2��$��0,10,0,0q�$��64��.+��� -%   $ rect_spr_pal_2_1��$��64��$��16���� -&   $ rect_spr_pal_2_2��$ 16��$��64��$��16���� -'   $ rect_spr_pal_2_3��$ 32��$��64��$��16���� -(   $ rect_spr_pal_2_4��$ 48��$��64��$��16���� -)   $ rect_spr_pal_2_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -*   $
 canvas_spr_palette_3��$��0,10,0,0q�$��64��.+��� -+   $ rect_spr_pal_3_1��$��64��$��16���� -,   $ rect_spr_pal_3_2��$ 16��$��64��$��16���� --   $ rect_spr_pal_3_3��$ 32��$��64��$��16���� -.   $ rect_spr_pal_3_4��$ 48��$��64��$��16���� -/   $ rect_spr_pal_3_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -0   $
 canvas_spr_palette_4��$��0,10,0,0q�$��64��.+��� -1   $ rect_spr_pal_4_1��$��64��$��16���� -2   $ rect_spr_pal_4_2��$ 16��$��64��$��16���� -3   $ rect_spr_pal_4_3��$ 32��$��64��$��16���� -4   $ rect_spr_pal_4_4��$ 48��$��64��$��16���� -5   $ rect_spr_pal_4_sel��$��64��$��64��
�������$��4��$y�Hidden=��� -6   $ button_pal_edit��$��0,24,0,0q�.��Edit�� $ Vertical=�.+��� $ 
Horizontal=�$��10q�.+��� -7   $
 
canvas_chr��$��0,0,0,0q�$	��512��$	��512�� J� Clip ��  �� Rect$ 0,0,512,512�.+��� -8   $
 canvas_chr_scroll��$ 0��.+��� -9    ��Name$ img_chr�� �� BitmapScalingMode$ NearestNeighbor=�$	��512��$
��1024��$ 0���� -:   $ rect_chr_shade��$	��512��$
��1024��
���   � J� Opacity$ 0Q��� -;   $ img_chr_sel��$ NearestNeighbor=�$��32��$��32���� -<   $ rect_chr_sel2��$��32��$��32��
���  ��$��2��$y�Hidden=��� -=   $ rect_chr_sel1��$��32��$��32�� �� Left$ 0��$ 0��
�������$��2��$y�Hidden=��� ->   $ rect_chr_hover��$��32��$��32��
�������$��1��$y�Hidden=��� $ 
Horizontal=�$��10,0,0,0q�.+��� -?   $
 canvas_color_picker��$	��512��$	��128��$��0, 0, 0, 10q�.+��� -@   $ 
img_colors���� -A   $ rect_isolate_color��$	��512��$	��128��$y�	Collapsed=��� -B   $ rect_color_picker_sel��
�������$��2��$��32��$��32��$y�Hidden=�� -C   $ grid_color_picker_hex��$
��32px��$
��32px��.+��� -D   $ label_color_picker_hex��
�������$��0q�$��Center=�$��Center=���  �� Right$ 0�� �� TextAlignment$ Right=�  brush_light3#��[ .��
Color mode�� -E   $ label_color_mode��$ 0��$ 20��$ Right=�
�������.��	Wikipedia
   M S B A M L   `   `   `  ���� A  =Wagner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WagnerWagner      Wagner.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Wagner     -     � Title$  Wagner��$	��800��$	��800��   
brush_form#��[   � 
ResizeMode$ CanMinimize=� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  .��� -    �Name$ panel_allContent�� � ColumnDefinitions �� $
��655* ��� $
��138* �.+��� $��40,70,40,30.4q�$��2��.+���  �� Orientation$ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Object graphics file (.asm):�� -    ��Name$ textbox_objsfile��{�.  $	��360���� -    ��Name$ button_objs��$��8,0,0,0q�.��...�� -   $ button_objs_im��$��8,0,0,0q�{�.  .��Import�� -   $ button_objs_ex��$��8,0,0,0q�.��Export�� $ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Player graphics file (.asm):�� -   $ textbox_plrfile��{�.  $	��360���� -   $ 
button_plr��$��8,0,0,0q�.��...�� -	   $ button_plr_im��$��8,0,0,0q�{�.  .��Import�� -
   $ button_plr_ex��$��8,0,0,0q�.��Export�� $ 
Horizontal=�$��Right=�$��0,56,0,0q�.+��� .��CHR file (.bin):�� -   $ textbox_chrfile��$	��360��{�.  �� -   $ 
button_chr��$��8,0,0,0q�.��...�� -   $ button_chr_im��$��8,0,0,0q�{�.  .��Import�� -   $ button_chr_ex��$��8,0,0,0q�$y�Hidden=�.��Export�� $ 
Horizontal=�$��Right=�$��
0,10,128,0q�.+��� .��Appears in room data file as:�� -   $ textbox_chr_data_appear��$	��300���� $ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Metatiles file (.asm):�� -   $ textbox_metatilesfile��{�.  $	��360���� -   $ button_metatiles��$��8,0,0,0q�.��...�� -   $ button_metatiles_im��$��8,0,0,0q�{�.  .��Import�� -   $ button_metatiles_ex��$��8,0,0,0q�.��Export�� $ 
Horizontal=�$��Right=�$��
0,10,128,0q�.+��� .��Appears in room data file as:�� -   $# textbox_metatiles_data_appear��$	��300���� $ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Map tiles file (.asm):�� -   $ textbox_maptilesfile��{�.  $	��360���� -   $ button_maptiles��$��8,0,0,0q�.��...�� -   $ button_maptiles_im��$��8,0,0,0q�{�.  .��Import�� -   $ button_maptiles_ex��$��8,0,0,0q�{�.  .��Export�� $ 
Horizontal=�$��Right=�$��
0,10,128,0q�.+��� .��Appears in room data file as:�� -   $" textbox_maptiles_data_appear��$	��300���� $ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Map attributes file (.asm):�� -   $ textbox_mapattrsfile��{�.  $	��360���� -   $ button_mapattrs��$��8,0,0,0q�.��...�� -   $ button_mapattrs_im��$��8,0,0,0q�{�.  .��Import�� -   $ button_mapattrs_ex��$��8,0,0,0q�{�.  .��Export�� $ 
Horizontal=�$��Right=�$��
0,10,128,0q�.+��� .��Appears in room data file as:�� -   $" textbox_mapattrs_data_appear��$	��300���� $ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Walls file (.asm):�� -   $ textbox_wallsfile��{�.  $	��360���� -    $ button_walls��$��8,0,0,0q�.��...�� -!   $ button_walls_im��$��8,0,0,0q�{�.  .��Import�� -"   $ button_walls_ex��$��8,0,0,0q�.��Export�� $ 
Horizontal=�$��Right=�$��
0,10,128,0q�.+��� .��Appears in room data file as:�� -#   $ textbox_walls_data_appear��$	��300���� $ 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� .��Room data file (.asm):�� -$   $ textbox_roomfile��{�.  $	��360���� -%   $ button_room��$��8,0,0,0q�.��...�� -&   $ button_room_im��$y�Hidden=�$��8,0,0,0q�{�.  .��Import�� -'   $ button_room_ex��$��8,0,0,0q�.��Export�� -(    ��Name$ checkbox_meta��$��Center=�$��0,20,0,0q�.��Use metatiles�� $ 
Horizontal=�$��Center=�.+��� .��Bank:�� -)   $ textbox_bank��$��40��.��0�� $��0,8,0,0q�$ 
Horizontal=�$��Center=�.+��� -*   $ button_import��.��
Import all�� -+   $ button_export��$��8,0,0,0q�.��
Export all�� $ Vertical=�$��2��$��0,0,-0.4,0.4q�.+�o� .@�m� $
��File��.@�m� -,   	 m�Name$	 	menu_open��$��Open...��
 m� InputGestureText$
 Ctrl + O��m� --   $	 	menu_save��$
��Save��$
 Ctrl + S��m� -.   $	 menu_saveas��$��
Save as...��m� -/   $	 menu_save_export��$��Save & export all��$
 Ctrl + Shift + S��m� -0   $	 	menu_exit��$
��Exit��$
 Ctrl + W��m� $��Colors��.@�m� -1   $	 	menu_wiki��$��	Wikipedia��$
 	Shift + W��m� -2   $	 
menu_mesen��$��Mesen��$
 	Shift + M��m� -3   $	 menu_ase��$��Aseprite��$
 	Shift + A��m� -4   $	 	menu_theo��$��Theoretical��$
 	Shift + T��m� -5   $	 
menu_yychr��$��yy-chr��$
 	Shift + Y��m� -6   $	 
menu_fceux��$��FCEUX��$
 	Shift + F��m� $��Window��.@�m� -7   $	 menu_tile_picker��$��Tile picker�� m� IsCheckable . $
 Ctrl + T��m� -8   $	 menu_map��$��
Map editor�� . $
 Ctrl + M��m� -9   $	 	menu_meta��$��	Metatiles�� . $
 Ctrl + E��m� -:   $	 	menu_objs��$��Objects�� . $
 Ctrl + B��m� -;   $	 
menu_iobjs��$��	I-objects�� . $
 Ctrl + I��m� $
��Help��.@�m� -<   $	 
menu_about��$��About...��
   M S B A M L   `   `   `  ���� A  =Tetris, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:TetrisTetris      Tetris.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     (controlclr-namespace:Tetris     -     �Name$  mainViewModel�� � WindowStyle$ 
ToolWindow=�$	��640��$	��800�� � Title$ Tetris��$	��640��$	��800��.���  � ColumnDefinitions �� $	��410 ��� .+�   Tetris.BoardView  $��0��   TetrisBoard ��  �� ElementName$ mainViewModel�� �� Path$ DataContext.TetrisBoard� �� UpdateSourceTrigger$ PropertyChanged=�   
IsGameOver �� $ mainViewModel��$ DataContext.IsGameOver�	 �� Mode$	 TwoWay=��� $��1��
 �� Orientation$
 Vertical=�
�������.+��� $��5,5,5,5q�$
 
Horizontal=�
�������.+��� $��5,5,5,5q�$��Game Status��$	��200��$��Black"�
�������.��(� $	��100��$
��Left=�.���  � RowDefinitions �� �� .+��� $��0��$��Level���� 2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve$��1��$��Center=����� *	GameLevel+$	 TwoWay=��� $��5,5,5,5q�$��
Next Block��$��Black"�
�������.��   Tetris.BlockControl  $��Right=�   
BlockBoard �� $ mainViewModel��$ DataContext.BlockBoard�$ PropertyChanged=��� $��5,5,5,5q�$��Tetris��$��Black"�
�������
�������$��1q�.���  �� $	��100 ��� $��10 ��� $	��100 �.+��� $��0��$��30������ *StartPauseResume+$	 TwoWay=����� *StartGameCommand+�� $��4��$��30��$��Reset������ *ResetGameCommand+�� $��5,5,5,5q�$��Black"�
�������
�������.���� $
 Vertical=�.+�(� $��40��$
��Left=�.���� $
 
Horizontal=�.+��� $��Current Score��$��80��$��Medium"��� $	��110������ *Score+$	 TwoWay=�(� $��40��$
��Left=�.���� $
 
Horizontal=�.+��� $��
High Score��$��80��$��Medium"��� $	��110������ *	HighScore+$	 TwoWay=��� $��5,5,5,5q�$��Controls��$��Black"�
�������
�������.���  �� $��25 ��� $��25 ��� $��25 ��� $��25 ��� $��25 � �� $	��25* ��� $	��12* ��� $	��37* �.+��� $��0��$��	Move Left��$��Center=�$��2��$	��0,5q��� $��1��$��
Move Right��$��Center=�$��2��$��0,5,0,4q��� $��2��$��
Land Block��$��Center=�$��2��$��0,5,0,4q��� $��3��$��Rotate AntiClock Wise��$��Center=�$��2��$��0,5,0,4q��� $��4��$��Rotate Clock Wise��$��Center=�$��2��$��0,5,0,4q��� $��0��$��2��$��Key Left��$��Center=�$	��0,5q��� $��1��$��2��$��	Key Right��$��Center=�$��0,5,0,4q��� $��2��$��2��$��Key Down��$��Center=�$��0,5,0,4q��� $��3��$��2��$��A��$��Center=�$��0,5,0,4q��� $��4��$��2��$��S��$��Center=�$��0,5,0,4q��� $	��100���� 
�������
�������$�� ��$��2q�.���� $1��+2017 © Bhagwat Kumar. All rights reserved.��$��Right=�$��SemiBold"�$��Italic$�$��Center=�
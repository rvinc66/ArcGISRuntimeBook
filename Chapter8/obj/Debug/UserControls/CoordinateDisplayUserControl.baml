   M S B A M L   `   `   `  ����C  ?Chapter8, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Chapter8.ViewModelsChapter8.ViewModels  5      8    2Chapter8.UserControls.CoordinateDisplayUserControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0vm!clr-namespace:Chapter8.ViewModels     5        � 	Resources  5
      4   .Chapter8.ViewModels.CoordinateDisplayViewModel   Locator��%   &	          5   
   6(   5      .��5      �  � DataContext 6   �� *CoordinateDisplayViewModel+ �� Source# [  .+�5   
   ��  �� Orientation$ 
Horizontal=�6   .+�5      �� 
������ 
���   �$��1q�6C   $
��Left=�5      $	��Top=�6,   $��30q�5       �� Padding$ 20q�6    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5      ._�5      �� .+�5      �� .+�5      �� $ 
Horizontal=�6%   .+�5      �� -   5         ��Name$ Latitude��5   (   
�������$
��Left=�5      $��Center=�68   $��
Latitude: ��5   !   $��80��64   $
��Wrap=�6?   $
��Bold"�6S   �� 5      -   5        $ LatitudeValue��5   (   
�������$
��Left=�5   !   $��Center=�6<    �� TextAlignment$ Right=�6W   $��80��5    )   $
��Bold"�64   ��5   !   �� *
Latitude+ �� Mode$ TwoWay=�	 �� UpdateSourceTrigger$	 PropertyChanged=�# [  5    4   5!      �� 5"      $ 
Horizontal=�6!   .+�5#      �� -   5        $ 	Longitude��5#   (   
�������$
��Left=�5$      $��Center=�68   $��Longitude: ��5%      $
��Wrap=�60   $
��Bold"�6D   �� 5&      -   5        $ LongitudeValue��5&   (   
�������$
��Left=�5'   !   $��Bottom=�6<   $ Right=�6W   $��95��5*   )   $
��Bold"�64   ��5(   !   �� *	Longitude+$ TwoWay=�$	 PropertyChanged=�# [  5*   4   5+      5,      5-      5.      5/      50      51      
   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=3.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	    NetworkDiscoveryGUI$   NetworkDiscoveryGUI.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    _
extToolkitJhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit/extended    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4xctk)http://schemas.xceed.com/wpf/xaml/toolkit   5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   HWPFTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5   
   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  6G     � Title$  Nulodgic Discovery Tool��5	   	    � 
ResizeMode$ NoResize=�6*   $	��600��6@   $
��1000��6M      MaterialDesignBody#���   5
   
   $��Regular"�5   
   $��13(�5   
   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   
      TextRenderingMode$
 Auto=�5   
     MaterialDesignPaper#���  5   
   .��5      %   Xceed.Wpf.Toolkit.BusyIndicator  -   5          Name$ BusyIndicator��5      .��5   
   � .+�5      � -   5         �Name$ regularContent��5       J� RenderTransformOrigin$ 0.503,0.958&�6+   ��5      �� 5       �� EndPoint$ 0.5,1&�6*    �� 
StartPoint$ 0.5,0&�6;   .��5      � 	 � Color$	 Black��6'   
 � Offset$
 0Q�65   � 5      $	 	#FFC7B7F1��6'   $
 1Q�69   5      .+�5      �� -   5         ��Name$ start_discovery��5   )   $��Start��6   $
��Left=�6B   $��	31,45,0,0q�6u   $	��Top=�6�   $	��117��6�   $��36��6�   $��Normal"�6�    J� Opacity$	 0.8Q�6�   ���	#FFE8E8E8��5      �� 5      $ 0.5,1&�6.   $ 0.5,0&�6?   .��5      � $	 Black��6+   $
 0Q�69   � 5      $	 	#FFBBB1F7��6+   $
 1Q�6=   5      5       �� 5!      -   5        $ push_discovery��5!   (   $
��Push��6   $
��Left=�6@   $��
184,45,0,0q�6[   $	��Top=�6�   $	��117��6�   $��36��6�   $��Normal"�6�   $	 0.8Q�6�   ���	#FFEBEAEE��5"      �� 5#      $ 0.5,1&�6.   $ 0.5,0&�6?   .��5$      � $	 Black��6+   $
 0Q�69   � 5%      $	 	#FFBBB1F7��6+   $
 1Q�6=   5&      5(      &   System.Windows.Controls.DataGrid  5)      -   5          Name$ 
discovered��5)         
IsReadOnly .    AutoGenerateColumns .  $��31,139,37,48.5q�6]   ���	#895A5ACF���	#DD000000���	#FFCCCCD6 � 	Resources 5*      =  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType �� Property �� Value��%�   '        �� 5+      "  6    .�5,      ��  � �� 5-   "   �� 5.   &   .R�5/   *   �� $
��Wrap=�6E   ��64   �� 6E   50   '   52      53      54         Columns 55      0  *System.Windows.Controls.DataGridTextColumn  56      ,  &System.Windows.Controls.DataGridColumn   Header$ Display Name��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 57   ;   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6   �� *displayName+6;     58      $ MAC Address��6-   $ * 59   :    6   �� *
macAddress+6:     5:      $ 
IP Address��6-   $ * 5;   9    6   �� *	ipAddress+69     5<      $ Operating System��6-   $ * 5=   2    6   �� *os+62     5>      $ Manufacturer��6-   $ 2* 5?   4    6   �� *manf+64   5A      �� 5B      -   5         ��Name$ count��5B      $��Right=�6+   $��
0,103,37,0q�6G   $
��Wrap=�6[   $	��Top=�6o   $��31��6�   $	��147��6�   ���	#DDFFFFFF$��16(�5C      $��Forte)�6+   $ 0.475,1.756&�6>   �� 5D      -   5         ��Name$ range��5D      $��Right=�6)   $��
0,90,399,0q�6E    �� TextWrapping$
 Wrap=�6Y   $	��Top=�6m   $��30��6�   $	��270��6�   ���	#DDFFFFFF$��16(�5E      $��Forte)�6+   $ 0.475,1.756&�6>   �� 5F      $-��'Enter IP Range(Valid: 192.168.10.1-254)��6   $��Right=�6L   $��0,103,669,0q�6h   $
��Wrap=�6}   $	��Top=�6�   $��31��6�   $	��289��6�   ���	#DDFFFFFF$��16(�5G      $��Forte)�6    $ 0.475,1.756&�63   5J      �� 5K      -   5        $
��SBar��5K      $��0��6$   $��5��64   $��Bottom=�6A   
����ح�.@�5L      �� ���	#FF8084DC$��33.96��67   .��5M      �� 	Status:6)   5N      �� 5O      ���	#FF8084DC$��33.96��67   .��5P      �� -	   5        $ Percent��5P       0%65   5Q      5R      5S      5T      5U      